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
using BridgeUI.Model;

namespace BridgeUI
{
    /// <summary>
    /// 所有ui界面的父级
    /// [用于界面创建及打开的规则]
    /// </summary>
    public interface IPanelBaseInternal : IPanelBase
    {
        string Name { get; }
        int InstenceID { get; }
        IPanelGroup Group { get; set; }
        Transform Content { get; }
        List<IPanelBaseInternal> ChildPanels { get; }
        event UnityAction<IPanelBaseInternal> onDelete;
        UIType UType { get; set; }
        bool IsShowing { get; }
        bool IsAlive { get; }
        void SetParent(Transform parent);

        void RecordChild(IPanelBaseInternal childPanel);

        void HandleData(Bridge bridge);
    }

    public interface IPanelBase
    {
        void Close();
        void Hide();
        void UnHide();
    }
}