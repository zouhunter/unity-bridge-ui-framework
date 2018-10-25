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
    [System.Serializable]
    public class ButtonListSelector : ListSelector
    {
        public override bool singleChoise
        {
            get
            {
                return true;
            }

            set
            {
                base.singleChoise = value;
            }
        }
     
        protected override void OnCreateItem(int id,GameObject instence)
        {
            base.OnCreateItem(id,instence);
            var type = options[id];
            instence.GetComponentInChildren<Text>().text = type;
            var button = instence.GetComponentInChildren<Button>();
            UnityAction action = () => {
                Select(id);
            };

            button.onClick.AddListener(action);

            onResetEvent += () =>
            {
                button.onClick.RemoveListener(action);
            };
        }
    }
}
