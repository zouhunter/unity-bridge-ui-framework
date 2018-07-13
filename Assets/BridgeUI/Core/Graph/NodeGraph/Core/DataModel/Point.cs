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

namespace NodeGraph.DataModel
{
    [System.Serializable]
    public struct Point
    {
        public string label;
        public string type;
        public int max;
        public Point(string label, string type, int max = 1)
        {
            this.label = label;
            this.type = type;
            this.max = max;
        }
    }
}