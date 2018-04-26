using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;

public class ButtonList : MonoBehaviour {
    public UnityEvent<string> onSelectChanged;
    public string[] infos;

    public float floatValue;
    public int intValue;
    public string stringValue;
    public double doubleValue;
    public bool boolValue;

    public void SetList(string[] infos)
    {
    }
}
