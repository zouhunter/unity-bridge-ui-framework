using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using System;
using UnityEditor;

namespace BridgeUI.Drawer
{
    public class StringListDrawer : ReorderListDrawer
    {
        public override void InitReorderList(IList list, Type type)
        {
            base.InitReorderList(list, type);
            reorderList.onAddCallback = OnAddElement;
            reorderList.headerHeight = 0.1f;
        }

        private void OnAddElement(ReorderableList list)
        {
            List.Add("");
        }
        protected override float ElementHeightCallback(int index)
        {
            return EditorGUIUtility.singleLineHeight + 6;
        }
        protected override void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = BridgeUI.Drawer.BridgeEditorUtility.DrawBoxRect(rect, "", 3);
            List[index] = EditorGUI.TextField(rect, index.ToString(), List[index].ToString());
        }
    }
}