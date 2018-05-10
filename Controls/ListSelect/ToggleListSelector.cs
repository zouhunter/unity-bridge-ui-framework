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

namespace BridgeUI.Control
{
    public class ToggleListSelector:ListSelector
    {
        [SerializeField]
        private Button m_Select;
        private ToggleGroup group;
        private bool stopEvent;
        private Dictionary<string, Toggle> createdDic = new Dictionary<string, Toggle>();
        public bool AnyToggleOn { get { return group.AnyTogglesOn(); } }
        protected void Awake()
        {
            if (m_Select)
            {
                m_Select.onClick.AddListener(TriggerIDs);
            }

            group = m_parent.GetComponentInChildren<ToggleGroup>();

            if (group == null)
            {
                group = m_parent.gameObject.AddComponent<ToggleGroup>();
            }
        }
        public void SetActiveItem(string key)
        {
            stopEvent = true;

            if (createdDic.ContainsKey(key))
            {
                var tog = createdDic[key];
            tog.isOn = false;
                tog.isOn = true;
            }

            stopEvent = false;
        }
        public void SetSelect(int defultValue)
        {
            if (options.Length <= defultValue) return;
            var key = options[defultValue];
            SetActiveItem(key);
        }
        protected override void OnCreateItem(int id, GameObject instence)
        {
            base.OnCreateItem(id, instence);
            var type = options[id];
            instence.GetComponentInChildren<Text>().text = type;
            var toggle = instence.GetComponentInChildren<Toggle>();
            Debug.Assert(toggle, "预制体或子物体上没有toggle组件");

            if(singleChoise){
                toggle.group = group;
            }

            UnityAction<bool> action = (x) => {
                if(x)
                {
                    if (!stopEvent) Select(id);
                }
                else
                {
                    UnSelect(id);
                }
            };
            toggle.onValueChanged.AddListener(action);
            onResetEvent += () => {
                toggle.onValueChanged.RemoveListener(action);
            };
            createdDic.Add(type, toggle);
        }
        protected override void ClearCreated()
        {
            base.ClearCreated();
            createdDic.Clear();
        }
    }
}
