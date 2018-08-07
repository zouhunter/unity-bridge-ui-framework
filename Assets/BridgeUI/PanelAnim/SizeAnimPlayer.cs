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
    public class SizeAnimPlayer : PanelAnimPlayer
    {
        [SerializeField]
        private float sizeA = 0.8f;
        [SerializeField]
        private float sizeB = 1f;
     
        protected override uTweener CreateTweener(RectTransform rectTrans)
        {
            return uTweenScale.Begin(rectTrans, Vector3.one * sizeA, Vector3.one * sizeB);
        }
    }

}