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
using BridgeUI;
namespace BridgeUIEditor
{
    [CustomPropertyDrawer(typeof(UIType))]
    public class UITypeDrawer : PropertyDrawer
    {
        SerializedProperty formProp;
        SerializedProperty layerProp;
        SerializedProperty layerIndexProp;
        SerializedProperty hideAlaphProp;
        SerializedProperty enterAnimProp;
        SerializedProperty quitAnimProp;
        SerializedProperty hideRuleProp;
        SerializedProperty closeRuleProp;

        const float lableWidth = 120;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InitProperty(property);

            float height = 7;

            if ((HideRule)hideRuleProp.enumValueIndex == HideRule.AlaphGameObject)
            {
                height += 1;
            }

            return height * EditorGUIUtility.singleLineHeight;
        }

        private void InitProperty(SerializedProperty property)
        {
            formProp = property.FindPropertyRelative("form");
            layerProp = property.FindPropertyRelative("layer");
            layerIndexProp = property.FindPropertyRelative("layerIndex");
            hideAlaphProp = property.FindPropertyRelative("hideAlaph");
            enterAnimProp = property.FindPropertyRelative("enterAnim").FindPropertyRelative("typeName");
            quitAnimProp = property.FindPropertyRelative("quitAnim").FindPropertyRelative("typeName");
            hideRuleProp = property.FindPropertyRelative("hideRule");
            closeRuleProp = property.FindPropertyRelative("closeRule");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitProperty(property);
            var rect = new Rect(position.x, position.y + 5/**/, position.width, EditorGUIUtility.singleLineHeight);
            DrawGroup(rect, "移动机制:", (r) =>
            {
                formProp.enumValueIndex = (int)(UIFormType)EditorGUI.EnumPopup(r, (UIFormType)(formProp.enumValueIndex));
            });
            rect.y += EditorGUIUtility.singleLineHeight;
            DrawGroup(rect, "绝对显示:", (r) =>
            {
                layerProp.enumValueIndex = (int)(UILayerType)EditorGUI.EnumPopup(r, (UILayerType)layerProp.enumValueIndex);
            });
            rect.y += EditorGUIUtility.singleLineHeight;
            DrawGroup(rect, "相对优先:", (r) =>
            {
                layerIndexProp.intValue = EditorGUI.IntField(r, layerIndexProp.intValue);
            });
            rect.y += EditorGUIUtility.singleLineHeight;
            DrawGroup(rect, "隐藏方式:", (r) =>
            {
                hideRuleProp.enumValueIndex = (int)(HideRule)EditorGUI.EnumPopup(r, (HideRule)hideRuleProp.enumValueIndex);
            });
            if ((HideRule)hideRuleProp.enumValueIndex == HideRule.AlaphGameObject)
            {
                rect.y += EditorGUIUtility.singleLineHeight;
                DrawGroup(rect, "隐藏透明:", (r) =>
                {
                    hideAlaphProp.floatValue = EditorGUI.Slider(r, hideAlaphProp.floatValue, 0, 1);
                });
            }
            rect.y += EditorGUIUtility.singleLineHeight;
            DrawGroup(rect, "关闭方式:", (r) =>
            {
                closeRuleProp.enumValueIndex = (int)(CloseRule)EditorGUI.EnumPopup(r, (CloseRule)closeRuleProp.enumValueIndex);
            });
            rect.y += EditorGUIUtility.singleLineHeight;
            DrawGroup(rect, "出场动画:", (r) =>
            {
                EditorGUI.LabelField(r,enterAnimProp.stringValue);
            });
            rect.y += EditorGUIUtility.singleLineHeight;
            DrawGroup(rect, "闭场动画:", (r) =>
            {
                EditorGUI.LabelField(r, quitAnimProp.stringValue);
            });
        }

        private void DrawGroup(Rect rect, string label, UnityAction<Rect> drawBody)
        {
            var labelRect = new Rect(rect.x, rect.y, rect.width * 0.3f, rect.height);
            var bodyRect = new Rect(rect.x + rect.width * 0.4f, rect.y, rect.width * 0.6f, rect.height);
            EditorGUI.LabelField(labelRect, label, EditorStyles.largeLabel);
            drawBody(bodyRect);
        }
    }
}