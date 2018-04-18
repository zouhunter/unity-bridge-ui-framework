using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
namespace BridgeUI.Common.Tree
{
    public class ColumnTreeItemCreater 
    {
        private ColumnTreeRule rule;
        private Transform m_parent { get { return rule.parent; } }
        private ColumnItem m_prefab { get { return rule.prefab; } }
        private string selected;
        private Dictionary<string, ColumnItem> createdDic;
        private ToggleGroup group;
        private string[] options;
        private bool stopEvent;
        private GameObjectPool gameObjectPool;


        public bool AnyToggleOn { get { return group.AnyTogglesOn(); } }
        public string Selected { get { return selected; } }
        public int SelectedID { get { if(!string.IsNullOrEmpty(selected))return System.Array.IndexOf(options, selected); return -1; } }
        public UnityAction<string> onChoise { get; set; }
        public UnityAction<int> onChoiseID { get; set; }
        

        public ColumnTreeItemCreater(ColumnTreeRule rule)
        {
            this.rule = rule;
            InitEnviroment();
        }

        public void OpenSelect(string[] selectables)
        {
            this.options = selectables;
            this.selected = null;
            CreateUIList(selectables);
        }

        public void SetActiveItem(string key,bool dispatchEvent = false)
        {
            stopEvent = !dispatchEvent;
            if (createdDic.ContainsKey(key))
            {
                selected = key;
                createdDic[key].Tog.isOn = false;//如果只设置为true不会触发事件
                createdDic[key].Tog.isOn = true;
            }

            stopEvent = false;
        }
        public void SetActiveItem(int id, bool dispatchEvent = false)
        {
            stopEvent = !dispatchEvent;
            string key = null;
            if(options.Length > id)
            {
                key = options[id];
            }
            if (!string.IsNullOrEmpty(key) && createdDic.ContainsKey(key))
            {
                selected = key;
                createdDic[key].Tog.isOn = true;
            }

            stopEvent = false;
        }
        /// <summary>
        /// 初始化环境
        /// </summary>
        private void InitEnviroment()
        {
            gameObjectPool = UIFacade.PanelPool;
            group = m_parent.GetComponentInChildren<ToggleGroup>();
            if (group == null)
                group = m_parent.gameObject.AddComponent<ToggleGroup>();
        }
        /// <summary>
        /// 选中回调
        /// </summary>
        /// <param name="type"></param>
        private void OnSelect(string type)
        {
            selected = type;
            if (this.onChoise != null)
            {
                this.onChoise.Invoke(selected);
            }
            if(onChoiseID != null)
            {
                var id = System.Array.IndexOf(options, selected);
                onChoiseID.Invoke(id);
            }
        }

        /// <summary>
        /// 创建列表
        /// </summary>
        /// <param name="selectables"></param>
        private void CreateUIList(string[] selectables)
        {
            if (createdDic == null)
            {
                createdDic = new Dictionary<string, ColumnItem>();
            }
            else
            {
                foreach (var item in createdDic)
                {
                    var toggle = item.Value;
                    toggle.Reset();
                    gameObjectPool.SavePoolObject(item.Value.gameObject, false);
                }
                createdDic.Clear();
            }

            for (int i = 0; i < selectables.Length; i++)
            {
                var go = gameObjectPool.GetPoolObject(m_prefab.gameObject, m_parent, false);
                var item = go.GetComponent<ColumnItem>();
                var type = selectables[i];
                item.Init(type, group, (x) => { if (!stopEvent && x) OnSelect(type); });
                createdDic.Add(type, item);
            }
        }

    }
}
