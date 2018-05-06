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
    public class ButtonListSelector : MonoBehaviour
    {
        [SerializeField]
        private Transform m_parent;
        [SerializeField]
        private Button m_prefab;
        private string selected;
        private UnityAction<string> onChoise;
        private Dictionary<string, Button> createdDic;
        private string[] options;
        private GameObjectPool gameObjectPool { get { return UIFacade.PanelPool; } }

        public void OpenSelect(string[] selectables, UnityAction<string> onChoise)
        {
            this.options = selectables;
            this.onChoise = onChoise;
            CreateUIList(selectables);
        }


        public void OpenSelect(string[] selectables, UnityAction<int> onChoise)
        {
            this.options = selectables;
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
        void TrySelectItem()
        {
            if (this.onChoise != null){
                this.onChoise.Invoke(selected);
            }

        }
        void OnSelect(string type)
        {
            selected = type;
            TrySelectItem();
        }

        void CreateUIList(string[] selectables)
        {
            if (createdDic == null)
            {
                createdDic = new Dictionary<string, Button>();
            }
            else
            {
                foreach (var item in createdDic)
                {
                    var button = item.Value;
                    button.onClick.RemoveAllListeners();
                    gameObjectPool.SavePoolObject(item.Value.gameObject, false);
                }
                createdDic.Clear();
            }

            for (int i = 0; i < selectables.Length; i++)
            {
                var item = gameObjectPool.GetPoolObject(m_prefab.gameObject, m_parent, false);
                var type = selectables[i];
                item.GetComponentInChildren<Text>().text = type;
                var button = item.GetComponentInChildren<Button>();
                Debug.Assert(button, "预制体或子物体上没有button组件");
                button.onClick.AddListener(() => {
                        OnSelect(type);
                });
                createdDic.Add(type, button);
            }
        }
    }
}
