using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeGraph;
using NodeGraph.DataModel;
namespace BridgeUI.Graph
{
    public class UIGraph : NodeGraph.DataModel.NodeGraphObj
    {
        public LoadType loadType = LoadType.DirectLink;
        public List<Model.BundleUIInfo> b_nodes = new List<Model.BundleUIInfo>();
        public List<Model.PrefabUIInfo> p_nodes = new List<Model.PrefabUIInfo>();
        public List<Model.ResourceUIInfo> r_nodes = new List<Model.ResourceUIInfo>();
        public List<Model.BridgeInfo> bridges = new List<Model.BridgeInfo>();
    }

}