using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BridgeUI;
using BridgeUI.Model;
using System;

namespace BridgeUI.Drawer
{
    public class GraphListDrawer : ReorderListDrawer
    {
        public event Action onChanged;
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
            base.DrawElementCallBack(rect, index, isActive, isFocused);
            var prop = property.GetArrayElementAtIndex(index);
            var graphGUID = prop.stringValue;
            BridgeUI.Graph.UIGraph graph = null;
            if (!string.IsNullOrEmpty(graphGUID))
            {
                var path = AssetDatabase.GUIDToAssetPath(graphGUID);
                if (!string.IsNullOrEmpty(path))
                {
                    graph = AssetDatabase.LoadAssetAtPath<Graph.UIGraph>(path);
                }
            }

            var graphName = graph == null ? "empty" : graph.name;

            rect = BridgeEditorUtility.DrawBoxRect(rect, index.ToString("00"));

            var btnRect = new Rect(rect.x, rect.y, rect.width - 30, EditorGUIUtility.singleLineHeight);

            if (GUI.Button(btnRect, graphName, EditorStyles.toolbarButton))
            {
                AssetDatabase.OpenAsset(graph);
            }

            btnRect = new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight);

            if (graph != null)
            {
                if (GUI.Button(btnRect, " ", EditorStyles.objectFieldMiniThumb))
                {
                    EditorGUIUtility.PingObject(graph);
                }
                DragGroupObj(btnRect, prop);
            }
            else
            {
                var graphNew = EditorGUI.ObjectField(btnRect, graph, typeof(BridgeUI.Graph.UIGraph), false) as Graph.UIGraph;
                if (graphNew != null && graphNew != graph)
                {
                    SetProperyValue(prop, graphNew);
                }
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
            var rect = GUILayoutUtility.GetRect(BridgeUI.Drawer.BridgeEditorUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            rect.y -= EditorGUIUtility.singleLineHeight;
            rect.height = 2 * EditorGUIUtility.singleLineHeight;
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
                                if (prop.objectReferenceValue != null)
                                {
                                    SetProperyValue(prop, obj as Graph.UIGraph);
                                }
                            }
                        }
                    }
                    break;
            }

            if (Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition))
            {
                reorderList.ReleaseKeyboardFocus();
                SetOnSelect(-1);
                Event.current.Use();
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
                            if (obj is BridgeUI.Graph.UIGraph)
                            {
                                SetProperyValue(prop, obj as BridgeUI.Graph.UIGraph);
                            }
                        }
                        DragAndDrop.AcceptDrag();
                        Event.current.Use();
                    }
                    break;
            }
        }

        protected void SetProperyValue(SerializedProperty property, Graph.UIGraph obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(path);
            property.stringValue = guid;
            property.serializedObject.ApplyModifiedProperties();
            EditorApplication.delayCall +=  OnGraphChanged;
        }

        protected override float ElementHeightCallback(int index)
        {
            var prop = property.GetArrayElementAtIndex(index);
            var height = EditorGUI.GetPropertyHeight(prop, null, true) + BridgeUI.Drawer.BridgeEditorUtility.padding * 2;
            return height;
        }

        private void OnGraphChanged()
        {
            if(onChanged != null)
            {
                onChanged.Invoke();
            }
        }
    }
}
