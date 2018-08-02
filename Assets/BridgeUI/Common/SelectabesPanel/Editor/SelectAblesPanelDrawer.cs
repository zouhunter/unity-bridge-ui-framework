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
    [CustomEditor(typeof(SelectAblesPanel), true)]
    public class SelectAblesPanelDrawer : Editor
    {
        SerializedProperty selectablesProp;
        ControlWithIDListDrawer selectablesDrawer;

        private void OnEnable()
        {
            selectablesProp = serializedObject.FindProperty("selectables");
            InitReorderList();
        }
        private void InitReorderList()
        {
            selectablesDrawer = new ControlWithIDListDrawer("触发器列表", typeof(Selectable), selectablesProp);
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            selectablesDrawer.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}