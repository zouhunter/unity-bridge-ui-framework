using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
namespace BridgeUI.Model
{
    public class PanelGroupObj : ScriptableObject
    {
#if UNITY_EDITOR
        public List<GraphWorp> graphList = new List<GraphWorp>();
#endif
        public bool resetMenu;
        public string menu;
        public LoadType loadType = LoadType.Prefab;
        public List<BundleUIInfo> b_nodes = new List<BundleUIInfo>();
        public List<PrefabUIInfo> p_nodes = new List<PrefabUIInfo>();
        public List<BridgeInfo> bridges = new List<BridgeInfo>();
    }
}