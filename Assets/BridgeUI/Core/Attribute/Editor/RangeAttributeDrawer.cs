using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using BridgeUI;
namespace BridgeUIEditor
{
    [CustomPropertyDrawer(typeof(BridgeUI.RangeAttribute))]
    public class RangeAttributeDrawer : PropertyDrawer
    {
        BridgeUI.RangeAttribute range;
        const float buttonWidth = 20;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            range = attribute as BridgeUI.RangeAttribute;
            var fieldRect = new Rect(position.x, position.y, position.width - 2 * buttonWidth, position.height);
            var btnRect = new Rect(position.x + position.width - 2 * buttonWidth, position.y, buttonWidth, position.height);

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                DrawIntField(fieldRect, btnRect, property);

            }
            else if (property.propertyType == SerializedPropertyType.Float)
            {
                DrawFloatField(fieldRect, btnRect, property);
            }
        }
        private void DrawIntField(Rect fieldRect, Rect btnRect, SerializedProperty property)
        {
            EditorGUI.PropertyField(fieldRect, property);
            btnRect.x -= 1;

            if (GUI.Button(btnRect, "-", EditorStyles.miniButtonLeft))
            {
                property.intValue--;
            }
            btnRect.x += buttonWidth + 1;
            if (GUI.Button(btnRect, "+", EditorStyles.miniButtonRight))
            {
                property.intValue++;
            }
            if (property.intValue < range.min)
            {
                property.intValue = (int)range.min;
            }
            if (property.intValue > range.max)
            {
                property.intValue = (int)range.max;
            }
        }
        private void DrawFloatField(Rect fieldRect, Rect btnRect, SerializedProperty property)
        {
            EditorGUI.PropertyField(fieldRect, property);
            btnRect.x -= 1;
            if (GUI.Button(btnRect, "-", EditorStyles.miniButtonLeft))
            {
                property.floatValue -= 0.1f;
            }
            btnRect.x += buttonWidth + 1;
            if (GUI.Button(btnRect, "+", EditorStyles.miniButtonRight))
            {
                property.floatValue += 0.1f;
            }

            if (property.floatValue < range.min)
            {
                property.floatValue = range.min;
            }
            if (property.floatValue > range.max)
            {
                property.floatValue = range.max;
            }
            property.floatValue = (float)System.Math.Round(property.floatValue, 2);
        }

    }

}
