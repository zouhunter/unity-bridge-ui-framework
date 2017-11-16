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

public enum HideRule  {
    HideChildObject,//隐藏自己的可见物体
    HideGameObject,//直接隐藏自己
    MoveToPoint,//向空间移动
}
