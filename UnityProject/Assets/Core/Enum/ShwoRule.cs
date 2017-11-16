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

public enum ShowRule
{
    Single = 1,//只打开唯一
    HideParent = 1 << 1,//隐藏父级开关
    Mask = 1<<2,//建立遮罩
}
