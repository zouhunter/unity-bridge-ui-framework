using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using System;
public partial class Utility
{
    public static void DefineLoadType()
    {
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        if (defines.Contains(_defineLoadType))
        {
            var newDefines = defines.Replace(_defineLoadType, "").Replace(";;", ";");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newDefines);
        }
    }
    public static void UnDefineLoadType()
    {
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        if (!defines.Contains(_defineLoadType))
        {
            var newDefines = defines.TrimEnd(';');
            newDefines += _defineLoadType;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newDefines);
        }
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

    public static string ShowModelToString(ShowModel show)
    {
        string str = "";
        if((show & ShowModel.Auto) == ShowModel.Auto)
        {
            str += "[a]";
        }
        if((show & ShowModel.Cover) == ShowModel.Cover)
        {
            str += "[c]";
        }
        if((show & ShowModel.Mutex) == ShowModel.Mutex)
        {
            str += "[m]";
        }
        if((show & ShowModel.HideBase) == ShowModel.HideBase)
        {
            str += "[h]";
        }
        if((show & ShowModel.Single) == ShowModel.Single)
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
#endif

public partial class Utility {
    
    public static void SetTranform(Transform item, UILayerType layer, Transform parent)
    {
        string rootName = LayerToString(layer);
        var root = parent.transform.Find(rootName);
        if (root == null)
        {
            root = new GameObject(rootName).transform;
            if (parent is RectTransform)
            {
                var rectParent = root.gameObject.AddComponent<RectTransform>();
                rectParent.anchorMin = Vector2.zero;
                rectParent.anchorMax = Vector2.one;
                rectParent.offsetMin = Vector3.zero;
                rectParent.offsetMax = Vector3.zero;
                root = rectParent;
                root.SetParent(parent, false);
            }
            else
            {
                root.SetParent(parent, true);
            }

            if (rootName.StartsWith("-1"))
            {
                root.SetAsLastSibling();
            }
            else
            {
                int i = 0;
                for (; i < parent.childCount; i++)
                {
                    var ritem = parent.GetChild(i);
                    if (ritem.name.StartsWith("-1"))
                    {
                        break;
                    }
                    if (string.Compare(rootName, ritem.name) < 0)
                    {
                        break;
                    }
                }
                root.SetSiblingIndex(i);
            }
        }
        item.transform.SetParent(root, !(item.GetComponent<Transform>() is RectTransform));
    }
    public static string LayerToString(UILayerType layer, bool showint = true)
    {
        string str = "";
        if (showint) str += (int)layer + "|";

        switch (layer)
        {
            case UILayerType.Base:
                str += "[B]";
                break;
            case UILayerType.Tip:
                str += "[M]";
                break;
            case UILayerType.Warning:
                str += "[T]";
                break;
            default:
                break;
        }
        return str;
    }
    private const string _defineLoadType = "UsePrefab";
  
}
