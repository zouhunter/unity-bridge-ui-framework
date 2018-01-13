using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace BridgeUI
{
    public class SingleCloseAblePanel : SinglePanel
    {
        [SerializeField]
        private Button m_close;
        protected override void Awake()
        {
            base.Awake();
            if (m_close != null) m_close.onClick.AddListener(Close);
        }
    }
}