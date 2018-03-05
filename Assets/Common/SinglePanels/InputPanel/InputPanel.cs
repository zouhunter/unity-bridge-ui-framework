#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 01:13:36
    * 说    明：       1.接收信息输入
                       2.实时返回编辑的信息
* ************************************************************************************/
#endregion
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;

namespace BridgeUI.Common
{
    /// <summary>
    /// 输入面板
    /// </summary>
    public class InputPanel : SingleCloseAblePanel
    {
        public Text title;
        public InputField info;
        protected override void Awake()
        {
            base.Awake();
            info.onEndEdit.AddListener((x)=> {
                CallBack(x);
            });
        }
        protected override void HandleData(object message)
        {
            var dic = message as Hashtable;
            if (message != null && dic["title"] != null)
            {
                title.text = dic["title"] as string;
            }
            else
            {
                title.text = "请输入";
            }
        }
    }
}