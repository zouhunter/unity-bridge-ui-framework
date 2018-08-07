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
using NodeGraph;
using UnityEditor;
using NodeGraph.DefultSkin;

namespace BridgeUIEditor
{
    [CustomNodeView(typeof(AnyNode))]
    public class AnyNodeView : DefultSkinNodeView
    {
        public override float SuperHeight
        {
            get
            {
                return -EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnInspectorGUI(NodeGUI gui)
        {
            base.OnInspectorGUI(gui);
            if (target != null)
            {
                gui.Name = "AnyState";
            }
        }
    }

}