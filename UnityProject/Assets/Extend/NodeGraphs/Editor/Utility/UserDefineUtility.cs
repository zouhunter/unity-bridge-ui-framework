using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace NodeGraph
{

    public class UserDefineUtility
    {
        private static List<Type> _controllerTypes;
        public static List<Type> CustomControllerTypes
        {
            get
            {
                if (_controllerTypes == null)
                {
                    _controllerTypes = BuildControllerTypeList();
                }
                return _controllerTypes;
            }
        }
        private static List<Type> BuildControllerTypeList()
        {
            var list = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var nodes = assembly.GetTypes()
                    .Where(t => t != typeof(NodeGraphController))
                    .Where(t => typeof(NodeGraphController).IsAssignableFrom(t));
                list.AddRange(nodes);
            }
            return list;
        }

        internal static NodeGraphController CreateController(NodeGraph.DataModel.NodeGraphObj graph)
        {
            var type = CustomControllerTypes.Find(x => x.Name == graph.ControllerType);
            if (type != null)
            {
                var ctrl = System.Activator.CreateInstance(type);
                var gctrl = ctrl as NodeGraphController;
                gctrl.TargetGraph = graph;
                return gctrl;
            }
            else
            {
                return null;
            }
        }
        private static Dictionary<Type, Type> userDrawer;
        internal static object GetUserDrawer(Type type)
        {
            InitDrawerTypes();
            if (userDrawer.ContainsKey(type))
            {
                var drawer = Activator.CreateInstance(userDrawer[type]);
                return drawer;
            }
            return null;
        }
        private static void InitDrawerTypes()
        {
            if (userDrawer == null)
            {
                userDrawer = new Dictionary<Type, Type>();
                var allDrawer = new List<Type>();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var nodes = assembly.GetTypes()
                        .Where(t => t != typeof(NodeDrawer) && t != typeof(ConnectionDrawer))
                        .Where(t => typeof(NodeDrawer).IsAssignableFrom(t) || typeof(ConnectionDrawer).IsAssignableFrom(t));
                    allDrawer.AddRange(nodes);
                }
                foreach (var type in allDrawer)
                {
                    CustomNodeGraphDrawer attr = type.GetCustomAttributes(typeof(CustomNodeGraphDrawer), false).FirstOrDefault() as CustomNodeGraphDrawer;

                    if (attr != null)
                    {
                        foreach (var item in attr.targetTypes)
                        {
                            userDrawer.Add(item, type);
                        }
                    }
                }
            }
        }

        public static void DrawSerializedObject(SerializedObject serializedObject)
        {
            if (serializedObject == null || serializedObject.targetObject == null) return;

            serializedObject.Update();
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                EditorGUI.BeginDisabledGroup("m_Script" == iterator.propertyPath);
                EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                EditorGUI.EndDisabledGroup();
                enterChildren = false;
            }
            serializedObject.ApplyModifiedProperties();
        }

    }

}