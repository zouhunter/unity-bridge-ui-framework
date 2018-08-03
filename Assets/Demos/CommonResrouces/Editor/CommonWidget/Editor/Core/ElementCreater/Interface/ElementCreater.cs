using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace CommonWidget
{

    public abstract class DefultElementCreater : ElementCreater
    {
        protected abstract string MenuPath { get; }
        protected abstract Type CommponentType { get; }
        public override GameObject CreateOrCharge(WidgetItem info)
        {
            var oldBtn = Selection.activeTransform == null ? null : Selection.activeTransform.GetComponent(CommponentType);
            if (oldBtn != null)
            {
                Undo.RecordObject(oldBtn, "charge:" + oldBtn.name);
                ChargeWidgetInfo(oldBtn, info);
                return oldBtn.gameObject;
            }
            else
            {
                var instence = CreateInstence(info);
                if(instence)
                {
                    instence.name = info.name;
                    Undo.RecordObject(instence, "charge:" + instence.name);
                    ChargeWidgetInfo(instence, info);
                    instence.transform.SetParent(Selection.activeTransform, false);
                    instence.transform.localPosition = Vector3.zero;
                    return instence.gameObject;
                }
            }

            return null;
        }
        protected virtual Component CreateInstence(WidgetItem info)
        {
            var ok = EditorApplication.ExecuteMenuItem(MenuPath);
            if (ok)
            {
                var created = Selection.activeGameObject;
                var btn = created.GetComponent(CommponentType);
                return btn;
            }
            return null;
        }

        protected abstract void ChargeWidgetInfo(Component component, WidgetItem info);
    }

    public abstract class ElementCreater : IElementCreater
    {
        protected List<string> _keys;
        public virtual List<string> Keys { get { if (_keys == null) _keys = CreateDefultList(); return _keys; } }
        public abstract GameObject CreateOrCharge(WidgetItem info);
        public abstract List<Sprite> GetPreviewList(WidgetItem info);
        protected abstract List<string> CreateDefultList();
    }
}
