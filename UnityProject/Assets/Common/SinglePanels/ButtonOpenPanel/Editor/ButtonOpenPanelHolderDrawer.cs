using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using UnityEditorInternal;

namespace BridgeUI.Common
{
    [CustomPropertyDrawer(typeof(ButtonOpenPanel.Holder))]
    public class HolderDrawer : PropertyDrawer
    {
        string[] panelNames;
        int selected = 0;

        public HolderDrawer()
        {
            var fields = typeof(PanelNames).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty);
            panelNames = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                panelNames[i] = fields[i].Name;
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            else
            {
                return 3 * EditorGUIUtility.singleLineHeight;
            }
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var panelName = property.FindPropertyRelative("panelName");
            var button = property.FindPropertyRelative("button");
            var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            if (GUI.Button(rect, panelName.stringValue, EditorStyles.toolbarButton))
            {
                property.isExpanded = !property.isExpanded;
            }

            if (property.isExpanded)
            {
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, button);
                rect.y += EditorGUIUtility.singleLineHeight;
                selected = System.Array.IndexOf(panelNames, panelName.stringValue);
                if (selected < 0) selected = 0;
                selected = EditorGUI.Popup(rect, selected, panelNames);
                panelName.stringValue = panelNames[selected];
            }
        }
    }
#endif
}