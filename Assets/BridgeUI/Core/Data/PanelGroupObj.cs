using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
namespace BridgeUI.Model
{
    public class PanelGroupObj : ScriptableObject
    {
        public List<Graph.UIGraph> graphList = new List<Graph.UIGraph>();
        public bool resetMenu;
        public string menu;
        public LoadType loadType = LoadType.Prefab;
    }
}