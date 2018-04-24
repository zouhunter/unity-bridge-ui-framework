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
#if xLua
    [XLua.CSharpCallLua]
#endif
    public delegate void ButtonEvent(PanelBase panel, Button btn,params object[] args);
#if xLua
    [XLua.CSharpCallLua]
#endif
    public delegate void ToggleEvent(PanelBase panel, Toggle btn,params object[] args);
#if xLua
    [XLua.CSharpCallLua]
#endif
    public delegate void SliderEvent(PanelBase panel, Slider btn,params object[] args);
#if xLua
    [XLua.CSharpCallLua]
#endif
    public delegate void InputFieldEvent(PanelBase panel, InputField btn,params object[] args);
#if xLua
    [XLua.CSharpCallLua]
#endif
    public delegate void DropdownEvent(PanelBase panel, Dropdown dropdown, params object[] args);
}
