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
    public class FlyAnimPlayer : PanelAnimPlayer
    {
        [SerializeField]
        private Vector3 posA = Vector3.left * 100; 
        [SerializeField]
        private Vector3 posB = Vector3.zero;
     
        protected override uTweener CreateTweener(RectTransform rectTrans)
        {
            return uTweenPosition.Begin(rectTrans, posA, posB);
        }
    }

}