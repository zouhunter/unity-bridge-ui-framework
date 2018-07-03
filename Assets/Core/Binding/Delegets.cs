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
    public delegate void BindHandler(IBindingContext source);
    public delegate void UnBindHandler(IBindingContext source);

    public delegate void PanelAction<T>(IBindingContext panel,T arg0);
    public delegate void PanelAction<T,S>(IBindingContext panel,T arg0,S arg1);
    public delegate void PanelAction<T,S,R>(IBindingContext panel,T arg0,S arg1,R arg2);
    public delegate void PanelAction<T,S,R,Q>(IBindingContext panel,T arg0,S arg1,R arg2,Q arg3);
    public delegate void PanelAction(IBindingContext panel);
}