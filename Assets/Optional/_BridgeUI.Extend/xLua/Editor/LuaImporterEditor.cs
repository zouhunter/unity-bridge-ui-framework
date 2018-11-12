using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.U2D;
using UnityEngine.Experimental.U2D;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine.Experimental.Rendering;
namespace LuaView
{
    [CustomEditor(typeof(LuaImporter))]
    [CanEditMultipleObjects]
    internal class LuaImporterEditor : ScriptedImporterEditor
    {
        private SerializedProperty textValueProp;
        private SerializedProperty lua_pic_guidProp;

        public override void OnEnable()
        {
            base.OnEnable();
            textValueProp = serializedObject.FindProperty("textValue");
            lua_pic_guidProp = serializedObject.FindProperty("lua_pic_guid");
        }

        public override void OnInspectorGUI()
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight * 2);
            EditorGUI.HelpBox(rect, "Lua代码可视化", MessageType.Info);
            rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.SelectableLabel(rect,"Icon-guid:" +lua_pic_guidProp.stringValue);
        }
    }
}