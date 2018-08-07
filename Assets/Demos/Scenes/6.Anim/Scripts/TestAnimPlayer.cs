using BridgeUI;
using BridgeUI.uTween;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TestAnimPlayer : AnimPlayer
{
    [SerializeField]
    private float duration = 5;
    RectTransform childPanel;
    private Image image;
    public override void SetContext(MonoBehaviour context)
    {
        base.SetContext(context);
        childPanel = context.transform.GetChild(0) as RectTransform;
        image = childPanel.GetComponent<Image>();
    }
    protected override List<uTweener> CreateTweeners()
    {
        var tweens = new List<uTweener>();
        tweens.Add(uTweenScale.Begin(childPanel, Vector3.one * 0.8f, Vector3.one, duration));
        tweens.Add(uTweenColor.Begin(childPanel, Color.red, Color.green, duration, 1));
        tweens.Add(uTweenColor.Begin(image.rectTransform, Color.blue, Color.yellow, duration, 1));
        return tweens;
    }
}
