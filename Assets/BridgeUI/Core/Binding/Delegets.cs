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

namespace BridgeUI.Binding
{
    public delegate void ValueChangedHandler1<T>(T newValue);
    public delegate void ValueChangedHandler2<T>(T oldValue,T newValue);

    public delegate void PanelAction(IUIPanel panel);
    public delegate void PanelAction<T>(IUIPanel panel,T arg0);
    public delegate void PanelAction<T,S>(IUIPanel panel,T arg0,S arg1);
    public delegate void PanelAction<T,S,R>(IUIPanel panel,T arg0,S arg1,R arg2);
    public delegate void PanelAction<T,S,R,Q>(IUIPanel panel,T arg0,S arg1,R arg2,Q arg3);
    public delegate void PanelAction<T,S,R,Q,P>(IUIPanel panel, T arg0,S arg1,R arg2,Q arg3,P arg4);
}