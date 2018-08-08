
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

namespace BridgeUI.Drawer
{
    [CustomNodeView(typeof(Graph.RemoteNode))]
    public class RemoteNodeView : DefultSkinNodeView
    {
        public override string Category
        {
            get
            {
                return "remote";
            }
        }
        public override void OnInspectorGUI(NodeGUI gui)
        {
            base.OnInspectorGUI(gui);
            gui.Name = EditorGUILayout.TextField("Name", gui.Name);
        }
    }

}