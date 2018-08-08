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
using NodeGraph;
using NodeGraph.DataModel;
using NodeGraph.DefultSkin;
using BridgeUI;
using BridgeUI.Graph;
using BridgeUI.Model;
using BridgeUI.CodeGen;
using System;
using System.Linq;

namespace BridgeUI.Drawer
{
    [CustomNodeView(typeof(PanelNode))]
    public class PanelNodeView : DefultSkinNodeView
    {
        protected static UIType uiTypeTemplate;
        protected static NodeType nodeTypeTemplate;
        protected PanelNodeBase panelNode { get { return target as PanelNodeBase; } }
        public override int Style
        {
            get
            {
                return panelNode.style;
            }
        }
        public override string Category
        {
            get
            {
                return "panel";
            }
        }
        public override float SuperHeight
        {
            get
            {
                if (panelNode != null && !string.IsNullOrEmpty(panelNode.Info.discription))
                {
                    return EditorGUIUtility.singleLineHeight * 0.5f;
                }
                return -EditorGUIUtility.singleLineHeight * 0.5f;
            }
        }

        public override void OnInspectorGUI(NodeGUI gui)
        {
            base.OnInspectorGUI(gui);
            if (target != null)
            {
                var node = target as PanelNode;
                node.name = node.assetName;
                gui.Name = node.name;
            }
        }

        public override void OnNodeGUI(Rect position, NodeData data)
        {
            base.OnNodeGUI(position, data);

            if (panelNode != null)
            {
                if (!string.IsNullOrEmpty(panelNode.Info.discription))
                {
                    var rect = new Rect(position.x + 20, position.y + position.height - 1.5f * EditorGUIUtility.singleLineHeight, position.width - 20,  EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(rect, panelNode.Info.discription, EditorStyles.label);
                }
            }

        }

        public override void OnClickNodeGUI(NodeGUI nodeGUI, Vector2 mousePosition, ConnectionPointData result)
        {
            base.OnClickNodeGUI(nodeGUI, mousePosition, result);
            InitPanelNodeChild(nodeGUI.Data);
            if (panelNode == null) return;
            var nodeInfo = panelNode.nodeInfo;
            var prefab = nodeInfo.GetPrefab();
            if (prefab){
                EditorGUIUtility.PingObject(prefab);
            }
        }
        
        public override void OnContextMenuGUI(GenericMenu menu, NodeGUI gui)
        {
            base.OnContextMenuGUI(menu, gui);
            menu.AddItem(new GUIContent("Copy UIType"), false, () =>
            {
                var nodeItem = (target as PanelNode);
                uiTypeTemplate = nodeItem.nodeInfo.uiType;
            });
            menu.AddItem(new GUIContent("Paste UIType"), false, () =>
            {
                var nodeItem = (target as PanelNode);
                nodeItem.nodeInfo.uiType = (UIType)uiTypeTemplate;
            });
        }

        /// <summary>
        /// 更新panelNode的贪睡
        /// </summary>
        private void InitPanelNodeChild(NodeData nodeData)
        {
            TryUpdateNodeDescribe(panelNode);
            nodeData.Object.Initialize(nodeData);
        }

        /// <summary>
        /// 更新节点信息描述
        /// </summary>
        /// <param name="node"></param>
        private static void TryUpdateNodeDescribe(Graph.PanelNodeBase node)
        {
            var guid = node.nodeInfo.guid;
            var behaivers = GetBehaiversFromGUID(guid);
            if (behaivers != null && behaivers.Length > 0)
            {
                foreach (var item in behaivers)
                {
                    var list = AnalysisBehaiver(item);
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (node.nodedescribe.Count < i + 1)
                            {
                                node.nodedescribe.Add(list[i]);
                            }

                            else
                            {
                                if (!string.IsNullOrEmpty(list[i]))
                                {
                                    node.nodedescribe[i] = list[i];
                                }
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 从脚本解析代码
        /// </summary>
        /// <returns></returns>
        private static List<string> AnalysisBehaiver(MonoBehaviour behaiver)
        {
            List<string> list = null;
            if (behaiver is IPortGroup)
            {
                var ports = (behaiver as IPortGroup).Ports;
                if (ports != null)
                {
                    list = new List<string>((behaiver as IPortGroup).Ports);
                }
            }

            var types = behaiver.GetType().GetFields(System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.FlattenHierarchy);

            var supportPorts = from type in types
                               where type.FieldType == typeof(int)
                               from att in type.GetCustomAttributes(true)
                               where att is Attributes.PortAttribute
                               let portName = (att as Attributes.PortAttribute).portInfo
                               select new KeyValuePair<int, string>((int)type.GetValue(behaiver), string.IsNullOrEmpty(portName)? type.Name:portName);

            if (supportPorts != null)
            {
                if (list == null)
                    list = new List<string>();

                foreach (var item in supportPorts)
                {
                    var span = item.Key + 1 - list.Count;

                    if (span > 0)
                    {
                        for (int i = 0; i < span; i++)
                        {
                            list.Add("");
                        }
                    }
                    if (item.Key >= 0)
                    {

                        list[item.Key] = item.Value;
                    }
                }
            }
            return list;
        }

        /// <summary>
        ///从guid解析一组MonoBehaiver
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private static MonoBehaviour[] GetBehaiversFromGUID(string guid)
        {
#if UNITY_EDITOR
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            if (!string.IsNullOrEmpty(path))
            {
                var go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var behaivers = go.GetComponents<MonoBehaviour>();
                var supportBehaiver = (from beahiver in behaivers
                                       where beahiver != null
                                       let type = beahiver.GetType()
                                       where string.IsNullOrEmpty(type.Namespace) || !(type.Namespace.StartsWith("UnityEngine."))
                                       select beahiver).ToArray();
                return supportBehaiver;
            }
#endif
            return null;
        }


    }
}