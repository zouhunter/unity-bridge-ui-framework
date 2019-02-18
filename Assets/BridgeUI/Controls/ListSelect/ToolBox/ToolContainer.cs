using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
using BridgeUI.Control;
using System.Linq;

namespace BridgeUI.Control
{
    public class ToolContainer : ListViewer
    {
        [SerializeField]
        protected float _distence = 1;
        protected GameObject template;
        protected List<ToolData> _toolDatas;
        protected Ray ray;
        protected RaycastHit[] hits;
        protected List<GameObject> pool = new List<GameObject>();
        public UnityAction<GameObject> onActive { get; set; }
        public UnityAction<GameObject> onUse { get; set; }
        public UnityAction<GameObject> onHide { get; set; }
        public List<ToolData> tools { get { return _toolDatas; } set { _toolDatas = value; ResetItems(_toolDatas); } }
        public readonly List<string> clampTags = new List<string>();
        public float distence { get { return _distence; }set { if (_distence > 0.2f) _distence = value; } }
        private Camera _catchedCamera;
        private Camera activeCamera
        {
            get
            {
                if(_catchedCamera ==null ||!_catchedCamera.isActiveAndEnabled)
                {
                    if (Camera.main)
                    {
                        _catchedCamera = Camera.main;
                    }
                    else
                    {
                        _catchedCamera = FindObjectOfType<Camera>();
                    }
                }
                return _catchedCamera;
            }
        }

        protected virtual void ResetItems(List<ToolData> list)
        {
            ResetAllCreated();

            var items = CreateItems(list.Count);
            for (int i = 0; i < items.Length; i++)
            {
                var icon = items[i].GetComponent<ToolIconItem>();
                var info = list[i];
                icon.Init(info.title, info.sprite, info.texture);
                icon.onDrag = OnDragItem;
                icon.onPointDown = OnPointDown;
                icon.onPointUp = OnPointUp;
            }
        }

        protected virtual void OnPointUp(string arg0)
        {
            if (template != null)
            {
                var behaiver = template.GetComponent<IUseAble>();

                if(behaiver != null)
                {
                    Debug.Log("应用:" + behaiver);
                    var used = behaiver.TryUse(() =>
                    {
                        if (onUse != null)
                        {
                            onUse.Invoke(template);
                        }
                    });

                    if ((used && behaiver.hideOnUse) || !used)
                    {
                        SaveBack(template);
                    }
                }
                else
                {
                    SaveBack(template);
                }

            }
            template = null;
        }

        public void SaveBack(GameObject template)
        {
            template.gameObject.SetActive(false);

            if (onHide != null)
                onHide.Invoke(template);
        }

        protected virtual void OnPointDown(string arg0)
        {
            template = GetItemFromPool(arg0);

            if (onActive != null)
                onActive.Invoke(template);

            OnDragItem(arg0);
        }

        protected virtual void OnDragItem(string arg0)
        {
            if (template != null && activeCamera)
            {
                var pos = activeCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distence));
                var minDistence = Vector3.Distance(pos, activeCamera.transform.position);

                if (clampTags != null && clampTags.Count > 0)
                {
                    ray = activeCamera.ScreenPointToRay(Input.mousePosition);
                    hits = Physics.RaycastAll(ray);
                    var grounds = hits.Where(hit => clampTags.Contains(hit.collider.gameObject.tag)).ToArray();

                    if (grounds.Length > 0)
                    {
                        
                        for (int i = 1; i < grounds.Length; i++){
                            var groundToCameraDistence = Vector3.Distance(grounds[i].point, activeCamera.transform.position);
                            if(groundToCameraDistence < minDistence)
                            {
                                minDistence = groundToCameraDistence;
                                pos = grounds[i].point;
                            }
                        }
                    }
                }
                ///设置最小距离
                SetItemPosition(template, pos);
            }
        }

        protected virtual void SetItemPosition(GameObject template, Vector3 pos)
        {
            var setPosAble = template.GetComponent<ICustomMove>();
            if (setPosAble != null)
            {
                setPosAble.SetPosition(pos);
            }
            else
            {
                template.transform.position = pos;
            }
        }

        protected virtual void ResetAllCreated()
        {
            var length = pool.Count;

            for (int i = 0; i < length; i++)
            {
                if (!pool[i])
                {
                    pool.RemoveAt(i);
                    length--;
                }
                else
                {
                    var useAble = pool[i].GetComponent<IUseAble>();
                    if (useAble != null){
                        useAble.OnReset();
                    }

                    pool[i].gameObject.SetActive(false);
                }
            }
        }

        protected virtual GameObject GetItemFromPool(string arg0)
        {
            var item = pool.Find(x => x && x.name == arg0 && !x.gameObject.activeSelf);
            if (item == null)
            {
                var pointItem = tools.Find(x => x.title == arg0);
                item = Instantiate(pointItem.prefab);
                item.name = arg0;
                var newBehaiver = item.GetComponent<ICustomMove>();
                if (newBehaiver != null){
                    newBehaiver.oringalPos = pointItem.prefab.transform.position;
                }

                pool.Add(item);
            }

            item.gameObject.SetActive(true);
            return item;
        }

        protected virtual void OnDestroy()
        {
            foreach (var item in pool)
            {
                if (item && item.gameObject)
                    Destroy(item.gameObject);
            }
        }
    }
}