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
    Normal = 1,//普通演示不对其他对象造成影响
    Mask = 1 << 1,//建立遮罩防止点击其他对象
    HideParent = 1 << 2,//将父级面板隐去
    TypeSingle = 1 << 3,//该类型中唯一显示
    CanvasSingle = 1 << 4,//整个界面上唯一显示
    WorldSingle = 1 << 5//整个场景界面唯一显示
}
