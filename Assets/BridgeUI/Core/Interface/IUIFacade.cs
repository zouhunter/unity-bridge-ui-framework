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
        IUIHandle Open(string panelName,  object data = null);
        IUIHandle Open(IPanelBase parentPanel, string panelName,  object data = null);
        bool IsPanelOpen(string panelName);
        bool IsPanelOpen(IPanelGroup parentGroup, string panelName);
        void Hide(string panelName);
        void Hide(IPanelGroup parentGroup, string panelName);
        void Close(string panelName);
        void Close(IPanelGroup parentGroup,string panelName);
    }
}