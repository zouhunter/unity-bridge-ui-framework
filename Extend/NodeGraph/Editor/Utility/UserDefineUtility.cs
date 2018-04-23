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
        private static Dictionary<Type, Type> userDrawer;

        internal static NodeGraphController CreateController(NodeGraph.DataModel.NodeGraphObj graph)
        {
            var type = CustomControllerTypes.Find(x => x.FullName == graph.ControllerType);
            if (type != null)
            {
                var ctrl = System.Activator.CreateInstance(type);
                var gctrl = ctrl as NodeGraphController;
                gctrl.TargetGraph = graph;
                return gctrl;
            }
            else
            {
                Debug.LogError("can not find controllerType:" + graph.ControllerType);
                return null;
            }
        }
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
                        .Where(t => t != typeof(NodeView) && t != typeof(ConnectionView))
                        .Where(t => typeof(NodeView).IsAssignableFrom(t) || typeof(ConnectionView).IsAssignableFrom(t));
                    allDrawer.AddRange(nodes);
                }
                foreach (var type in allDrawer)
                {
                    CustomNodeView attr = type.GetCustomAttributes(typeof(CustomNodeView), false).FirstOrDefault() as CustomNodeView;

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
    }

}