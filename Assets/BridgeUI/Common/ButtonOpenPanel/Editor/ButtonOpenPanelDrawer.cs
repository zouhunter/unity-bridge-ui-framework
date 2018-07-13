using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace BridgeUI.Common
{

    [CustomEditor(typeof(ButtonOpenPanel), true)]
    public class ButtonOpenPanelDrawer : Editor
    {
        SerializedProperty holdersPorp;
        ReorderableList reorderList;
        private void OnEnable()
        {
            holdersPorp = serializedObject.FindProperty("holders");
            InitReorderList();
        }
        private void InitReorderList()
        {
            reorderList = new ReorderableList(serializedObject, holdersPorp);
            reorderList.drawHeaderCallback += DrawHeader;
            reorderList.drawElementCallback += DrawElement;
            reorderList.elementHeightCallback += GetElementHeight;
            reorderList.displayAdd = true;
            reorderList.displayRemove = true;
        }

        private float GetElementHeight(int index)
        {
            var prop = holdersPorp.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(prop);
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, new GUIContent("按扭列表"));
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var prop = holdersPorp.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, prop);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            reorderList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}