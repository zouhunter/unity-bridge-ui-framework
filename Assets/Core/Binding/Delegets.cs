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
#if xLua
    [XLua.CSharpCallLua]
#endif
    public delegate void CallBack<T>(IBindingContext panel,T sender);
    public delegate void PanelAction(IBindingContext panel);
}