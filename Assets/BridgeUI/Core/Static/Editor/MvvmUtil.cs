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
        private static GUIContent[] options;


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