using BridgeUI;
using BridgeUI.uTween;

using UnityEngine;

public class PopAnimPlayer : AnimPlayer
{
    private float duration = 5;
    private RectTransform panel;
    public void Awake()
    {
        panel = transform.GetChild(0) as RectTransform;
    }
    protected override uTweener CreateTweener(bool isEnter)
    {
        tween = uTweenScale.Begin(panel, Vector3.one * 0.8f, Vector3.one, duration);
        return tween;
        //tween = uTweenScale.Begin(panel, Vector3.one * 0.8f, Vector3.one, duration);
        //tween = uTweenPosition.Begin(panel, Vector3.left * 200, Vector3.zero, duration);
        //tween = uTweenRotation.Begin(panel, Vector3.up * 30, Vector3.zero, duration);
    }
}
