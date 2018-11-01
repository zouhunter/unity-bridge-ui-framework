using UnityEngine;
using UnityEditor;
using System;
using XLua;
namespace BridgeUI.Extend.XLua
{
    [CustomEditor(typeof(LuaScriptModel))]
    public class LuaScriptDrawer : Editor
    {
        private SerializedProperty luaScript_prop;

        private void OnEnable()
        {
            luaScript_prop = serializedObject.FindProperty("luaScript");
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeadTools();
            DrawScriptBody();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeadTools()
        {
        }
        private void DrawScriptBody()
        {
            var width = EditorGUIUtility.currentViewWidth - 30;
            var height = EditorGUIUtility.singleLineHeight * 100;
            luaScript_prop.stringValue = EditorGUILayout.TextArea(luaScript_prop.stringValue,GUILayout.Width(width),GUILayout.Height(height));
        }
    }

}
