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

public enum CloseRule  {
    DestroyImmediate,//快速销毁并从内存中清除
    DestroyDely,//延迟销毁
    DestroyWithAnim,//播放销毁动画
}
