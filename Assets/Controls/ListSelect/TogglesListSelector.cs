using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
namespace BridgeUI.Control
{
    public class TogglesListSelector : MonoBehaviour
    {
        [SerializeField]
        private Transform m_parent;
        [SerializeField]
        private GameObject m_prefab;
        [SerializeField]
        private Button m_Select;
        private string selected;
        private UnityAction<int> onChoiseIndex;
        private UnityAction<int[]> onChoiseIndexs;
        private Dictionary<string, GameObject> createdDic;
        private ToggleGroup group;
        private bool quick;
        private string[] options;
        public bool AnyToggleOn { get { return group.AnyTogglesOn(); } }
        private bool stopEvent;
        private bool muti = false;
        private List<int> selecteds = new List<int>();
        private event UnityAction onResetEvent;
        private List<GameObject> objectPool = new List<GameObject>();
        private void Awake()
        {
            if (m_Select) m_Select.onClick.AddListener(TrySelectItem);
            group = GetComponentInChildren<ToggleGroup>();
            if (group == null)
                group = gameObject.AddComponent<ToggleGroup>();
        }

        public void OpenSelectID(string[] selectables, UnityAction<int> onChoise, bool quick = false)
        {
            this.options = selectables;
            this.quick = quick;
            this.onChoiseIndex = onChoise;
            this.muti = false;
            CreateUIList(selectables);
        }

        public void OpenSelectIDs(string[] selectables, UnityAction<int[]> onChoise)
        {
            this.options = selectables;
            this.quick = false;
            this.onChoiseIndexs = onChoise;
            this.muti = true;
            selecteds.Clear();
            CreateUIList(selectables);
        }

        public void SetActiveItem(string key)
        {
            stopEvent = true;

            if (createdDic.ContainsKey(key))
            {
                createdDic[key].GetComponentInChildren<Toggle>().isOn = true;
            }
            stopEvent = false;
        }

        void TrySelectItem()
        {
            if (muti)
            {
                if (selecteds.Count == 0) return;

                if (this.onChoiseIndexs != null)
                    onChoiseIndexs.Invoke(selecteds.ToArray());
            }
            else
            {
                if (!group.AnyTogglesOn()) return;

                if (this.onChoiseIndex != null)
                {
                    var id = System.Array.IndexOf(options, selected);
                    onChoiseIndex.Invoke(id);
                }
            }

        }

        void OnSelect(bool isOn, string type, int id)
        {
            if (isOn)
            {
                if (!selecteds.Contains(id)) selecteds.Add(id);
                selected = type;
                if (!muti && quick)
                {
                    TrySelectItem();
                }
            }
            else
            {
                if (selecteds.Contains(id)) selecteds.Remove(id);
            }

        }
        void CreateUIList(string[] selectables)
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            if (createdDic == null)
            {
                createdDic = new Dictionary<string, GameObject>();
            }
            else
            {
                foreach (var item in createdDic)
                {
                    var toggle = item.Value.GetComponentInChildren<Toggle>();
                    //toggle.onValueChanged.RemoveAllListeners();
                    toggle.group = null;
                    toggle.isOn = false;
                    SavePoolObject(item.Value.gameObject);
                }
                if (onResetEvent != null)
                {
                    onResetEvent.Invoke();
                    onResetEvent = delegate { };
                }
                createdDic.Clear();
            }

            for (int i = 0; i < selectables.Length; i++)
            {
                //Debug.Log(SceneMain.Current);
                var item = GetPoolObject();
                var type = selectables[i];
                item.GetComponentInChildren<Text>().text = type;
                var toggle = item.GetComponentInChildren<Toggle>();
                Debug.Assert(toggle, "预制体或子物体上没有toggle组件");
                toggle.group = muti ? null : group;
                var index = i;
                UnityAction<bool> action = (x) => { if (!stopEvent) OnSelect(x, type, index); };
                toggle.onValueChanged.AddListener(action);
                onResetEvent += () =>
                {
                    if (toggle != null)
                        toggle.onValueChanged.RemoveListener(action);
                };
                createdDic.Add(type, item);
            }
        }

        internal void SetSelect(int defultValue)
        {
            if (options.Length <= defultValue) return;

            var toogleName = options[defultValue];
            var tog = createdDic[toogleName].GetComponentInChildren<Toggle>();
            tog.isOn = false;
            tog.isOn = true;
        }

        private GameObject GetPoolObject()
        {
            GameObject instence = null;
            if(objectPool.Count > 0)
            {
                instence = objectPool[0];
                instence.gameObject.SetActive(true);
                objectPool.RemoveAt(0);
            }
            else
            {
                instence = Instantiate(m_prefab.gameObject);
                instence.transform.SetParent(m_parent,false);
            }
            return instence;
        }
        private void SavePoolObject(GameObject obj)
        {
            objectPool.Add(obj);
            obj.gameObject.SetActive(false);
        }
    }
}