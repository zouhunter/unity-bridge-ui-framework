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

        public static void CreateNewViewModel(UnityAction<ViewModel> onCreate)
        {
            InitEnviroments();
            EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.zero), options, -1, OnSelect, onCreate);
        }

        private static void OnSelect(object userData, string[] options, int selected)
        {
            if (selected >= 0 && options.Length > 0)
            {
                var type = supportViewModels[selected];
                var item = ScriptableObject.CreateInstance(type);
                ProjectWindowUtil.CreateAsset(item, "new viewModel.asset");
                BridgeUI.Drawer.BridgeEditorUtility.DelyAcceptObject(item, (obj) =>
                {
                    var action = userData as UnityAction<ViewModel>;
                    if (action != null)
                    {
                        action.Invoke(obj as ViewModel);
                    }

                });
            }
        }

        private static void InitEnviroments()
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