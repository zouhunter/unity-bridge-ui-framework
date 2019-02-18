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
    public interface IChildPanelOpenClose
    {
        #region OpenClose
        void Open(string panelName, object data = null);
        void Open(int index, object data = null);
        void Hide(string panelName);
        void Hide(int index);
        void Close(string panelName);
        void Close(int index);
        bool IsOpen(int index);
        bool IsOpen(string panelName);
        #endregion
    }
    /// <summary>
    /// 所有ui界面的父级
    /// [用于界面创建及打开的规则]
    /// </summary>

    public interface IUIPanel: IChildPanelOpenClose
    {
        GameObject Target { get; }
        string Name { get; }
        int InstenceID { get; }
        IPanelGroup Group { get; set; }
        IUIPanel Parent { get; set; }
        Transform Content { get; }
        Transform Root { get; }
        List<IUIPanel> ChildPanels { get; }
        event PanelCloseEvent onClose;
        UIType UType { get; set; }
        bool IsShowing { get; }
        bool IsAlive { get; }
        void Binding(GameObject target);
        void SetParent(Transform Trans, Dictionary<int, Transform> transDic, Dictionary<int, IUIPanel> transRefDic);
        void CallBack(object data);
        void Close();
        void Hide();
        void UnHide();
        void RecordChild(IUIPanel childPanel);
        void HandleData(Bridge bridge);
    }


}