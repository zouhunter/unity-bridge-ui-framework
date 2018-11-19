using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
using System;
using BridgeUI.Binding;
using System.Linq;

namespace BridgeUI.Drawer
{
    public static class MvvmUtil
    {
        private static List<Type> supportViewModels;
        private static GUIContent[] options;

        public static void CreateNewViewModel(UnityAction<ScriptableObject> onCreate)
        {
            InitEnviroments();
            EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.zero), options, -1, OnSelect, onCreate);
        }

        private static void OnSelect(object userData, string[] options, int selected)
        {
            if (selected >= 0 && options.Length > 0)
            {
                var type = supportViewModels[selected];
                CreateAssets(type, userData as UnityAction<ScriptableObject>);
            }
        }

        private static void InitEnviroments()
        {
            if (supportViewModels == null)
            {
                supportViewModels = GetSubInstenceTypes(typeof(ViewModelObject));
                supportViewModels.Insert(0, typeof(ViewModelContainer));
            }

            if (options == null)
            {
                options = (from model in supportViewModels
                           select new GUIContent(model.FullName)).ToArray();
            }

        }

        public static void CreateAssets(Type type, UnityAction<ScriptableObject> action)
        {
            var item = ScriptableObject.CreateInstance(type);
            ProjectWindowUtil.CreateAsset(item,type.Name + ".asset");
            BridgeUI.Drawer.BridgeEditorUtility.DelyAcceptObject(item, (obj) =>
            {
                if (action != null){
                    action.Invoke(obj as ScriptableObject);
                }

            });
        }

        public static List<Type> GetSubInstenceTypes(Type rootType)
        {
            var types = (from type in rootType.Assembly.GetTypes()
                                 where type.IsSubclassOf(rootType)
                                 where !type.IsAbstract
                                 select type).ToList();
            return types;
        }
    }
}