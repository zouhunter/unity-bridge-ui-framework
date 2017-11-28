using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
[CustomPropertyDrawer(typeof(UIType))]
public class UITypeDrawer : PropertyDrawer {
    //SerializedProperty mutexKeyProp;
    SerializedProperty formProp;
    SerializedProperty layerProp;
    SerializedProperty layerIndexProp;
    SerializedProperty hideAlaphProp;
    SerializedProperty animTypeProp;

    const float lableWidth = 120;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label);
    }

    private void InitProperty(SerializedProperty property)
    {
        //mutexKeyProp = property.FindPropertyRelative("mutexKey");
        formProp = property.FindPropertyRelative("form");
        layerProp = property.FindPropertyRelative("layer");
        layerIndexProp = property.FindPropertyRelative("layerIndex");
        hideAlaphProp = property.FindPropertyRelative("hideAlaph");
        animTypeProp = property.FindPropertyRelative("animType");
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        InitProperty(property);
        //using (var hor = new EditorGUILayout.HorizontalScope())
        //{
        //    EditorGUILayout.LabelField("唯一关键字:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
        //    mutexKeyProp.stringValue = EditorGUILayout.TextField(mutexKeyProp.stringValue);
        //}
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("可移动机制:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            formProp.enumValueIndex = (int)(UIFormType)EditorGUILayout.EnumPopup((UIFormType)(formProp.enumValueIndex));
        }
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("绝对显示层:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            layerProp.enumValueIndex = (int)(UILayerType)EditorGUILayout.EnumPopup((UILayerType)layerProp.enumValueIndex);
        }
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("相对优先级:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            layerIndexProp.intValue = EditorGUILayout.IntField(layerIndexProp.intValue);
        }
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("隐藏透明度:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            hideAlaphProp.floatValue = EditorGUILayout.Slider(hideAlaphProp.floatValue, 0, 1);
        }
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("出场动画组:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            animTypeProp.enumValueIndex = (int)(UIAnimType)EditorGUILayout.EnumPopup((UIAnimType)animTypeProp.enumValueIndex);
        }
    }
}
