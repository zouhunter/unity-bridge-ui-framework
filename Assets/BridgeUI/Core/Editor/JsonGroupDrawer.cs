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

namespace BridgeUI
{
    using ResourceType = JsonGroup.ResourceType;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(JsonGroup), editorForChildClasses: true)]
    public class JsonGroupDrawer : Editor
    {
        SerializedProperty scriptPorp;
        SerializedProperty graphTextProp;
        SerializedProperty resourceType;
        SerializedProperty streamingPath;
        SerializedProperty webUrl;
        SerializedProperty assetBundleName;
        SerializedProperty assetName;
        SerializedProperty menu;
        SerializedProperty resetMenu;
        SerializedProperty resourcePathProp;

        void OnEnable()
        {
            InitProps();
        }

        private void InitProps()
        {
            scriptPorp = serializedObject.FindProperty("m_Script");
            resourceType = serializedObject.FindProperty("resourceType");
            streamingPath = serializedObject.FindProperty("streamingPath");
            webUrl = serializedObject.FindProperty("webUrl");
            assetBundleName = serializedObject.FindProperty("assetBundleName");
            assetName = serializedObject.FindProperty("assetName");
            menu = serializedObject.FindProperty("menu");
            resetMenu = serializedObject.FindProperty("resetMenu");
            graphTextProp = serializedObject.FindProperty("graphText");
            resourcePathProp = serializedObject.FindProperty("resourcePath");
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
            var type = (ResourceType)resourceType.intValue;
            switch (type)
            {
                case ResourceType.OriginLink:
                    EditorGUILayout.PropertyField(graphTextProp);
                    break;
                case ResourceType.StreamingFile:
                    DrawStreamPath();
                    break;
                case ResourceType.WebFile:
                    EditorGUILayout.PropertyField(webUrl);
                    break;
#if AssetBundleTools
                    case ResourceType.AssetBundle:
                    EditorGUILayout.PropertyField(resetMenu);

                    if(resetMenu.boolValue)
                    EditorGUILayout.PropertyField(menu);

                    EditorGUILayout.PropertyField(assetBundleName);
                    EditorGUILayout.PropertyField(assetName);
                    break;
#endif
                case ResourceType.Resource:
                    DrawResourcePath();
                    break;
                default:
                    break;
            }
        }

        private void DrawStreamPath()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(streamingPath);
                if (GUILayout.Button("open",EditorStyles.miniButtonRight,GUILayout.Width(40)))
                {
                   var path = EditorUtility.OpenFilePanel("选择正确的UIGraph的json文件", Application.streamingAssetsPath, "json");
                    if (!string.IsNullOrEmpty(path))
                    {
                        var streamPath = path.Replace(Application.streamingAssetsPath + "/", "");
                        streamingPath.stringValue = streamPath;
                    }
                }
            }
        }
        private void DrawResourcePath()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(resourcePathProp);

                if (GUILayout.Button("open", EditorStyles.miniButtonRight, GUILayout.Width(40)))
                {
                    var path = EditorUtility.OpenFilePanel("选择正确的UIGraph的json文件", Application.dataPath, "json");
                    if (!string.IsNullOrEmpty(path) && path.Contains("Resources"))
                    {
                        var resourcePath = path.Substring(path.IndexOf("Resources") + 10);
                        Debug.Log(resourcePath);
                        resourcePath = resourcePath.Remove(resourcePath.IndexOf("."));
                        resourcePathProp.stringValue = resourcePath;
                    }
                }
            }
        }
    }
}