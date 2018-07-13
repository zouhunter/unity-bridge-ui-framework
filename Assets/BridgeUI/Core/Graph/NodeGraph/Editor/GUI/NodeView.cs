using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using NodeGraph.DataModel;
using UnityEngine.Events;
using System.Reflection;

namespace NodeGraph
{
    public class NodeView
    {
        protected Node _target;
        public virtual Node target { get { return _target; } set { _target = value; } }

        protected Editor targetDrawer;
        public virtual GUIStyle ActiveStyle { get { return EditorStyles.miniButton; } }
        public virtual GUIStyle InactiveStyle { get { return EditorStyles.miniButton; } }
        public virtual string Category { get { return "empty"; } }
        public virtual void OnContextMenuGUI(GenericMenu menu, NodeGUI gui) { }
        public virtual float SuperHeight { get { return 0; } }

        public virtual float SuperWidth { get { return 0; } }

        public virtual void OnNodeGUI(Rect position, NodeData data)
        {
            DrawLabel(position, data);
        }

        protected virtual void DrawLabel(Rect position, NodeData data)
        {
            var oldColor = GUI.color;
            var textColor = (EditorGUIUtility.isProSkin) ? Color.black : oldColor;
            var style = new GUIStyle(EditorStyles.label);
            style.alignment = TextAnchor.MiddleCenter;
            var titleHeight = style.CalcSize(new GUIContent(data.Name)).y + NGEditorSettings.GUI.NODE_TITLE_HEIGHT_MARGIN;
            var nodeTitleRect = new Rect(0, 0, position.width, titleHeight);
            GUI.color = textColor;
            GUI.Label(nodeTitleRect, data.Name, style);
            GUI.color = oldColor;
        }

        public virtual void OnInspectorGUI(NodeGUI gui)
        {
            if (target == null) return;

            if (targetDrawer == null)
                targetDrawer = Editor.CreateEditor(target);

            targetDrawer.DrawHeader();
            targetDrawer.OnInspectorGUI();
        }

        public virtual void OnClickNodeGUI(NodeGUI nodeGUI, Vector2 mousePosition, ConnectionPointData result) { }

        protected void RecordUnDo(string message, NodeGUI node, bool saveOnScopeEnd, UnityAction action)
        {
            using (new RecordUndoScope("Change Node Name", node, saveOnScopeEnd))
            {
                action.Invoke();
            }
        }
    }
}

