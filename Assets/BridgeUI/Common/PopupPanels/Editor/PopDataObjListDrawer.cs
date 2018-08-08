using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using BridgeUI.Common;
using System;

namespace BridgeUI.Drawer
{
    public class PopDataObjListDrawer : ReorderListDrawer
    {
        public PopDataObjListDrawer(string title) : base(title) { }

        protected override float ElementHeightCallback(int index)
        {
            return EditorGUIUtility.singleLineHeight + 2 * BridgeUI.Drawer.BridgeEditorUtility.padding;
        }

        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = BridgeEditorUtility.DrawBoxRect(rect, "");
            base.DrawElementCallBack(rect, index, isActive, isFocused);
            var prop = property.GetArrayElementAtIndex(index);
            GUIContent content = GUIContent.none;
            if (prop.objectReferenceValue != null)
            {
                var obj = (prop.objectReferenceValue as PopDataObj);
                content = new GUIContent(obj.descrition);
            }

            EditorGUI.PropertyField(rect, prop, content);
        }
    }
}