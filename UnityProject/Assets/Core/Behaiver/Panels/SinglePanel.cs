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
using System;
namespace BridgeUI
{
    /// <summary>
    /// 单一面板[不包含容器]
    /// </summary>
    public class SinglePanel : PanelBase
    {
        public override Transform Content { get { return Root; } }
    }

    public class SingleCloseAblePanel : SinglePanel
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