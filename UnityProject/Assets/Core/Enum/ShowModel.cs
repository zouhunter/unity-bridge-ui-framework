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

/// <summary>
/// 界面的显示状态
/// </summary>
public enum ShowModel
{
    Auto = 1,//当上级显示时显示
    Mutex = 1 << 1,//排斥有相同类型面版
    HideBase = 1 << 3,//将父级面板隐去
    Single = 1 << 4,//隐藏所有打开的面板
}
