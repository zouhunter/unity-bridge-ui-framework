using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

namespace BridgeUIEditor
{
    public abstract class ReorderListDrawer
    {
        protected ReorderableList reorderList;
        protected SerializedProperty property;
        protected IList list;
        protected Type type;
        protected string title;
        public event UnityAction<int> onSelectID;
        public IList List { get { return list; } }
        protected int lastFocus = -1;
        public ReorderListDrawer(string title = null)
        {
            this.title = title;
        }

        public ReorderableList.HeaderCallbackDelegate drawHeaderCallback { get; set; }

        public virtual void InitReorderList(SerializedProperty property)
        {
            this.property = property;
            reorderList = new ReorderableList(property.serializedObject, property);
            OnRegistEvents();
        }
        public virtual void InitReorderList(IList list,Type type)
        {
            this.list = list;
            this.type = type;
            reorderList = new ReorderableList(list, type);
            OnRegistEvents();
        }

        protected virtual void OnRegistEvents()
        {
            reorderList.drawHeaderCallback = DrawHeaderCallBack;
            reorderList.drawElementCallback = DrawElementCallBack;
            reorderList.elementHeightCallback = ElementHeightCallback;
        }

        protected abstract float ElementHeightCallback(int index);
        protected virtual void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused && lastFocus != index){
                SetOnSelect(index);
            }
          
        }
        protected virtual void DrawHeaderCallBack(Rect rect)
        {
            if (drawHeaderCallback != null)
                drawHeaderCallback.Invoke(rect);
            else
            {
                EditorGUI.LabelField(rect, title);
            }
        }

        protected void SetOnSelect(int index)
        {
            if (onSelectID != null)
            {
                onSelectID.Invoke(index);
            }
            lastFocus = index;
        }

        public virtual void DoLayoutList()
        {
            if(reorderList != null)
            {
                reorderList.DoLayoutList();
            }
        }
        public virtual void DoList(Rect rect)
        {
            if (reorderList != null)
                reorderList.DoList(rect);
        }
    }
}