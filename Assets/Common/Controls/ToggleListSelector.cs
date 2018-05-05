#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 11:27:06
    * 说    明：       1.这是一个简单类型的Toggle列表生成器
                       2.传入字符串数组
                       3.返回id或者对应的字符串
* ************************************************************************************/
#endregion
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace BridgeUI.Common
{
    [System.Serializable]
    public class ToggleListSelector:MonoBehaviour
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
        private bool inited;
        /// <summary>
        /// 调用其他方法时，先进行初始化
        /// </summary>
        public void Awake()
        {
            gameObjectPool = UIFacade.PanelPool;
            if (m_Select) m_Select.onClick.AddListener(TrySelectItem);
            group = m_parent.GetComponentInChildren<ToggleGroup>();
            if (group == null)
                group = m_parent.gameObject.AddComponent<ToggleGroup>();
            inited = true;
        }

        public void OpenSelect(string[] selectables, UnityAction<string> onChoise, bool quick = false)
        {
            if(JudgeInit())
            {
                this.options = selectables;
                this.quick = quick;
                this.onChoise = onChoise;
                CreateUIList(selectables);
            }
        }

       
        public void OpenSelect(string[] selectables, UnityAction<int> onChoise, bool quick = false)
        {
            if (JudgeInit())
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
        }

        public void SetActiveItem(string key)
        {
            if (JudgeInit())
            {
                stopEvent = true;

                if (createdDic.ContainsKey(key))
                {
                    createdDic[key].isOn = true;
                }
                stopEvent = false;
            }
        }
        private bool JudgeInit()
        {
            Debug.Assert(inited, this + ":please init first!!!");
            return inited;
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
