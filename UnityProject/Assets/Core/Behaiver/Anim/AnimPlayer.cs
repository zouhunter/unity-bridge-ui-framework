using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI.uTween;
namespace BridgeUI
{
    public class AnimPlayer : MonoBehaviour, IAnimPlayer
    {
        private float duration = 0.2f;
        private uTweener tween;
        private RectTransform panel;
        void Awake()
        {
            panel = GetComponent<RectTransform>();
        }

        void Update()
        {
           if(tween != null)
                tween.Update();
        }

        public void EnterAnim(UIAnimType animType,UnityAction onComplete)
        {
            ResetAnim(animType);
            if (onComplete != null){
                tween.AddOnFinished(onComplete);
            }
            tween.ResetToBeginning();
            tween.PlayForward();
        }

        public void QuitAnim(UIAnimType animType, UnityAction onComplete)
        {
            ResetAnim(animType);
            if (onComplete != null)
                tween.AddOnFinished(onComplete);
            tween.ResetToComplete();
            tween.PlayReverse();
        }

        private void ResetAnim(UIAnimType animType)
        {
            switch (animType)
            {
                case UIAnimType.ScalePanel:
                    tween = uTweenScale.Begin(panel, Vector3.one * 0.8f, Vector3.one, duration);
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
        }
    }
}
