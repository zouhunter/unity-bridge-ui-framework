using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
using BridgeUI.uTween;
using System;
namespace BridgeUI.PanelAnim
{
    public class RotAnimPlayer : PanelAnimPlayer
    {
        [SerializeField]
        private float angleA = 500f;
        [SerializeField]
        private float angleB = 0f;
     
        protected override uTweener CreateTweener(RectTransform rectTrans)
        {
            return uTweenRotation.Begin(rectTrans,Vector3.forward * angleA, Vector3.forward * angleB);
        }
    }

}