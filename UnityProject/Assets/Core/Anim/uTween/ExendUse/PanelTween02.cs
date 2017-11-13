using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using uTween;

public class PanelTween02 : MonoBehaviour {

    public enum TweenType{
        WorningPanel,
    }

    public TweenType type;
    public RectTransform panel;
    private float duration = 0.2f;
    private uTweener tween;
    private UnityAction<bool> playEvent;
    private bool targetBool;
    void Start()
    {
        switch (type)
        {
            case TweenType.WorningPanel:
                tween = uTweenColor.Begin(panel,Color.white,Color.red,duration);/*.Begin(panel, Vector3.one * 0.8f, panel.localScale , duration);*/
                break;
            default:
                break;
        }
        tween.AddOnFinished(OnFinish);

        if (playEvent != null)
        {
            playEvent(targetBool);
            playEvent = null;
        }
    }

    void Update()
    {
        tween.Update();
    }

    void OnFinish()
    {
        if (tween.direction == Direction.Forward)
        {
            tween.ResetToBeginning();
        }
    }

    public void TogglePlay(bool isOn)
    {
        if (tween == null)
        {
            targetBool = isOn;
            playEvent = TogglePlay;
            return;
        }

        if(isOn)
        {
            tween.PlayForward();
        }
        else
        {

            tween.PlayReverse();
        }
    }

    public void PlayForward()
    {
        TogglePlay(true);
    }

    public void PlayBackward()
    {
        TogglePlay(false);
    }
}
