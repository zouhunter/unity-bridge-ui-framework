using UnityEngine;
using UnityEditor;
using System;

namespace NodeGraph
{
    [CustomEditor(typeof(ConnectionGUIInspectorHelper))]
    public class ConnectionGUIEditor : Editor
    {
        protected override void OnHeaderGUI()
        {
        }
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

            con.DrawObject();
        }

        private bool DrawToggle(bool on, string tip)
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                 on = GUILayout.Toggle(on, tip, EditorStyles.radioButton, GUILayout.Height(60),GUILayout.Width(100));
                EditorGUILayout.LabelField(tip);
            }
            return on;
        }

        private Enum DrawEnum(Enum em, string tip) 
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                em = EditorGUILayout.EnumPopup(em, EditorStyles.toolbarDropDown, GUILayout.Height(60), GUILayout.Width(100));
                EditorGUILayout.LabelField(tip);
            }
            return em;
        }


    }
}