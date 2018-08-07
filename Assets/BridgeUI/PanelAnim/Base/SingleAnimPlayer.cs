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
    public abstract class PanelAnimPlayer : AnimPlayer
    {
        [SerializeField]
        protected float animTime = 1;
        [SerializeField]
        protected EaseType easeType = EaseType.easeInBounce;
        protected uTweener tween;

        protected override List<uTweener> CreateTweeners()
        {
            var tweenList = new List<uTweener>();
            var rectTrans = panel.GetComponent<RectTransform>();
            if (rectTrans != null)
            {
                var tweener = CreateTweener(rectTrans);
                tweener.duration = animTime;
                tweener.method = easeType;
                tweenList.Add(tweener);
            }
            return tweenList;
        }

        protected abstract uTweener CreateTweener(RectTransform rectTrans);
    }

}