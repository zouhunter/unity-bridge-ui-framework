#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-04-29 11:34:55
    * 说    明：       
* ************************************************************************************/
#endregion
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
namespace BridgeUI.Extend.XLua
{

    using ResourceType = LuaPanel.ResourceType;
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LuaPanel), editorForChildClasses: true)]
    public class LuaPanelDrawer : Editor
    {
        SerializedProperty scriptPorp;
        SerializedProperty resourceType;
        SerializedProperty luaScript;
        SerializedProperty streamingPath;
        SerializedProperty webUrl;
        SerializedProperty assetBundleName;
        SerializedProperty assetName;
        SerializedProperty menu;
        SerializedProperty scriptName;
        SerializedProperty bundleLoader;

        void OnEnable()
        {
            InitProps();
        }

        private void InitProps()
        {
            scriptPorp = serializedObject.FindProperty("m_Script");
            resourceType = serializedObject.FindProperty("resourceType");
            luaScript = serializedObject.FindProperty("luaScript");
            streamingPath = serializedObject.FindProperty("streamingPath");
            webUrl = serializedObject.FindProperty("webUrl");
            assetBundleName = serializedObject.FindProperty("assetBundleName");
            assetName = serializedObject.FindProperty("assetName");
            menu = serializedObject.FindProperty("menu");
            scriptName = serializedObject.FindProperty("scriptName");
            bundleLoader = serializedObject.FindProperty("bundleLoader");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawScript();
            DrawResourceType();
            SwitchDrawProps();
            DrawChildProps();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawResourceType()
        {
            EditorGUILayout.PropertyField(resourceType);

        }
        private void DrawScript()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(scriptPorp);
            EditorGUI.EndDisabledGroup();
        }
        private void DrawChildProps()
        {
            var iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            var changed = false;
            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.name == "m_Script")
                {
                    continue;
                }
                changed |= EditorGUILayout.PropertyField(iterator, new GUILayoutOption[0]);
                enterChildren = false;
            }
            if (changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void SwitchDrawProps()
        {
            var type = (LuaPanel.ResourceType)resourceType.intValue;
            switch (type)
            {
                case ResourceType.OriginLink:
                    EditorGUILayout.PropertyField(luaScript);
                    break;
                case ResourceType.StreamingFile:
                    EditorGUILayout.PropertyField(streamingPath);
                    break;
                case ResourceType.WebFile:
                    EditorGUILayout.PropertyField(webUrl);
                    break;
                case ResourceType.AssetBundle:
                    EditorGUILayout.PropertyField(bundleLoader);
                    EditorGUILayout.PropertyField(menu);
                    EditorGUILayout.PropertyField(assetBundleName);
                    EditorGUILayout.PropertyField(assetName);
                    break;

                case ResourceType.Resource:
                    EditorGUILayout.PropertyField(scriptName);
                    break;
                case ResourceType.RuntimeString:
                    break;
                default:
                    break;
            }
        }
    }
}