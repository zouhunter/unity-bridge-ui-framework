using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace BridgeUI
{
    [System.Serializable]
    public class RangeAttribute: PropertyAttribute
    {
        public float min;
        public float max;
        public RangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}