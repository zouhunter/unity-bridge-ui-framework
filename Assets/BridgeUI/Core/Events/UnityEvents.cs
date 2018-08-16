using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace BridgeUI
{
 
    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }
    [System.Serializable]
    public class IntArrayEvent : UnityEvent<int[]> { }
    [System.Serializable]
    public class IntListEvent : UnityEvent<List<int>> { }

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    [System.Serializable]
    public class BoolArrayEvent : UnityEvent<bool[]> { }
    [System.Serializable]
    public class BoolListEvent : UnityEvent<List<bool>> { }


    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }
    [System.Serializable]
    public class FloatArrayEvent : UnityEvent<float[]> { }
    [System.Serializable]
    public class FloatListEvent : UnityEvent<List<float>> { }


    [System.Serializable]
    public class StringEvent : UnityEvent<string> { }
    [System.Serializable]
    public class StringArrayEvent : UnityEvent<string[]> { }
    [System.Serializable]
    public class StringListEvent : UnityEvent<List<string>> { }
}