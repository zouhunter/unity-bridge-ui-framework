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
    public class ConnectionDrawer
    {
        public DataModel.Connection target;
        protected SerializedObject serializedObj;
        internal virtual int LineWidth { get { return 3; } }
        internal virtual Color LineColor { get { return Color.white; } }

        internal virtual void OnDrawLabel(Vector3 centerPos, string label)
        {
            GUIStyle labelStyle = new GUIStyle("WhiteMiniLabel");
            labelStyle.alignment = TextAnchor.MiddleLeft;
            var labelWidth = labelStyle.CalcSize(new GUIContent(label));
            var labelPointV3 = new Vector3(centerPos.x - (labelWidth.x / 2), centerPos.y - 24f, 0f);
            Handles.Label(labelPointV3, label, labelStyle);
        }

        internal virtual void OnConnectionGUI(Vector3 startV3, Vector3 endV3, Vector3 startTan, Vector3 endTan) { }

        internal virtual void OnInspectorGUI()
        {
            if (target == null) return;
            if (serializedObj == null)
                serializedObj = new SerializedObject(target);
            EditorGUILayout.HelpBox("[默认绘制:]", MessageType.Info);
            UserDefineUtility.DrawSerializedObject(serializedObj);
        }

        internal virtual void OnContextMenuGUI(GenericMenu menu, ConnectionGUI connectionGUI) { }
    }

}