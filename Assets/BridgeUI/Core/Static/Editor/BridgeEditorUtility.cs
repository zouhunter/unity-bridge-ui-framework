using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ShowMode = BridgeUI.ShowMode;
using BridgeUI;
using UnityEditor;
using System;

namespace BridgeUIEditor
{
    public static class BridgeEditorUtility
    {
        public const float padding = 5;
        public static float currentViewWidth { get { return EditorGUIUtility.currentViewWidth - 100; } }
        public static Rect DrawBoxRect(Rect orignalRect, string index)
        {
            var idRect = new Rect(orignalRect.x - 15, orignalRect.y + 8, 20, 20);
            EditorGUI.LabelField(idRect, index);
            var boxRect = PaddingRect(orignalRect, padding * 0.5f);
            GUI.Box(boxRect, "");
            var rect = PaddingRect(orignalRect);
            return rect;
        }

        public static Rect PaddingRect(Rect orignalRect, float padding = padding)
        {
            var rect = new Rect(orignalRect.x + padding, orignalRect.y + padding, orignalRect.width - padding * 2, orignalRect.height - padding * 2);
            return rect;
        }
        public static void DelyAcceptObject(UnityEngine.Object instence, UnityAction<UnityEngine.Object> onCreate)
        {
            if (onCreate == null) return;
            EditorApplication.CallbackFunction action = () =>
            {
                var path = AssetDatabase.GetAssetPath(instence);

                if (!string.IsNullOrEmpty(path))
                {
                    var item = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                    if (item)
                    {
                        onCreate.Invoke(item);
                    }

                    EditorApplication.update = null;

                }
            };
            EditorApplication.update = action;
        }
        public static void SetPrefab(this BridgeUI.Model.NodeInfo nodeInfo, GameObject prefab)
        {
            if (prefab != null)
            {
                var path = AssetDatabase.GetAssetPath(prefab);
                if (!string.IsNullOrEmpty(path))
                {
                    nodeInfo.guid = AssetDatabase.AssetPathToGUID(path);
                }
            }
            else
            {
                nodeInfo.guid = null;
            }
        }
        public static GameObject GetPrefab(this BridgeUI.Model.NodeInfo nodeInfo)
        {
            if (!string.IsNullOrEmpty(nodeInfo.guid))
            {
                var path = AssetDatabase.GUIDToAssetPath(nodeInfo.guid);
                if (!string.IsNullOrEmpty(path))
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    return prefab;
                }
            }
            return null;
        }
        public static void ApplyPrefab(GameObject gitem)
        {
            var instanceRoot = PrefabUtility.FindValidUploadPrefabInstanceRoot(gitem);
            var prefab = PrefabUtility.GetPrefabParent(instanceRoot);
            if (prefab != null)
            {
                if (prefab.name == gitem.name)
                {
                    PrefabUtility.ReplacePrefab(gitem, prefab, ReplacePrefabOptions.ConnectToPrefab);
                }
            }
        }
        /// <summary>
        /// Reset the value of a property.
        /// </summary>
        /// <param name="property">Serialized property for a serialized property.</param>
        public static void ResetValue(SerializedProperty property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = 0;
                    break;
                case SerializedPropertyType.Boolean:
                    property.boolValue = false;
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = 0f;
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = "";
                    break;
                case SerializedPropertyType.Color:
                    property.colorValue = Color.black;
                    break;
                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = null;
                    break;
                case SerializedPropertyType.LayerMask:
                    property.intValue = 0;
                    break;
                case SerializedPropertyType.Enum:
                    property.enumValueIndex = 0;
                    break;
                case SerializedPropertyType.Vector2:
                    property.vector2Value = default(Vector2);
                    break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = default(Vector3);
                    break;
                case SerializedPropertyType.Vector4:
                    property.vector4Value = default(Vector4);
                    break;
                case SerializedPropertyType.Rect:
                    property.rectValue = default(Rect);
                    break;
                case SerializedPropertyType.ArraySize:
                    property.intValue = 0;
                    break;
                case SerializedPropertyType.Character:
                    property.intValue = 0;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 1f);
                    break;
                case SerializedPropertyType.Bounds:
                    property.boundsValue = default(Bounds);
                    break;
                case SerializedPropertyType.Gradient:
                    //!TODO: Amend when Unity add a public API for setting the gradient.
                    break;
            }

            if (property.isArray)
            {
                property.arraySize = 0;
            }

            ResetChildPropertyValues(property);
        }

        internal static Enum EnumMaskField(string title, Enum buildOption)
        {
#if UNITY_2018_1_OR_NEWER
            return EditorGUILayout.EnumFlagsField(title, buildOption);
#else
            return EditorGUILayout.EnumMaskField(title, buildOption);
#endif
        }

        private static void ResetChildPropertyValues(SerializedProperty element)
        {
            if (!element.hasChildren)
                return;

            var childProperty = element.Copy();
            int elementPropertyDepth = element.depth;
            bool enterChildren = true;

            while (childProperty.Next(enterChildren) && childProperty.depth > elementPropertyDepth)
            {
                enterChildren = false;
                ResetValue(childProperty);
            }
        }

        /// <summary>
        /// Copies value of <paramref name="sourceProperty"/> into <pararef name="destProperty"/>.
        /// </summary>
        /// <param name="destProperty">Destination property.</param>
        /// <param name="sourceProperty">Source property.</param>
        public static void CopyPropertyValue(SerializedProperty destProperty, SerializedProperty sourceProperty)
        {
            if (destProperty == null)
                throw new ArgumentNullException("destProperty");
            if (sourceProperty == null)
                throw new ArgumentNullException("sourceProperty");

            sourceProperty = sourceProperty.Copy();
            destProperty = destProperty.Copy();

            CopyPropertyValueSingular(destProperty, sourceProperty);

            if (sourceProperty.hasChildren)
            {
                int elementPropertyDepth = sourceProperty.depth;
                while (sourceProperty.Next(true) && destProperty.Next(true) && sourceProperty.depth > elementPropertyDepth)
                    CopyPropertyValueSingular(destProperty, sourceProperty);
            }
        }

        public static string ShowModelToString(ShowMode show)
        {
            string str = "";
            if (show.auto)
            {
                str += "[a]";
            }
            if (show.cover)
            {
                str += "[c]";
            }
            if (show.mutex != MutexRule.NoMutex)
            {
                switch (show.mutex)
                {
                    case MutexRule.NoMutex:
                        break;
                    case MutexRule.SameParentAndLayer:
                        str += "[m(p)]";
                        break;
                    case MutexRule.SameLayer:
                        str += "[m]";
                        break;
                    default:
                        break;
                }
            }
            if (show.baseShow != BaseShow.NoChange)
            {
                switch (show.baseShow)
                {
                    case BaseShow.NoChange:
                        break;
                    case BaseShow.Hide:
                        str += "[h]";
                        break;
                    case BaseShow.Destroy:
                        str += "[d]";
                        break;
                    default:
                        break;
                }
            }
            if (show.single)
            {
                str += "[s]";
            }
            return str;
        }

        private static void CopyPropertyValueSingular(SerializedProperty destProperty, SerializedProperty sourceProperty)
        {
            switch (destProperty.propertyType)
            {
                case SerializedPropertyType.Integer:
                    destProperty.intValue = sourceProperty.intValue;
                    break;
                case SerializedPropertyType.Boolean:
                    destProperty.boolValue = sourceProperty.boolValue;
                    break;
                case SerializedPropertyType.Float:
                    destProperty.floatValue = sourceProperty.floatValue;
                    break;
                case SerializedPropertyType.String:
                    destProperty.stringValue = sourceProperty.stringValue;
                    break;
                case SerializedPropertyType.Color:
                    destProperty.colorValue = sourceProperty.colorValue;
                    break;
                case SerializedPropertyType.ObjectReference:
                    destProperty.objectReferenceValue = sourceProperty.objectReferenceValue;
                    break;
                case SerializedPropertyType.LayerMask:
                    destProperty.intValue = sourceProperty.intValue;
                    break;
                case SerializedPropertyType.Enum:
                    destProperty.enumValueIndex = sourceProperty.enumValueIndex;
                    break;
                case SerializedPropertyType.Vector2:
                    destProperty.vector2Value = sourceProperty.vector2Value;
                    break;
                case SerializedPropertyType.Vector3:
                    destProperty.vector3Value = sourceProperty.vector3Value;
                    break;
                case SerializedPropertyType.Vector4:
                    destProperty.vector4Value = sourceProperty.vector4Value;
                    break;
                case SerializedPropertyType.Rect:
                    destProperty.rectValue = sourceProperty.rectValue;
                    break;
                case SerializedPropertyType.ArraySize:
                    destProperty.intValue = sourceProperty.intValue;
                    break;
                case SerializedPropertyType.Character:
                    destProperty.intValue = sourceProperty.intValue;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    destProperty.animationCurveValue = sourceProperty.animationCurveValue;
                    break;
                case SerializedPropertyType.Bounds:
                    destProperty.boundsValue = sourceProperty.boundsValue;
                    break;
                case SerializedPropertyType.Gradient:
                    //!TODO: Amend when Unity add a public API for setting the gradient.
                    break;
            }
        }
    }
}
