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

    [CustomEditor(typeof(ButtonsPanel), true)]
    public class ButtonsPanelDrawer : Editor
    {
        SerializedProperty btnsProp;
        ReorderableList reorderList;
        private void OnEnable()
        {
            btnsProp = serializedObject.FindProperty("btns");
            InitReorderList();
        }
        private void InitReorderList()
        {
            reorderList = new ReorderableList(serializedObject, btnsProp);
            reorderList.drawHeaderCallback += DrawHeader;
            reorderList.drawElementCallback += DrawElement;
            reorderList.displayAdd = true;
            reorderList.displayRemove = true;
        }
        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, new GUIContent("按扭列表(id 对应 于指定的事件 id)"));
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var r1 = new Rect(rect.x, rect.y, rect.width * 0.3f, rect.height);
            EditorGUI.LabelField(r1, new GUIContent(index.ToString()));
            var r2 = new Rect(rect.x + r1.width, rect.y, rect.width - r1.width, rect.height);
            btnsProp.GetArrayElementAtIndex(index).objectReferenceValue = EditorGUI.ObjectField(r2, btnsProp.GetArrayElementAtIndex(index).objectReferenceValue, typeof(Button), true);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            reorderList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}