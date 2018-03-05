using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using BridgeUI;
using BridgeUI.Model;
namespace BridgeUIEditor
{
    [CustomPropertyDrawer(typeof(PrefabUIInfo))]
    public class PrefabInfoDrawer : UIInfoBaseDrawer
    {
        SerializedProperty prefabProp;//prefab
        protected const int ht = 1;

        protected override void InitPropertys(SerializedProperty property)
        {
            base.InitPropertys(property);
            prefabProp = property.FindPropertyRelative("prefab");
        }

        protected override float GetInfoItemHeight()
        {
            return ht * EditorGUIUtility.singleLineHeight;
        }

        protected override void DrawExpanded(Rect opendRect)
        {
            var rect = new Rect(opendRect.x, opendRect.y, opendRect.width, singleHeight);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(rect, typeProp, new GUIContent("[type]"));
            EditorGUI.EndDisabledGroup();
        }

        protected override void DrawObjectField(Rect rect)
        {
            if (prefabProp.objectReferenceValue != null)
            {
                if (GUI.Button(rect, " ", EditorStyles.objectFieldMiniThumb))
                {
                    EditorGUIUtility.PingObject(prefabProp.objectReferenceValue);
                }
            }
            else
            {
                prefabProp.objectReferenceValue = EditorGUI.ObjectField(rect, null, typeof(GameObject), false);
            }
        }


        protected override void OnDragPerformGameObject(GameObject go)
        {
            prefabProp.objectReferenceValue = go;
            panelNameProp.stringValue = go.name;
        }

        protected override void ResetBuildInfoOnOpen()
        {
            base.ResetBuildInfoOnOpen();

            if (prefabProp.objectReferenceValue != null)
            {
                panelNameProp.stringValue = prefabProp.objectReferenceValue.name;
            }
        }

        protected override GameObject GetPrefabItem()
        {
            GameObject gopfb = prefabProp.objectReferenceValue as GameObject;
            return gopfb;
        }

        protected override void WorningIfNotRight(Rect rect)
        {
            if (prefabProp.objectReferenceValue == null)
            {
                Worning(rect, "prefab:" + panelNameProp.stringValue + "  is empty!");
            }
        }
    }
}