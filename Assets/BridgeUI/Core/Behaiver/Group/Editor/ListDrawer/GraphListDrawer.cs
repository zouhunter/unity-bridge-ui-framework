using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BridgeUI;
using BridgeUI.Model;
using System;

namespace BridgeUIEditor
{
    public class GraphListDrawer : ReorderListDrawer
    {
        public GraphListDrawer(string title) : base(title) { }

        public override void InitReorderList(SerializedProperty property)
        {
            base.InitReorderList(property);
            reorderList.drawElementBackgroundCallback = DrawBackground;
        }

        private void DrawBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (property.arraySize <= index || index < 0) return;

            if (isFocused)
            {
                var oldColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.yellow;
                GUI.Box(rect, "");
                GUI.backgroundColor = oldColor;
            }
        }
        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            var prop = property.GetArrayElementAtIndex(index);
            var graph = prop.objectReferenceValue;
            var graphName = graph == null ? "empty" : graph.name;

            rect = BridgeEditorUtility.DrawBoxRect(rect, index.ToString());

            var btnRect = new Rect(rect.x, rect.y, rect.width - 30, EditorGUIUtility.singleLineHeight);

            if (GUI.Button(btnRect, graphName, EditorStyles.toolbarButton))
            {
                AssetDatabase.OpenAsset(graph);
            }

            btnRect = new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight);

            if (graph != null)
            {
                if (GUI.Button(btnRect, " ", EditorStyles.objectFieldMiniThumb)) {
                    EditorGUIUtility.PingObject(graph);
                }
                DragGroupObj(btnRect, prop);
            }
            else
            {
                prop.objectReferenceValue = EditorGUI.ObjectField(btnRect, prop.objectReferenceValue, typeof(BridgeUI.Graph.UIGraph), false);
            }
        }
        public override void DoLayoutList()
        {
            base.DoLayoutList();
            DrawGraphAcceptRegion();
        }

        /// <summary>
        /// 绘制作快速导入的区域
        /// </summary>
        private void DrawGraphAcceptRegion()
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            rect.y -= EditorGUIUtility.singleLineHeight;

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                    break;
                case EventType.DragPerform:
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        var objs = DragAndDrop.objectReferences;
                        for (int i = 0; i < objs.Length; i++)
                        {
                            var obj = objs[i];
                            if (obj is BridgeUI.Graph.UIGraph)
                            {
                                property.InsertArrayElementAtIndex(property.arraySize);
                                var prop = property.GetArrayElementAtIndex(property.arraySize - 1);
                                prop.objectReferenceValue = obj;
                                property.serializedObject.ApplyModifiedProperties();
                                Debug.Log(property.serializedObject.targetObject);
                            }
                        }
                    }
                    break;
            }
        }



        protected virtual void DragGroupObj(Rect acceptRect, SerializedProperty prop)
        {
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    if (acceptRect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                    break;
                case EventType.DragPerform:
                    if (acceptRect.Contains(Event.current.mousePosition))
                    {
                        if (DragAndDrop.objectReferences.Length > 0)
                        {
                            var obj = DragAndDrop.objectReferences[0];
                            if (obj is NodeGraph.DataModel.NodeGraphObj)
                            {
                                var path = AssetDatabase.GetAssetPath(obj);
                                var guid = AssetDatabase.AssetPathToGUID(path);
                                var keyProp = prop.FindPropertyRelative("graphName");
                                var guidProp = prop.FindPropertyRelative("guid");
                                guidProp.stringValue = guid;
                                keyProp.stringValue = obj.name;
                            }
                        }
                        DragAndDrop.AcceptDrag();
                        Event.current.Use();
                    }
                    break;
            }
        }
        protected override float ElementHeightCallback(int index)
        {
            var prop = property.GetArrayElementAtIndex(index);
            var height = EditorGUI.GetPropertyHeight(prop, null, true) + BridgeUIEditor.BridgeEditorUtility.padding * 2;
            return height;
        }
    }
}
