using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace BridgeUI.Common
{
    [CustomEditor(typeof(PopUpSender))]
    public class PopUpSenderDrawer : Editor
    {
        private SerializedProperty popEnumProp;
        private SerializedProperty scriptProp;
        private SerializedProperty selectedProp;
        private SerializedProperty enumTypeProp;
        
        private string[] options;
        private int selected;

        void OnEnable()
        {
            scriptProp = serializedObject.FindProperty("m_Script");
            popEnumProp = serializedObject.FindProperty("popEnum");
            selectedProp = serializedObject.FindProperty("selected");
            enumTypeProp = serializedObject.FindProperty("enumType");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(scriptProp);
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(popEnumProp);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateOptions();
            }

            EditorGUI.BeginDisabledGroup(!popEnumProp.objectReferenceValue);
            if(options != null)
            {
                EditorGUI.BeginChangeCheck();
                selected = EditorGUILayout.Popup(selected, options);
                if (EditorGUI.EndChangeCheck())
                {
                    UpdateTargetInfo();
                }
            }
            EditorGUI.EndDisabledGroup();
            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateTargetInfo()
        {
            if(options != null && options.Length > selected && selected >=0)
            {
                selectedProp.stringValue = options[selected];
            }
        }

        private void UpdateOptions()
        {
            if(popEnumProp.objectReferenceValue)
            {
                var script = popEnumProp.objectReferenceValue as MonoScript;
                var classes = script.GetClass();
                if(!classes.IsEnum)
                {
                    Debug.LogError("请放入枚举类型！");
                    popEnumProp.objectReferenceValue = null;
                }
                else
                {
                    enumTypeProp.stringValue = classes.FullName;
                    options = System.Enum.GetNames(classes);
                }
            }
        }
    }
}