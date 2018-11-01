using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BridgeUI.Model;


namespace BridgeUI.Drawer
{

    [CustomPropertyDrawer(typeof(BundleLoader))]
    public class BundlePanelCreaterDrawer : PropertyDrawer
    {
        private const int labelWidth = 100;
        private const int btnWidth = 60;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect0 = new Rect(position.x, position.y, labelWidth, position.height);
            var rect1 = new Rect(position.x + labelWidth, position.y, position.width - btnWidth - labelWidth, position.height);
            var rect2 = new Rect(position.x + position.width - btnWidth, position.y, btnWidth, position.height);
            EditorGUI.LabelField(rect0, "【创建规则】");
            property.objectReferenceValue = EditorGUI.ObjectField(rect1,property.objectReferenceValue,typeof(BundleLoader),false);
            var path = property.propertyPath;
            var obj = property.serializedObject.targetObject;
            if (GUI.Button(rect2,"new", EditorStyles.miniButtonRight))
            {
                BridgeUI.Drawer.BundleUtil.CreateNewBundleCreateRule(x =>
                {
                    property = new SerializedObject(obj).FindProperty(path);
                    property.objectReferenceValue = x;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
        }
    }

}