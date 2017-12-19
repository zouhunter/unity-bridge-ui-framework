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
using UnityEditor;
using System;

namespace NodeGraph
{
    public class ConnectionDrawer: DefultDrawer
    {
        public DataModel.Connection target;
        internal virtual int LineWidth { get { return 3; } }
        internal virtual Color LineColor { get { return Color.white; } }
        internal virtual string Label { get { return ""; } }
        internal virtual void OnInspectorGUI()
        {
            base.OnInspectorGUI(target);
        }
    }

}