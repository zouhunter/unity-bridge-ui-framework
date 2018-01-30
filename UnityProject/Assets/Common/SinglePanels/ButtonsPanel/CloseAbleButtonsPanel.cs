using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
namespace BridgeUI.Common
{
    public class CloseAbleButtonsPanel : ButtonsPanel
    {
        [SerializeField]
        private Button m_close;

        protected override void Awake()
        {
            base.Awake();
            m_close.onClick.AddListener(Close);
        }
    }
}