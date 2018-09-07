using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI.Trigger;
using UnityEditor;

namespace BridgeUIEditor
{
    [CustomEditor(typeof(OpenCloseBehaiver))]
    public class OpenCloseBehaiverDrawer : Editor
    {
        private SerializedProperty prop_index;
        private SerializedProperty prop_panelName;
        private SerializedProperty prop_fromindex;

        private void OnEnable()
        {
            prop_fromindex = serializedObject.FindProperty("fromIndex");
            prop_index = serializedObject.FindProperty("index");
            prop_panelName = serializedObject.FindProperty("panelName");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            if(prop_fromindex.boolValue)
            {
                EditorGUILayout.PropertyField(prop_index);
            }
            else
            {
                EditorGUILayout.PropertyField(prop_panelName);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}

