using UnityEngine;
using UnityEditor;
using System;

namespace NodeGraph
{
    [CustomEditor(typeof(ConnectionGUIInspectorHelper))]
    public class ConnectionGUIEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            ConnectionGUIInspectorHelper helper = target as ConnectionGUIInspectorHelper;
            var con = helper.connectionGUI;
            if (con == null) {
                return;
            }

            EditorGUILayout.HelpBox("连接信息:", MessageType.Info);
            con.DrawObject();
        }
    }
}