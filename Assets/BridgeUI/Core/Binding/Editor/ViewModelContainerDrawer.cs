using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BridgeUI.Binding;
using UnityEditor;
using System;
using System.Linq;
using UnityEditorInternal;

namespace BridgeUI.Drawer
{
    [CustomEditor(typeof(ViewModelContainer))]
    public class ViewModelContainerDrawer : Editor
    {
        private GUIContent[] options;
        private Type[] viewModelTypes;
        private SerializedProperty prop_instence;
        public const float middleButtonWidth = 45f;
        private Editor editor;

        private void OnEnable()
        {
            prop_instence = serializedObject.FindProperty("instence");
            InitTypeOptions();

            if (prop_instence.objectReferenceValue != null)
            {
                editor = Editor.CreateEditor(prop_instence.objectReferenceValue);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawInstence();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInstence()
        {
            if (GUILayout.Button("更改ViewModel类型",EditorStyles.toolbarButton)){
                OnUpdateElement();
            }

            if (editor != null && editor.target && prop_instence.objectReferenceValue != null)
            {
                editor.serializedObject.Update();
                editor.OnInspectorGUI();
                editor.serializedObject.ApplyModifiedProperties();
            }
        }


        private void OnUpdateElement()
        {
            EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.zero), options, -1, (x, y, z) =>
            {
                if (prop_instence.objectReferenceValue != null)
                    DestroyImmediate(prop_instence.objectReferenceValue, true);

                var type = viewModelTypes[z];
                var instence = GetAssetByType(type);
                prop_instence.objectReferenceValue = instence;
                serializedObject.ApplyModifiedProperties();
                if (prop_instence.objectReferenceValue != null){
                   editor = Editor.CreateEditor(prop_instence.objectReferenceValue);
                }
                AssetDatabase.Refresh();

            }, null);
        }

        private ScriptableObject GetAssetByType(Type type)
        {
            var path = AssetDatabase.GetAssetPath(target);
            var instence = ScriptableObject.CreateInstance(type);
            instence.name = type.Name;
            instence.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(instence, path);
            return instence as ScriptableObject;
        }

        private void InitTypeOptions()
        {
            viewModelTypes = BridgeUI.Drawer.MvvmUtil.GetSubInstenceTypes(typeof(ViewModel)).ToArray();
            options = viewModelTypes.Select(x => new GUIContent(x.FullName)).ToArray();
        }

    }
}