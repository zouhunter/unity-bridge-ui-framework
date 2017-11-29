using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI.uTween;
namespace BridgeUI
{
    public class UIPanelTween : MonoBehaviour
    {
        public UIAnimType type;
        private float duration = 0.2f;
        private uTweener tween;
        void Start()
        {
            var panel = GetComponent<RectTransform>();
            switch (type)
            {
                case UIAnimType.ScalePanel:
                    tween = uTweenScale.Begin(panel, Vector3.one * 0.8f, panel.localScale, duration);
                    break;
                case UIAnimType.PosUpPanel:
                    tween = uTweenPosition.Begin(panel, Vector3.left * 200, Vector3.zero, duration);
                    break;
                case UIAnimType.RotatePanel:
                    tween = uTweenRotation.Begin(panel, Vector3.up * 30, Vector3.zero, duration);
                    break;
                default:
                    break;
            }
            Play();
        }

        void Update()
        {
            tween.Update();
        }

        void Play()
        {
            tween.PlayForward();
        }

    }
}
