using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;


namespace BridgeUI.Common
{
    public class CloseAbleSelectablePanel : SelectAblesPanel
    {
        [SerializeField]
        protected Button m_close;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            m_close.onClick.AddListener(Close);
        }
        protected override void OnRecover()
        {
            base.OnRecover();
            m_close.onClick.RemoveListener(Close);
        }
    }
}