
using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace NodeGraph
{
    public class NodeConnectionUtility
    {
        public static void Reset()
        {
            _customConnectionTypes = null;
            s_customNodes = null;
        }
        private static List<DataModel.CustomConnectionInfo> _customConnectionTypes;
        public static List<DataModel.CustomConnectionInfo> CustomConnectionTypes
        {
            get
            {
                if (_customConnectionTypes == null)
                {
                    _customConnectionTypes = BuildCustomConnectionList();
                }
                return _customConnectionTypes;
            }
        }
        private static List<DataModel.CustomConnectionInfo> BuildCustomConnectionList()
        {
            var list = new List<DataModel.CustomConnectionInfo>();

            var allNodes = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = assembly.GetTypes()
                    .Where(t => typeof(DataModel.Connection).IsAssignableFrom(t));
                allNodes.AddRange(types);
            }

            foreach (var type in allNodes)
            {
                CustomConnection attr =
                    type.GetCustomAttributes(typeof(CustomConnection), false).FirstOrDefault() as CustomConnection;

                if (attr != null)
                {
                    list.Add(new DataModel.CustomConnectionInfo(type, attr));
                }
            }

            return list;
        }
        private static List<DataModel.CustomNodeInfo> s_customNodes;
        public static List<DataModel.CustomNodeInfo> CustomNodeTypes(string group)
        {
            if (s_customNodes == null)
            {
                s_customNodes = BuildCustomNodeList(group);
            }
            return s_customNodes;
        }
        private static List<DataModel.CustomNodeInfo> BuildCustomNodeList(string group)
        {
            var list = new List<DataModel.CustomNodeInfo>();

            var allNodes = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var nodes = assembly.GetTypes()
                    .Where(t => t != typeof(DataModel.Node))
                    .Where(t => typeof(DataModel.Node).IsAssignableFrom(t));
                allNodes.AddRange(nodes);
            }

            foreach (var type in allNodes)
            {
                CustomNode attr = type.GetCustomAttributes(typeof(CustomNode), false).FirstOrDefault() as CustomNode;

                if (attr != null && attr.Group == group)
                {
                    list.Add(new DataModel.CustomNodeInfo(type, attr));
                }
            }

            list.Sort();

            return list;
        }
        public static bool HasValidCustomNodeAttribute(Type t)
        {
            CustomNode attr =
                t.GetCustomAttributes(typeof(CustomNode), false).FirstOrDefault() as CustomNode;
            return attr != null && !string.IsNullOrEmpty(attr.Name);
        }
        public static string GetNodeGUIName(DataModel.Node node)
        {
            CustomNode attr =
                node.GetType().GetCustomAttributes(typeof(CustomNode), false).FirstOrDefault() as CustomNode;
            if (attr != null)
            {
                return attr.Name;
            }
            return string.Empty;
        }
        public static string GetNodeGUIName(string className)
        {
            var type = Type.GetType(className);
            if (type != null)
            {
                CustomNode attr =
                    type.GetCustomAttributes(typeof(CustomNode), false).FirstOrDefault() as CustomNode;
                if (attr != null)
                {
                    return attr.Name;
                }
            }
            return string.Empty;
        }
        public static int GetNodeOrderPriority(string className)
        {
            var type = Type.GetType(className);
            if (type != null)
            {
                CustomNode attr =
                    type.GetCustomAttributes(typeof(CustomNode), false).FirstOrDefault() as CustomNode;
                if (attr != null){
                    return attr.OrderPriority;
                }
            }
            return CustomNode.kDEFAULT_PRIORITY;
        }
        public static DataModel.Node CreateNodeInstance(string assemblyQualifiedName)
        {
            if (assemblyQualifiedName != null)
            {
                var type = Type.GetType(assemblyQualifiedName);

                return (DataModel.Node)type.Assembly.CreateInstance(type.FullName);
            }
            return null;
        }
    }
}