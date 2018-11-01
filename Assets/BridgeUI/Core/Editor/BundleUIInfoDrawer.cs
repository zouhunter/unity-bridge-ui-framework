using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
using BridgeUI.Model;
namespace BridgeUI.Drawer
{
    [CustomPropertyDrawer(typeof(BundleUIInfo))]

    public class BundleUIInfoDrawer : UIInfoBaseDrawer
    {
        protected SerializedProperty goodProp;
        protected SerializedProperty guidProp;
        protected SerializedProperty bundleNameProp;

        protected const int ht = 2;

        protected override void InitPropertys(SerializedProperty property)
        {
            base.InitPropertys(property);
            goodProp = property.FindPropertyRelative("good");
            guidProp = property.FindPropertyRelative("guid");
            bundleNameProp = property.FindPropertyRelative("bundleName");
        }
        protected override float GetInfoItemHeight()
        {
            return  EditorGUIUtility.singleLineHeight * 1.2f;
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
            EditorGUI.LabelField(labelRect,"[资源包]");
            bundleNameProp.stringValue = EditorGUI.TextField(bodyRect, bundleNameProp.stringValue);
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
                AssetImporter importer = AssetImporter.GetAtPath(path);
                panelNameProp.stringValue = go.name;
                bundleNameProp.stringValue = importer.assetBundleName;
            }
        }
        protected override void InstantiatePrefab(GameObject gopfb,Transform parent)
        {
            base.InstantiatePrefab(gopfb,parent);
            var path = AssetDatabase.GetAssetPath(gopfb);
            guidProp.stringValue = AssetDatabase.AssetPathToGUID(path);
        }

        protected override GameObject GetPrefabItem()
        {
            string[] paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleNameProp.stringValue, panelNameProp.stringValue);
            if (paths != null && paths.Length > 0)
            {
                GameObject gopfb = AssetDatabase.LoadAssetAtPath<GameObject>(paths[0]);
                return gopfb;
            }
            return null;
        }
    }
}