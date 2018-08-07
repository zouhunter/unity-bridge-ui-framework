using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Cryptography;

using NodeGraph;
using NodeGraph.DataModel;
using BridgeUI.Model;
using BridgeUI.Graph;
using BridgeUI;
namespace BridgeUIEditor
{
    public class BridgeUIGraphCtrl : NodeGraphController
    {
        private const string prefer_script_guid = "BridgeUI_PanelNames_Guid";
        public override string Group
        {
            get
            {
                return "BridgeUI";
            }
        }

        private void StoreInfoOfUIGraph(UIGraph graph)
        {
            graph.bridges.Clear();
            graph.b_nodes.Clear();
            graph.p_nodes.Clear();

            InsertBridges(graph.bridges, GetBridges());

            if (graph.loadType == LoadType.Prefab)
            {
                InsertPrefabinfo(graph.p_nodes, GetPrefabUIInfos(GetNodeInfos()));
            }
            else if (graph.loadType == LoadType.Bundle)
            {
                InsertBundleinfo(graph.b_nodes, GetBundleUIInfos(GetNodeInfos()));
            }
            EditorUtility.SetDirty(graph);
        }

        private void InsertBridges(List<BridgeInfo> source, List<BridgeInfo> newBridges)
        {
            if (source == null) return;
            if (newBridges == null) return;
            foreach (var item in newBridges)
            {
                if (string.IsNullOrEmpty(item.outNode)) continue;
                source.RemoveAll(x => (x.inNode == item.inNode || (string.IsNullOrEmpty(x.inNode) && string.IsNullOrEmpty(item.inNode))) && x.outNode == item.outNode);
                source.Add(item);
            }
        }
        private void InsertPrefabinfo(List<PrefabUIInfo> source, List<PrefabUIInfo> newInfo)
        {
            foreach (var item in newInfo)
            {
                var old = source.Find(x => x.panelName == item.panelName);
                if (old != null)
                {
                    old.prefab = item.prefab;
                    old.type = item.type;
                }
                else
                {
                    source.Add(item);
                }
            }
        }
        private void InsertBundleinfo(List<BundleUIInfo> source, List<BundleUIInfo> newInfo)
        {
            if (newInfo == null) return;
            foreach (var item in newInfo)
            {
                CompleteBundleUIInfo(item);

                var old = source.Find(x => x.panelName == item.panelName);

                if (old != null)
                {
                    old.guid = item.guid;
                    old.type = item.type;
                }
                else
                {
                    source.Add(item);
                }
            }
        }
        private List<BridgeInfo> GetBridges()
        {
            var nodes = TargetGraph.Nodes;
            var connectons = TargetGraph.Connections;
            var bridges = new List<BridgeInfo>();
            foreach (var item in connectons)
            {
                if (!(item.Object is BridgeConnection)) continue;
                var connection = item.Object as BridgeConnection;

                var bridge = new BridgeInfo();
                var innode = nodes.Find(x => x.OutputPoints != null && x.OutputPoints.Find(y => y.Id == item.FromNodeConnectionPointId) != null);
                var outnode = nodes.Find(x => x.InputPoints != null && x.InputPoints.Find(y => y.Id == item.ToNodeConnectionPointId) != null);
                if (innode != null)
                {
                    if (innode.Object is IPanelInfoHolder)
                    {
                        bridge.inNode = innode.Name;
                    }
                }

                if (outnode != null)
                {
                    bridge.outNode = outnode.Name;
                }
                bridge.viewModel = connection.viewModel;
                bridge.showModel = connection.show;
                bridge.index = connection.index;
                bridges.Add(bridge);
            }
            return bridges;
        }
        private List<NodeInfo> GetNodeInfos()
        {
            var nodeInfos = new List<NodeInfo>();
            var nodes = TargetGraph.Nodes;
            foreach (var item in nodes)
            {
                var nodeItem = item.Object as IPanelInfoHolder;
                if (nodeItem != null)
                {
                    nodeInfos.Add(nodeItem.Info);
                }
            }
            return nodeInfos;
        }
        private List<PrefabUIInfo> GetPrefabUIInfos(List<NodeInfo> infos)
        {
            var pinfos = new List<PrefabUIInfo>();
            foreach (var item in infos)
            {
                var p = new PrefabUIInfo();
                p.type = item.uiType;
                p.prefab = LoadPrefabFromGUID(item.guid);
                p.panelName = p.prefab.name;
                p.discription = item.discription;
                pinfos.Add(p);
            }
            return pinfos;
        }

        private GameObject LoadPrefabFromGUID(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (!string.IsNullOrEmpty(path))
            {
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            else
            {
                return null;
            }
        }
        private List<BundleUIInfo> GetBundleUIInfos(List<NodeInfo> infos)
        {
            var binfo = new List<BundleUIInfo>();
            foreach (var item in infos)
            {
                var p = new BundleUIInfo();
                p.type = item.uiType;
                p.guid = item.guid;
                p.discription = item.discription;
                binfo.Add(p);
            }
            return binfo;
        }
        private void CompleteBundleUIInfo(BundleUIInfo binfo)
        {
            if (string.IsNullOrEmpty(binfo.guid))
            {
                return;
            }
            else
            {
                var path = AssetDatabase.GUIDToAssetPath(binfo.guid);
                var importer = AssetImporter.GetAtPath(path);
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (importer)
                {
                    binfo.bundleName = importer.assetBundleName = string.Format(Setting.bundleNameFormat, obj.name.ToLower());
                    binfo.panelName = obj.name;
                    binfo.good = true;
                    EditorUtility.SetDirty(importer);
                }
                else
                {
                    binfo.good = false;
                }
            }
        }
        internal override void Validate(NodeGUI node)
        {
            if (node.Data.Object is IPanelInfoHolder)
            {
                var nodeItem = node.Data.Object as IPanelInfoHolder;
                if (string.IsNullOrEmpty(nodeItem.Info.guid) || LoadPrefabFromGUID(nodeItem.Info.guid) == null)
                {
                    node.ResetErrorStatus();
                }
            }
        }
        protected override void JudgeNodeExceptions(NodeGraphObj m_targetGraph, List<NodeException> m_nodeExceptions)
        {
            bool haveError = false;
            foreach (var item in TargetGraph.Nodes)
            {
                if (item.Object is IPanelInfoHolder)
                {
                    var nodeItem = item.Object as IPanelInfoHolder;
                    if (string.IsNullOrEmpty(nodeItem.Info.guid) || LoadPrefabFromGUID(nodeItem.Info.guid) == null)
                    {
                        m_nodeExceptions.Add(new NodeException("prefab is null", item.Id));
                        haveError = true;
                    }
                }
            }
            if(!haveError)
            {
                Build();
            }
        }
        protected override void BuildFromGraph(NodeGraphObj m_targetGraph)
        {
            if(m_targetGraph is BridgeUI. Graph.UIGraph)
            {
                StoreInfoOfUIGraph(m_targetGraph as BridgeUI.Graph.UIGraph);
            }

            UpdateScriptOfPanelNames(m_targetGraph.Nodes.FindAll(x => x.Object is PanelNodeBase).ConvertAll<string>(x => x.Name));
        }


        private void UpdateScriptOfPanelNames(List<string> list)
        {
            var guid = PlayerPrefs.GetString(prefer_script_guid);
            bool needOpenSelect = false;
            string directory = null;
            string path = null;
            if (!string.IsNullOrEmpty(guid))
            {
                path = AssetDatabase.GUIDToAssetPath(guid);

                if (string.IsNullOrEmpty(path))
                {
                    needOpenSelect = true;
                }
                else
                {
                    var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                    if (script == null)
                    {
                        needOpenSelect = true;
                        directory = System.IO.Path.GetDirectoryName(path);
                    }
                }
            }

            if (string.IsNullOrEmpty(path))
            {
                needOpenSelect = true;
                directory = Application.dataPath;
            }

            if (needOpenSelect)
            {
                path = EditorUtility.SaveFilePanel("«Î—°‘ÒPanelNames.cs±£¥Ê¬∑æ∂", directory, "PanelNames", "cs");
                if (!string.IsNullOrEmpty(path))
                {
                    var relePath = path.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                    Debug.Log(relePath);
                    guid = AssetDatabase.AssetPathToGUID(relePath);
                    PlayerPrefs.SetString(prefer_script_guid, guid);
                }
            }

            if (!string.IsNullOrEmpty(path))
            {
                new PanelNameGenerater(path).GenerateParcialPanelName(list.ToArray());
            }
        }
        internal override void OnDragUpdated()
        {
            base.OnDragUpdated();
            foreach (UnityEngine.Object obj in DragAndDrop.objectReferences)
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path) && obj is GameObject)
                {
                    path = GetInstenceObjectPath(obj as GameObject);
                }

                if (!string.IsNullOrEmpty(path))
                {
                    FileAttributes attr = File.GetAttributes(path);

                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory || obj is GameObject)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        break;
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        break;
                    }
                }
            }
        }
        protected static string GetInstenceObjectPath(GameObject instenceObj)
        {
            var pfbTrans = PrefabUtility.GetPrefabParent(instenceObj);
            if (pfbTrans != null)
            {
                var prefab = PrefabUtility.FindPrefabRoot(pfbTrans as GameObject);
                if (prefab != null)
                {
                    return AssetDatabase.GetAssetPath(prefab);
                }
            }
            return null;
        }
        internal override List<KeyValuePair<string, Node>> OnDragAccept(UnityEngine.Object[] objectReferences)
        {
            var nodeList = new List<KeyValuePair<string, Node>>();
            foreach (UnityEngine.Object obj in DragAndDrop.objectReferences)
            {
                var path = AssetDatabase.GetAssetPath(obj);

                if (string.IsNullOrEmpty(path) && obj is GameObject)
                {
                    path = GetInstenceObjectPath(obj as GameObject);
                }

                if (!string.IsNullOrEmpty(path))
                {
                    FileAttributes attr = File.GetAttributes(path);
                    PanelNode panelNode = null;
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        var files = System.IO.Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
                        foreach (var item in files)
                        {
                            panelNode = ScriptableObject.CreateInstance<PanelNode>();
                            panelNode.Info.guid = AssetDatabase.AssetPathToGUID(item);
                            panelNode.name = typeof(PanelNode).FullName;
                            nodeList.Add(new KeyValuePair<string, Node>(Path.GetFileNameWithoutExtension(item), panelNode));
                        }
                    }
                    else if (obj is GameObject)
                    {
                        panelNode = ScriptableObject.CreateInstance<PanelNode>();
                        var prefab = PrefabUtility.GetPrefabParent(obj);
                        if (prefab == null)
                        {
                            prefab = obj;
                        }
                        panelNode.Info.guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefab as GameObject));
                        panelNode.name = typeof(PanelNode).FullName;
                        nodeList.Add(new KeyValuePair<string, Node>(prefab.name, panelNode));
                    }
                }
            }
            return nodeList;
        }
        public override NodeGraphObj CreateNodeGraphObject()
        {
            var obj = ScriptableObject.CreateInstance<BridgeUI.Graph.UIGraph>();
            obj.ControllerType = this.GetType().FullName;
            ProjectWindowUtil.CreateAsset(obj, string.Format("new {0}.asset", obj.GetType().Name));

            return obj;
        }
        public override void SaveGraph(List<NodeData> nodes, List<ConnectionData> connections, bool resetAll = false)
        {
            //base.SaveGraph(nodes, connections, resetAll);
            UnityEngine.Assertions.Assert.IsNotNull(this);
            TargetGraph.ApplyGraph(nodes, connections);
            NodeGraphObj obj = TargetGraph;
            var allAssets = AllNeededAssets();
            SetSubAssets(allAssets, obj);
            UnityEditor.EditorUtility.SetDirty(obj);
        }

        private ScriptableObject[] AllNeededAssets()
        {
            var list = new List<ScriptableObject>();
            list.Add(TargetGraph);
            list.AddRange(TargetGraph.Nodes.ConvertAll(x => x.Object as ScriptableObject));
            list.AddRange(TargetGraph.Connections.ConvertAll(x => x.Object as ScriptableObject));
            return list.ToArray();
        }

        public static void SetSubAssets(ScriptableObject[] subAssets, ScriptableObject mainAsset)
        {
            var path = AssetDatabase.GetAssetPath(mainAsset);
            var oldAssets = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (ScriptableObject subAsset in subAssets)
            {
                if (subAsset == mainAsset) continue;

                if (System.Array.Find(oldAssets, x => x == subAsset) == null)
                {
                    if (subAsset is PanelNode)
                    {
                        ScriptableObjUtility.AddSubAsset(subAsset, mainAsset, HideFlags.None);
                    }
                    else
                    {
                        ScriptableObjUtility.AddSubAsset(subAsset, mainAsset, HideFlags.HideInHierarchy);
                    }
                }
            }

            ScriptableObjUtility.ClearSubAssets(mainAsset, subAssets);
        }
    }
}
