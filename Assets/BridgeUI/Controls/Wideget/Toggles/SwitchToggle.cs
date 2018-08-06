using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
namespace BridgeUI.Control
{
    /// <summary>
    /// 控制Toggle背景图片的打开与关闭
    /// </summary>
    public class SwitchToggle : Toggle
    {
        protected override void Awake()
        {
            base.Awake();
            onValueChanged.AddListener(SwitchImage);
        }
        private void SwitchImage(bool arg0)
        {
            targetGraphic.enabled = !arg0;
        }
    }

}