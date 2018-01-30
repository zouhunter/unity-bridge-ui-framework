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
    /// 错误提示
    /// </summary>
    public class InputPanel : SinglePanel
    {
        public Text title;
        public InputField info;
        public Button closeBtn;
        protected override void Awake()
        {
            base.Awake();
            closeBtn.onClick.AddListener(Close);
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