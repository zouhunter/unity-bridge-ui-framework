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
using BridgeUI.Binding;
using System;
using System.Linq;
using BridgeUI.Attributes;

namespace BridgeUI.Drawer
{

    [CustomPropertyDrawer(typeof(DefultViewModelAttribute),true)]
    public class DefultViewModelDrawer : PropertyDrawer
    {
        private static GUIContent content = new GUIContent("IViewModel");
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var hide = (attribute as DefultViewModelAttribute).hide;
            if(hide && property.objectReferenceValue == null)
            {
                return 0;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var hide = (attribute as DefultViewModelAttribute).hide;

            if (hide && property.objectReferenceValue == null)
            {
                return;
            }

            if (!(property.objectReferenceValue is Binding.IViewModel))
            {
                property.objectReferenceValue = null;
            }

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var rect = new Rect(position.x + position.width - 60, position.y, 60, position.height);
                if (GUI.Button(rect, "new", EditorStyles.miniButtonRight))
                {
                    var target = property.serializedObject.targetObject;
                    var path = property.propertyPath;

                    MvvmUtil.CreateAssets(typeof(ViewModelContainer), (viewModel) =>
                    {
                        var serializeObj = new SerializedObject(target);
                        var prop = serializeObj.FindProperty(path);
                        prop.objectReferenceValue = viewModel;
                        serializeObj.ApplyModifiedProperties();
                    });
                }

                rect = new Rect(position.x, position.y, position.width - 60, position.height);

                if (property.objectReferenceValue != null)
                {
                    EditorGUI.PropertyField(rect, property, content);
                }
            }
        }
    }

}