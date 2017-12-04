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

public enum NodeType{
    Fixed = 1,//可自定义窗体结构
    ZeroLayer = 2,//可自定义层级
    HideGO = 4,//可自定义隐藏显示
    Destroy = 8,//可自定义销毁方式
    NoAnim = 16//可自定义动画效果
}
