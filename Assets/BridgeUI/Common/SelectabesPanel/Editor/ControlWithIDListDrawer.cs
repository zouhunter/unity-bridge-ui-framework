using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace BridgeUI.Common
{
    public class ControlWithIDListDrawer
    {
        protected ReorderableList reorderList;
        protected SerializedProperty property;
        protected Type type;
        protected string title;
        protected List<Type> supportedTypes;

        public ControlWithIDListDrawer(string title, Type type, SerializedProperty property)
        {
            this.title = title;
            this.type = type;
            this.property = property;
            InitReorderList();
        }

        protected virtual void InitReorderList()
        {
            reorderList = new ReorderableList(property.serializedObject, property);
            reorderList.drawHeaderCallback += DrawHeader;
            reorderList.drawElementCallback += DrawElement;
            reorderList.displayAdd = true;
            reorderList.displayRemove = true;
        }

        protected virtual void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, new GUIContent(title));
        }

        protected virtual void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var r1 = new Rect(rect.x, rect.y, rect.width * 0.3f, rect.height);
            EditorGUI.LabelField(r1, new GUIContent(index.ToString()));
            var r2 = new Rect(rect.x + r1.width, rect.y, rect.width - r1.width, rect.height);
            property.GetArrayElementAtIndex(index).objectReferenceValue = EditorGUI.ObjectField(r2, property.GetArrayElementAtIndex(index).objectReferenceValue, type, true);
        }

        public virtual void DoLayoutList()
        {
            reorderList.DoLayoutList();
        }
    }

}
