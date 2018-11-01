using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
using BridgeUI.Model;
namespace BridgeUI.Drawer
{
    [CustomPropertyDrawer(typeof(ResourceUIInfo))]

    public class ResourceUIInfoDrawer : UIInfoBaseDrawer
    {
        protected SerializedProperty goodProp;
        protected SerializedProperty guidProp;
        protected SerializedProperty resourcePathProp;

        protected const int ht = 2;

        protected override void InitPropertys(SerializedProperty property)
        {
            base.InitPropertys(property);
            goodProp = property.FindPropertyRelative("good");
            guidProp = property.FindPropertyRelative("guid");
            resourcePathProp = property.FindPropertyRelative("resourcePath");
        }
        protected override float GetInfoItemHeight()
        {
            return EditorGUIUtility.singleLineHeight * 1.2f;
        }

        protected override void DragAndDrapAction(Rect acceptRect)
        {
            base.DragAndDrapAction(acceptRect);
            if (Event.current.type == EventType.Repaint)
            {
                var path0 = AssetDatabase.GUIDToAssetPath(guidProp.stringValue);
                var obj0 = AssetDatabase.LoadAssetAtPath<GameObject>(path0);
                goodProp.boolValue = obj0 != null;
            }
        }

        private const int labelWidth = 60;
        protected override void DrawExpanded(Rect opendRect)
        {
            var rect = new Rect(opendRect.x, opendRect.y + 5f, opendRect.width, singleHeight);
            var labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
            var bodyRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.LabelField(labelRect, "[资源包]");
            resourcePathProp.stringValue = EditorGUI.TextField(bodyRect, resourcePathProp.stringValue);
            rect.y += singleHeight;
            EditorGUI.PropertyField(rect, typeProp);
            EditorGUI.EndDisabledGroup();
        }

        protected override void DrawObjectField(Rect acceptRect)
        {
            if (goodProp.boolValue)
            {
                if (GUI.Button(acceptRect, "", EditorStyles.objectFieldMiniThumb))
                {
                    var path = AssetDatabase.GUIDToAssetPath(guidProp.stringValue);
                    var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    EditorGUIUtility.PingObject(obj);
                }
            }
            else
            {
                var obj = EditorGUI.ObjectField(acceptRect, null, typeof(GameObject), false);
                if (obj != null)
                {
                    var path = AssetDatabase.GetAssetPath(obj);
                    guidProp.stringValue = AssetDatabase.AssetPathToGUID(path);
                }
            }

        }

        protected override void WorningIfNotRight(Rect rect)
        {
            if (goodProp != null && !goodProp.boolValue)
            {
                Worning(rect, panelNameProp.stringValue + " Changed！!!");
            }
        }

        protected override void OnDragPerformGameObject(GameObject go)
        {
            var path = AssetDatabase.GetAssetPath(go);
            if (!string.IsNullOrEmpty(path))
            {
                guidProp.stringValue = AssetDatabase.AssetPathToGUID(path);
                panelNameProp.stringValue = go.name;
                resourcePathProp.stringValue = BridgeUI.Drawer.BridgeEditorUtility.GetResourcesPath(path);
                Debug.Log(resourcePathProp.stringValue);
            }
        }

        protected override void InstantiatePrefab(GameObject gopfb, Transform parent)
        {
            base.InstantiatePrefab(gopfb, parent);
            var path = AssetDatabase.GetAssetPath(gopfb);
            guidProp.stringValue = AssetDatabase.AssetPathToGUID(path);
        }

        protected override GameObject GetPrefabItem()
        {
            return Resources.Load<GameObject>(resourcePathProp.stringValue);
        }
    }
}