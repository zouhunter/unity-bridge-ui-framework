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
using System;

public class Panel01 :  PanelBase,IPointerClickHandler{
   
    public void OnPointerClick(PointerEventData eventData)
    {
        CallBack("panel01 发送的回调");
    }
}
