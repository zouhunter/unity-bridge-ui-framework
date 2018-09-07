using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ShowMode = BridgeUI.ShowMode;
using BridgeUI;
using UnityEditor;
using System;
using NodeGraph.DataModel;

namespace BridgeUI.Drawer
{
    public static class BridgeEditorUtility
    {
        public const float padding = 5;
        public static float currentViewWidth { get { return EditorGUIUtility.currentViewWidth - 100; } }
        //记录坐标加载时,不需要记录下列信息变化
        public static List<string> coondinatePaths = new List<string>
        {
            "m_LocalPosition.x",
            "m_LocalPosition.y",
            "m_LocalPosition.z",
            "m_LocalRotation.x",
            "m_LocalRotation.y",
            "m_LocalRotation.z",
            "m_LocalRotation.w",
            "m_LocalScale.x",
            "m_LocalScale.y",
            "m_LocalScale.z",
            "m_AnchoredPosition.x",
            "m_AnchoredPosition.y",
            "m_SizeDelta.x",
            "m_SizeDelta.y",
            "m_AnchorMin.x",
            "m_AnchorMin.y",
            "m_AnchorMax.x",
            "m_AnchorMax.y",
            "m_Pivot.x",
            "m_Pivot.y",
        };
        [PreferenceItem("Bridge UI")]
        public static void PreferencesGUI()
        {
            BridgeUI.Setting.bundleNameFormat = EditorGUILayout.TextField("默认AssetBundle路径:", BridgeUI.Setting.bundleNameFormat);
        }

        public static Dictionary<UnityEngine.Object, Editor> editorDic = new Dictionary<UnityEngine.Object, Editor>();
        public static Dictionary<UnityEngine.Object, SerializedObject> serializedDic = new Dictionary<UnityEngine.Object, SerializedObject>();


        public static void DrawAddNodes(Rect position, NodeData data)
        {
            var nodePostion = new Rect(position.x + position.width - 40, 25, 15, 15);

            if (GUI.Button(nodePostion, "+"))
            {
                var count = data.OutputPoints.Count;
                data.AddOutputPoint(count.ToString(), "bridge", 100);
            }

            for (int i = 0; i < data.OutputPoints.Count; i++)
            {
                if (i == 0) continue;
                nodePostion.y += 31.8f;

                if (GUI.Button(nodePostion, "-"))
                {
                    if (data.OutputPoints.Count > 1)
                    {
                        data.OutputPoints.RemoveAt(i);
                        for (int j = i; j < data.OutputPoints.Count; j++)
                        {
                            data.OutputPoints[j].Label = j.ToString();
                        }
                    }
                }
            }

        }
        public static Rect DrawBoxRect(Rect orignalRect, string index,float padding = padding)
        {
            var idRect = new Rect(orignalRect.x - 15, orignalRect.y + 8, 20, 20);
            EditorGUI.LabelField(idRect, index);
            var boxRect = PaddingRect(orignalRect, padding * 0.5f);
            GUI.Box(boxRect, "");
            var rect = PaddingRect(orignalRect,padding);
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
        public static void SetPrefabGUID(this BridgeUI.Model.NodeInfo nodeInfo, GameObject prefab)
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
        #region 保存预制体
        public static void SavePrefab(ref int instanceID, bool destroy = true)
        {
            var gitem = EditorUtility.InstanceIDToObject(instanceID);
            if (gitem != null)
            {
                if (!Ignore(gitem))
                {
                    InternalApplyPrefab(gitem as GameObject);
                }
                if (destroy)
                {
                    GameObject.DestroyImmediate(gitem);
                    instanceID = 0;
                }
            }
            else
            {
                instanceID = 0;
            }
        }
        public static void SavePrefab(GameObject gitem, bool destroy = true)
        {
            if (!Ignore(gitem))
            {
                InternalApplyPrefab(gitem);
            }
            if (destroy)
            {
                GameObject.DestroyImmediate(gitem);
            }
        }
        public static void SavePrefab(SerializedProperty instanceIDProp, bool destroy = true)
        {
            var gitem = EditorUtility.InstanceIDToObject(instanceIDProp.intValue);
            if (gitem != null)
            {
                var transform = (gitem as GameObject).transform;
                if (!Ignore(gitem))
                {
                    InternalApplyPrefab(gitem as GameObject);
                }
                if (destroy)
                {
                    GameObject.DestroyImmediate(gitem);
                    instanceIDProp.intValue = 0;
                }
            }
            else
            {
                instanceIDProp.intValue = 0;
            }
        }
        private static void InternalApplyPrefab(GameObject gitem)
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
        #endregion 保存和加载预制体
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

        private static bool Ignore(UnityEngine.Object instence)
        {
            var modifyed = PrefabUtility.GetPropertyModifications(instence);
            var changed = false;

            var instanceRoot = PrefabUtility.FindValidUploadPrefabInstanceRoot(instence as GameObject) as GameObject;
            var prefab = PrefabUtility.GetPrefabParent(instanceRoot) as GameObject;

            if (prefab == null) return true;

            SerializedObject prefabObj = new SerializedObject(prefab.GetComponent<RectTransform>());
            SerializedObject instenceObj = new SerializedObject((instence as GameObject).GetComponent<RectTransform>());

            foreach (var item in modifyed)
            {
                if (item.propertyPath == "m_RootOrder")
                {
                    continue;
                }

                if (!coondinatePaths.Contains(item.propertyPath))
                {
                    Debug.Log("property changed:" + item.propertyPath);
                    changed = true;
                }
                else
                {
                    var intenalChanged = IsInteranlPropertyChanged(prefabObj, instenceObj, item.propertyPath);
                    if(intenalChanged)
                    {
                        changed = true;
                        Debug.Log("property changed:" + item.propertyPath);
                    }
                }
            }

            if (!changed)
            {
                Debug.Log("ignore changes of:" + instence);
            }

            return !changed;
        }


        private static bool IsInteranlPropertyChanged(SerializedObject prefab, SerializedObject instence, string path)
        {
            var change = prefab.FindProperty(path).floatValue - instence.FindProperty(path).floatValue;
            if (Mathf.Abs(change) > 0.001)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 缓存Object的SerilizedObject
        /// </summary>
        /// <param name="objectReferenceValue"></param>
        /// <returns></returns>
        public static SerializedObject CreateCachedSerializedObject(UnityEngine.Object objectReferenceValue)
        {
            if (!serializedDic.ContainsKey(objectReferenceValue) || serializedDic[objectReferenceValue] == null)
            {
                serializedDic[objectReferenceValue] = new SerializedObject(objectReferenceValue);
            }
            return serializedDic[objectReferenceValue];
        }
        /// <summary>
        /// 在指定区域绘制默认属性
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="position"></param>
        /// <param name="finalPropertyName"></param>
        public static void DrawChildInContent(SerializedProperty serializedProperty, Rect position, List<string> ignorePorps = null, string finalPropertyName = null, int level = 0)
        {
            bool enterChildren = true;
            SerializedProperty endProperty = string.IsNullOrEmpty(finalPropertyName) ? null : serializedProperty.FindPropertyRelative(finalPropertyName);
            while (serializedProperty.NextVisible(enterChildren))
            {
                if (ignorePorps != null && ignorePorps.Contains(serializedProperty.propertyPath)) continue;

                EditorGUI.indentLevel = serializedProperty.depth + level;
                position.height = EditorGUI.GetPropertyHeight(serializedProperty, null, true);
                EditorGUI.PropertyField(position, serializedProperty, true);
                position.y += position.height + 2f;
                enterChildren = false;

                if (SerializedProperty.EqualContents(serializedProperty, endProperty))
                {
                    break;
                }
            }
        }
        /// <summary>
        /// 计算高
        /// </summary>
        /// <param name="se"></param>
        /// <param name="ignorePorps"></param>
        /// <returns></returns>
        internal static float GetSerializedObjectHeight(SerializedObject se, List<string> ignorePorps = null)
        {
            var prop = se.GetIterator();
            var enterChildern = true;
            float hight = 0;
            while (prop.NextVisible(enterChildern))
            {
                enterChildern = false;
                if (ignorePorps != null && ignorePorps.Contains(prop.propertyPath))
                    continue;
                hight += EditorGUI.GetPropertyHeight(prop, null, true);
            }
            return hight;
        }
    }
}
