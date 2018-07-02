using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

using NodeGraph.DataModel;

namespace NodeGraph {

	[CustomEditor(typeof(NodeGraphObj))]
	public class NodeGraphObjDrawer : Editor {
        public static GUIStyle titleStyle;
        public static GUIStyle subTitleStyle;
        public static GUIStyle boldLabelStyle;

        public NodeGraphObj canvas;

        public void OnEnable()
        {
            canvas = (NodeGraphObj)target;
        }

        public override void OnInspectorGUI()
        {
            if (canvas == null)
                canvas = (NodeGraphObj)target;
            if (canvas == null)
                return;
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label);
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.alignment = TextAnchor.MiddleCenter;
                titleStyle.fontSize = 16;
            }
            if (subTitleStyle == null)
            {
                subTitleStyle = new GUIStyle(GUI.skin.label);
                subTitleStyle.fontStyle = FontStyle.Bold;
                subTitleStyle.alignment = TextAnchor.MiddleCenter;
                subTitleStyle.fontSize = 12;
            }
            if (boldLabelStyle == null)
            {
                boldLabelStyle = new GUIStyle(GUI.skin.label);
                boldLabelStyle.fontStyle = FontStyle.Bold;
            }

            EditorGUI.BeginChangeCheck();

            GUILayout.Space(10);

            GUILayout.Label(new GUIContent(canvas.ControllerType, "自己定义控制器类型"), titleStyle);
            GUILayout.Label(canvas.LastModified.ToString("yyyy-MM-dd hh:mm:ss"), subTitleStyle);

            GUILayout.Space(10);

            if (GUILayout.Button("Open",EditorStyles.toolbarButton))
            {
                var window = EditorWindow.GetWindow<NodeGraphWindow>();
                window.OpenGraph(canvas);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Description", GUILayout.Width(100f));
                string newdesc = EditorGUILayout.TextArea(canvas.Descrption, GUILayout.MaxHeight(100f));
                if (newdesc != canvas.Descrption)
                {
                    canvas.Descrption = newdesc;
                }
            }

          

            GUILayout.Space(10);

            if(canvas.Nodes.Count > 0)
            {
                GUILayout.Label("[Nodes]", boldLabelStyle);

                foreach (NodeData node in canvas.Nodes)
                {
                    string label = node.Name;
                    var type = node.Object == null ? typeof(Node) : node.Object.GetType();
                    node.Object = EditorGUILayout.ObjectField(label, node.Object, type, false) as Node;
                }

                GUILayout.Space(10);

            }

            if(canvas.Connections.Count > 0)
            {
                GUILayout.Label("[Connections]", boldLabelStyle);

                foreach (var connection in canvas.Connections)
                {
                    string label = connection.ConnectionType;
                    var type = connection.Object == null ? typeof(Connection) : connection.Object.GetType();
                    EditorGUILayout.ObjectField(label, connection.Object, type, false);
                }

                GUILayout.Space(10);
            }
        

            if (EditorGUI.EndChangeCheck())
            {
                
            }
        }
    }
}
	