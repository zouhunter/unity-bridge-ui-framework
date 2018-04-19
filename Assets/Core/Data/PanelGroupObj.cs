using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
namespace BridgeUI.Model
{
    public class PanelGroupObj : ScriptableObject
    {
#if UNITY_EDITOR
        public List<GraphWorp> graphList;
#endif
        public bool resetMenu;
        public string menu;
        public LoadType loadType = LoadType.Prefab;
        public List<BundleUIInfo> b_nodes;
        public List<PrefabUIInfo> p_nodes;
        public List<BridgeInfo> bridges;
    }
}