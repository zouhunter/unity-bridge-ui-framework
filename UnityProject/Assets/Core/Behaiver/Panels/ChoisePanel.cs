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
/// <summary>
/// 弹窗面板
/// [动态弹出，用于显示信息或记录信息，弹出排队]
/// </summary>
public class ChoisePanel :PanelBase
{
    public override Transform Content { get { return transform; } }
}
