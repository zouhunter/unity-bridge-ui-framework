using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace BridgeUI.Control
{
    [CustomEditor(typeof(BoarderImage))]
    public class BorderImageEditor : UnityEditor.UI.ImageEditor
    {
        private SerializedProperty left;
        private SerializedProperty right;
        private SerializedProperty top;
        private SerializedProperty bottom;

        protected override void OnEnable()
        {
            base.OnEnable();
            left = serializedObject.FindProperty("left");
            right = serializedObject.FindProperty("right");
            top = serializedObject.FindProperty("top");
            bottom = serializedObject.FindProperty("bottom");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(left);
            EditorGUILayout.PropertyField(right);
            EditorGUILayout.PropertyField(top);
            EditorGUILayout.PropertyField(bottom);
            serializedObject.ApplyModifiedProperties();
        }
    }
}