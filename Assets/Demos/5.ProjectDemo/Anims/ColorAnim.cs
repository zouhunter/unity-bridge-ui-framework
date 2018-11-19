using BridgeUI;
using BridgeUI.uTween;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class ColorAnim : AnimPlayer
{
    [SerializeField]
    private float dely = 1;
    [SerializeField]
    private float duration = 2;
    [SerializeField]
    private Color startColor = Color.red;
    [SerializeField]
    private Color endColor = Color.white;

    protected override List<uTweener> CreateTweeners()
    {
        var tweens = new List<uTweener>();
        var colorTween = uTweenColor.Begin(panel.transform as RectTransform, startColor, Color.white, duration, dely);
        colorTween.includeChildren = true;
        tweens.Add(colorTween);
        return tweens;
    }
}
