using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Cryptography;

using Model = NodeGraph.DataModel.Version2;

namespace NodeGraph
{
    public class NodeGraphController
    {

        private List<NodeException> m_nodeExceptions;
        private Model.ConfigGraph m_targetGraph;

        public bool IsAnyIssueFound
        {
            get
            {
                return m_nodeExceptions.Count > 0;
            }
        }

        public List<NodeException> Issues
        {
            get
            {
                return m_nodeExceptions;
            }
        }

        public Model.ConfigGraph TargetGraph
        {
            get
            {
                return m_targetGraph;
            }
        }


        public NodeGraphController(Model.ConfigGraph graph)
        {
            m_targetGraph = graph;
            m_nodeExceptions = new List<NodeException>();
        }
        public void Perform()
        {
            LogUtility.Logger.Log(LogType.Log, "---Setup BEGIN---");

            foreach (var e in m_nodeExceptions)
            {
                var errorNode = m_targetGraph.Nodes.Find(n => n.Id == e.Id);
                // errorNode may not be found if user delete it on graph
                if (errorNode != null)
                {
                    LogUtility.Logger.LogFormat(LogType.Log, "[Perform] {0} is marked to revisit due to last error", errorNode.Name);
                    errorNode.NeedsRevisit = true;
                }
            }

            m_nodeExceptions.Clear();

            foreach (var item in TargetGraph.Nodes)
            {
                if (item.Operation.Object is IPanelInfoHolder)
                {
                    var nodeItem = item.Operation.Object as IPanelInfoHolder;
                    var guid = nodeItem.Info.prefabGuid;
                    if (string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(guid)))
                    {
                        m_nodeExceptions.Add(new NodeException("prefab is null", item.Id));
                    }
                }
            }

            LogUtility.Logger.Log(LogType.Log, "---Setup END---");
        }
        internal void BuildToSelect()
        {
            if (Selection.activeGameObject != null)
            {
                var panelGroup = Selection.activeGameObject.GetComponent<PanelGroup>();
                if (panelGroup != null)
                {
                    StoreInfoOfPanel(panelGroup);
                }
            }
        }

        /// <summary>
        /// 将信息到保存到PanelGroup
        /// </summary>
        /// <param name="group"></param>
        private void StoreInfoOfPanel(PanelGroup group)
        {
            InsertBridges(group.bridges, GetBridges());
            if (group.loadType == LoadType.Prefab)
            {
                InsertPrefabinfo(group.p_nodes, GetPrefabUIInfos(GetNodeInfos()));
            }
            else if (group.loadType == LoadType.Bundle)
            {
                InsertBundleinfo(group.b_nodes, GetBundleUIInfos(GetNodeInfos()));
            }
            TryRecoredGraphGUID(group);
            EditorUtility.SetDirty(group);
        }

        private void TryRecoredGraphGUID(PanelGroup group)
        {
            var path = AssetDatabase.GetAssetPath(TargetGraph);
            var guid = AssetDatabase.AssetPathToGUID(path);
            if (group is PanelGroup)
            {
                var panelGroup = group as PanelGroup;
                var record = panelGroup.graphList.Find(x => x.guid == guid);
                if (record == null)
                {
                    var item = new GraphWorp(TargetGraph.name, guid);
                    panelGroup.graphList.Add(item);
                }
                else
                {
                    record.graphName = TargetGraph.name;
                }
            }
        }

        private void InsertBridges(List<Bridge> source, List<Bridge> newBridges)
        {
            if (newBridges == null) return;
            foreach (var item in newBridges)
            {
                var old = source.Find(x => (x.inNode == item.inNode||(x.inNode == "" && item.inNode == "")) && x.outNode == item.outNode);
                if (old != null)
                {
                    old.showModel = item.showModel;
                }
                else
                {
                    source.Add(item);
                }
            }
        }
        private void InsertPrefabinfo(List<PrefabUIInfo> source, List<PrefabUIInfo> newInfo)
        {
            if (newInfo == null) return;
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
        private List<Bridge> GetBridges()
        {
            var nodes = TargetGraph.Nodes;
            var connectons = TargetGraph.Connections;
            var bridges = new List<Bridge>();
            foreach (var item in connectons)
            {
                var bridge = new Bridge();
                var innode = nodes.Find(x => x.OutputPoints != null && x.OutputPoints.Find(y => y.Id == item.FromNodeConnectionPointId) != null);
                var outnode = nodes.Find(x => x.InputPoints != null && x.InputPoints.Find(y => y.Id == item.ToNodeConnectionPointId) != null);
                if (innode != null)
                {
                    if (innode.Operation.Object is IPanelInfoHolder)
                    {
                        bridge.inNode = innode.Name;
                    }
                }


                if (outnode != null && outnode.Operation.Object is IPanelInfoHolder)
                {
                    bridge.outNode = outnode.Name;
                }
                bridge.showModel = item.Show;
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
                var nodeItem = item.Operation.Object as IPanelInfoHolder;
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
                SwitchInfoFromNodeInfo(p, item);
                p.prefab = LoadPrefabFromGUID(item.prefabGuid);
                p.panelName = p.prefab.name;
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
                SwitchInfoFromNodeInfo(p, item);
                p.guid = item.prefabGuid;
                binfo.Add(p);
            }
            return binfo;
        }

        private void SwitchInfoFromNodeInfo(UIInfoBase p, NodeInfo item)
        {
            p.type = new global::UIType();
            p.type.form = item.form;
            p.type.layer = item.layer;
            p.type.hideLuceny = item.hideLuceny;
            p.type.mutexKey = item.mutexKey;
            p.type.layerIndex = item.layerIndex;
            p.type.animType = item.animType;
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
                    binfo.bundleName = importer.assetBundleName;
                    binfo.panelName = obj.name;
                    binfo.good = true;
                }
                else
                {
                    binfo.good = false;
                }
            }
        }

    }
}
