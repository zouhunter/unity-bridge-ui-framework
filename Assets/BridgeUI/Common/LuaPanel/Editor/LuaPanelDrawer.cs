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

namespace BridgeUI.Common
{
#if xLua

    [CanEditMultipleObjects]
    [CustomEditor(typeof(LuaPanel), editorForChildClasses: true)]
    public class LuaPanelDrawer : Editor
    {
        SerializedProperty scriptPorp;
        SerializedProperty resourceType;
        SerializedProperty luaScript;
        SerializedProperty scriptObj;
        SerializedProperty streamingPath;
        SerializedProperty webUrl;
        SerializedProperty assetBundleName;
        SerializedProperty assetName;
        SerializedProperty menu;
        SerializedProperty scriptName;

        void OnEnable()
        {
            InitProps();
        }

        private void InitProps()
        {
            scriptPorp = serializedObject.FindProperty("m_Script");
            resourceType = serializedObject.FindProperty("resourceType");
            luaScript = serializedObject.FindProperty("luaScript");
            scriptObj = serializedObject.FindProperty("scriptObj");
            streamingPath = serializedObject.FindProperty("streamingPath");
            webUrl = serializedObject.FindProperty("webUrl");
            assetBundleName = serializedObject.FindProperty("assetBundleName");
            assetName = serializedObject.FindProperty("assetName");
            menu = serializedObject.FindProperty("menu");
            scriptName = serializedObject.FindProperty("scriptName");
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
            var type = (LuaResourceType)resourceType.intValue;
            switch (type)
            {
                case LuaResourceType.OriginLink:
                    EditorGUILayout.PropertyField(luaScript);
                    break;
                case LuaResourceType.StreamingFile:
                    EditorGUILayout.PropertyField(streamingPath);
                    break;
                case LuaResourceType.WebFile:
                    EditorGUILayout.PropertyField(webUrl);
                    break;
                case LuaResourceType.AssetBundle:
                    EditorGUILayout.PropertyField(menu);
                    EditorGUILayout.PropertyField(assetBundleName);
                    EditorGUILayout.PropertyField(assetName);
                    break;
                case LuaResourceType.Resource:
                    EditorGUILayout.PropertyField(scriptName);
                    break;
                case LuaResourceType.ScriptObject:
                    EditorGUILayout.PropertyField(scriptObj);
                    break;
                case LuaResourceType.RuntimeString:
                    break;
                default:
                    break;
            }
        }
    }
#endif
}