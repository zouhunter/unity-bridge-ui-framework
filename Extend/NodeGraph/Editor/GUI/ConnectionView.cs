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
    public class ConnectionView
    {
        public DataModel.Connection target;
        protected Editor targetDrawer;

        internal virtual int LineWidth { get { return 3; } }
        internal virtual Color LineColor { get { return Color.white; } }

        internal virtual void OnDrawLabel(Vector2 centerPos, string label)
        {
            GUIStyle labelStyle = new GUIStyle("WhiteMiniLabel");
            labelStyle.alignment = TextAnchor.MiddleLeft;
            var labelWidth = labelStyle.CalcSize(new GUIContent(label));
            var labelPointV3 = new Vector2(centerPos.x - (labelWidth.x / 2), centerPos.y - 24f);
            Handles.Label(labelPointV3, label, labelStyle);
        }

        internal virtual void OnConnectionGUI(Vector2 startV3, Vector2 endV3, Vector2 startTan, Vector2 endTan) { }

        internal virtual void OnInspectorGUI()
        {
            if (target == null) return;

            if (targetDrawer == null)
                targetDrawer = Editor.CreateEditor(target);

            targetDrawer.DrawHeader();
            targetDrawer.OnInspectorGUI();
        }

        internal virtual void OnContextMenuGUI(GenericMenu menu, ConnectionGUI connectionGUI) { }
    }

}