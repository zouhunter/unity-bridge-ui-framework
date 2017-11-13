using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using uTween;

public class UIPanelTween : MonoBehaviour
{
    public enum TweenType
    {
        ScalePanel,
        PosUpPanel,
        RotatePanel,
    }

    public TweenType type;
    private float duration = 0.2f;
    private uTweener tween;
    void Start()
    {
        var panel = GetComponent<RectTransform>();
        switch (type)
        {
            case TweenType.ScalePanel:
                tween = uTweenScale.Begin(panel, Vector3.one * 0.8f, panel.localScale, duration);
                break;
            case TweenType.PosUpPanel:
                tween = uTweenPosition.Begin(panel, Vector3.left * 200, Vector3.zero, duration);
                break;
            case TweenType.RotatePanel:
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
