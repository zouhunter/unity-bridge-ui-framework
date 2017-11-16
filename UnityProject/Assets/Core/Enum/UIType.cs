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


public enum UIFormType
{
    Fixed,//固定窗口(只能打开单个)
    DragAble//可拖拽(可以打开多个小窗体)
}

public enum UILayerType
{
    Bottom = 1,
    Middle = 2,
    Top = 3
}

/// <summary>
/// UI窗体透明度类型
/// </summary>
public enum UILucenyType
{
    //完全透明，不能穿透
    Lucency,
    //半透明，不能穿透
    Translucence,
    //低透明度，不能穿透
    ImPenetrable,
    //可以穿透
    Pentrate
}

[System.Serializable]
public class UIType
{
    //位置
    public UIFormType form = UIFormType.Fixed;
    //层级
    public UILayerType layer = UILayerType.Bottom;
    //透明度
    public UILucenyType luceny = UILucenyType.Lucency;

}