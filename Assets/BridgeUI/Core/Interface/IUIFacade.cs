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
    public interface IUIFacade
    {
        void Open(string panelName, object data = null);
        void Open(IUIPanel parentPanel, string panelName, object data = null);
        void Open(string panelName, IPanelVisitor uiData);
        void Open(IUIPanel parentPanel, string panelName, IPanelVisitor uiData);
        bool IsPanelOpen(string panelName);
        bool IsPanelOpen<T>(string panelName, out T[] panels);
        bool IsPanelOpen(IPanelGroup parentGroup, string panelName);
        bool IsPanelOpen<T>(IPanelGroup parentPanel, string panelName, out T[] panels);
        void Hide(string panelName);
        void Hide(IPanelGroup parentGroup, string panelName);
        void Close(string panelName);
        void Close(IPanelGroup parentGroup,string panelName);
        void RegistCreate(UnityAction<IUIPanel> onCreate);
        void RegistClose(UnityAction<IUIPanel> onClose);
        void RemoveCreate(UnityAction<IUIPanel> onCreate);
        void RemoveClose(UnityAction<IUIPanel> onClose);
    }
}