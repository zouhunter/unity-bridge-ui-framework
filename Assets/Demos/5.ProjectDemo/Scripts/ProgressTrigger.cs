using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ProgressTrigger : MonoBehaviour {
    public bool playOnAwake;
    public float time = 2;
    public Slider slider;
    public UnityEvent onComplete;

    private void Awake()
    {
        if(playOnAwake)
        {
            Play();
        }
    }

    public void Play()
    {
        StartCoroutine(DelayPlaying());
    }

    IEnumerator DelayPlaying()
    {
        for (float i = 0; i < time; i+= Time.deltaTime)
        {
            yield return null;
            slider.value = (slider.maxValue - slider.minValue) * i / time + slider.minValue;
        }
        onComplete.Invoke();
    }
}
