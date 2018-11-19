using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ProgressTrigger : MonoBehaviour {
    public float time = 2;
    public Text progress;
    public Slider slider;
    public UnityEvent onComplete;

    private void OnEnable()
    {
        StartCoroutine(DelayPlaying());
    }

    IEnumerator DelayPlaying()
    {
        for (float i = 0; i < time; i+= Time.deltaTime)
        {
            yield return null;
            slider.value = (slider.maxValue - slider.minValue) * i / time + slider.minValue;
            if(progress!=null)
            {
                progress.text = string.Format("{0}%", (int)(slider.value * 100));
            }
        }
        onComplete.Invoke();
    }
}
