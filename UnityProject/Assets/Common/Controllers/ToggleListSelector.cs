using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace BridgeUI.Common
{
    [System.Serializable]
    public class ToggleListSelector
    {
        [SerializeField]
        private Transform m_parent;
        [SerializeField]
        private GameObject m_prefab;
        [SerializeField]
        private Button m_Select;
        private string selected;
        private UnityAction<string> onChoise;
        private Dictionary<string, Toggle> createdDic;
        private ToggleGroup group;
        private bool quick;
        private string[] options;
        public bool AnyToggleOn { get { return group.AnyTogglesOn(); } }
        private bool stopEvent;
        private GameObjectPool gameObjectPool;

        public ToggleListSelector(Transform parnent)
        {
            gameObjectPool = UIFacade.PanelPool;
            if (m_Select) m_Select.onClick.AddListener(TrySelectItem);
            group = parnent.GetComponentInChildren<ToggleGroup>();
            if (group == null)
                group = parnent.gameObject.AddComponent<ToggleGroup>();
        }

        public void OpenSelect(string[] selectables, UnityAction<string> onChoise, bool quick = false)
        {
            this.options = selectables;
            this.quick = quick;
            this.onChoise = onChoise;
            CreateUIList(selectables);
        }

        public void OpenSelect(string[] selectables, UnityAction<int> onChoise, bool quick = false)
        {
            this.options = selectables;
            this.quick = quick;
            this.onChoise = (x) =>
            {
                if (onChoise != null)
                {
                    var id = System.Array.IndexOf(options, selected);
                    onChoise.Invoke(id);
                }
            };
            CreateUIList(selectables);
        }

        public void SetActiveItem(string key)
        {
            stopEvent = true;

            if (createdDic.ContainsKey(key))
            {
                createdDic[key].isOn = true;
            }
            stopEvent = false;
        }

        void TrySelectItem()
        {
            if (this.onChoise != null)
            {
                this.onChoise.Invoke(selected);
            }

        }
        void OnSelect(string type)
        {
            selected = type;
            if (quick)
            {
                TrySelectItem();
            }
        }
        void CreateUIList(string[] selectables)
        {
            if (createdDic == null)
            {
                createdDic = new Dictionary<string, Toggle>();
            }
            else
            {
                foreach (var item in createdDic)
                {
                    var toggle = item.Value;
                    toggle.onValueChanged.RemoveAllListeners();
                    gameObjectPool.SavePoolObject(item.Value.gameObject, false);
                }
                createdDic.Clear();
            }

            for (int i = 0; i < selectables.Length; i++)
            {
                //Debug.Log(SceneMain.Current);
                var item = gameObjectPool.GetPoolObject(m_prefab.gameObject, m_parent, false);
                var type = selectables[i];
                item.GetComponentInChildren<Text>().text = type;
                var toggle = item.GetComponentInChildren<Toggle>();
                Debug.Assert(toggle, "预制体或子物体上没有toggle组件");
                toggle.group = group;
                toggle.onValueChanged.AddListener((x) => { if (!stopEvent) OnSelect(type); });
                createdDic.Add(type, toggle);
            }
        }
    }
}
