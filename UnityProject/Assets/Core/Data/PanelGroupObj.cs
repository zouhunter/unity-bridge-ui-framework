using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PanelGroupObj : ScriptableObject
{
#if UNITY_EDITOR
    public List<GraphWorp> graphList;
#endif
    public LoadType loadType = LoadType.Prefab;
    public List<BundleUIInfo> b_nodes;
    public List<PrefabUIInfo> p_nodes;
    public List<Bridge> bridges;

    internal void RegistUINodes(List<UIInfoBase> activeNodes, List<Bridge> bridges)
    {
        if ((loadType & LoadType.Prefab) == LoadType.Prefab)
        {
            activeNodes.AddRange(b_nodes.ConvertAll<UIInfoBase>(x => x));
        }
        if ((loadType & LoadType.Prefab) == LoadType.Prefab)
        {
            activeNodes.AddRange(p_nodes.ConvertAll<UIInfoBase>(x => x));
        }
        bridges.AddRange(this.bridges);
    }
}
