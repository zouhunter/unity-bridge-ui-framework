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
/// 所有ui界面的父级
/// [用于界面创建及打开的规则]
/// </summary>
public interface IPanelBase {
    int InstenceID { get; }
}
