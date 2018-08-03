using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace BridgeUI.Binding
{
    [System.Serializable]
    public class IntArrayEvent : UnityEvent<int[]> { }
    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }
}