#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 11:27:06
    * 说    明：       
* ************************************************************************************/
#endregion
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
using System;
using BridgeUI.Model;

namespace BridgeUI
{
    /// <summary>
    /// 用于标记ui打开的父级
    /// [3维场景中可能有多个地方需要打开用户界面]
    /// </summary>
    public class PanelGroup : PanelGroupBase
    {
        public bool resetMenu;
        public string menu;
        public LoadType loadType = LoadType.Prefab;
        public List<BundleUIInfo> b_nodes;
        public List<PrefabUIInfo> p_nodes;
        public List<BridgeInfo> bridges;

        public override string Menu { get { return menu; } }

        public override bool ResetMenu { get { return resetMenu; } }
        public override LoadType LoadType { get { return loadType; } }

        public override List<BundleUIInfo> B_Nodes { get { return b_nodes; } }

        public override List<PrefabUIInfo> P_Nodes { get { return p_nodes; } }

        public override List<BridgeInfo> Bridges { get { return bridges; } }

        private void Awake()
        {
            InitCreater();
            RegistUINodes();
            RegistBridgePool();
            TryAutoOpen(Trans);
            RegistUIEvents();
        }
    }
}