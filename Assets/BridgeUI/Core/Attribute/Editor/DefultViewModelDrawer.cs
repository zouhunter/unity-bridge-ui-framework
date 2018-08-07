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

namespace BridgeUIEditor
{
    [CustomPropertyDrawer(typeof(DefultViewModelAttribute))]
    public class DefultViewModelDrawer : PropertyDrawer
    {
        private static List<Type> supportViewModels;
        private static GUIContent[] options;
        private static ViewModel defultviewModel;
        private static GUIContent content = new GUIContent("View Model");
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(defultviewModel == null)
            {
                defultviewModel = ScriptableObject.CreateInstance<ViewModel>();
            }

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var rect = new Rect(position.x + position.width - 60, position.y, 60, position.height);
                if (GUI.Button(rect, "new", EditorStyles.miniButtonRight))
                {
                    var target = property.serializedObject.targetObject;
                    var path = property.propertyPath;
                    CreateNewViewModel((viewModel)=> {
                        var serializeObj = new SerializedObject(target);
                        var prop = serializeObj.FindProperty(path);
                        prop.objectReferenceValue = viewModel;
                        serializeObj.ApplyModifiedProperties();
                    });
                }
                rect = new Rect(position.x, position.y, position.width - 60, position.height);
                if (property.objectReferenceValue == null)
                {
                    var viewModel = EditorGUI.ObjectField(rect, content, defultviewModel,typeof(ViewModel),false) as ViewModel;
                    if(viewModel != defultviewModel)
                    {
                        property.objectReferenceValue = viewModel;
                    }
                }
                else
                {
                    EditorGUI.PropertyField(rect, property, content);
                }
            }
        }

        private void CreateNewViewModel(UnityAction<ViewModel> onCreate)
        {
            InitEnviroments();
            EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.zero), options,-1, OnSelect, onCreate);
        }

        private void OnSelect(object userData, string[] options, int selected)
        {
            if (selected >= 0 && options.Length > 0)
            {
                var type = supportViewModels[selected];
                var item = ScriptableObject.CreateInstance(type);
                ProjectWindowUtil.CreateAsset(item, "new viewModel.asset");
                BridgeUIEditor.BridgeEditorUtility.DelyAcceptObject(item, (obj) =>
                {
                    var action = userData as UnityAction<ViewModel>;
                    if (action != null)
                    {
                        action.Invoke(obj as ViewModel);
                    }

                });
                //EditorApplication.update = () =>
                //{
                //    var path = AssetDatabase.GetAssetPath(item);
                //    if(path != null)
                //    {
                //        var obj = AssetDatabase.LoadAssetAtPath(path, type);
                //        Debug.Assert(obj != null);
                //        EditorApplication.update = null;
                       
                //    }
                //};
            }
        }

        private void InitEnviroments()
        {
            if (supportViewModels == null)
            {
                supportViewModels = (from type in typeof(ViewModel).Assembly.GetTypes()
                                     where type.IsSubclassOf(typeof(ViewModel))
                                     where !type.IsAbstract
                                     select type).ToList();
            }

            if (options == null)
            {
                options = (from model in supportViewModels
                           select new GUIContent(model.FullName)).ToArray();
            }

        }
    }

}