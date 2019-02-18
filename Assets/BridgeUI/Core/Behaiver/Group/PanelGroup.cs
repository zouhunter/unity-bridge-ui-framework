#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 11:27:06
    * 说    明：       
* ************************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using BridgeUI.Graph;
using BridgeUI.Model;

namespace BridgeUI
{
    /// <summary>
    /// 用于标记ui打开的父级
    /// [3维场景中可能有多个地方需要打开用户界面]
    /// </summary>
    public class PanelGroup : PanelGroupBase
    {
        [UnityEngine.SerializeField]
        private List<string> graphList;
        [UnityEngine.SerializeField]
        private List<BridgeInfo> bridges;
        [UnityEngine.SerializeField]
        private List<Model.PrefabUIInfo> p_nodes;
        [UnityEngine.SerializeField]
        private List<Model.ResourceUIInfo> r_nodes;
        [UnityEngine.SerializeField]
        private List<Model.BundleUIInfo> b_nodes;
        
        private Dictionary<string, UIInfoBase> infoDic;

        public override List<BridgeInfo> Bridges
        {
            get
            {
                return bridges;
            }
        }
        public override Dictionary<string, UIInfoBase> Nodes
        {
            get
            {
                if (infoDic == null)
                {
                    infoDic = new Dictionary<string, UIInfoBase>();
                    if (b_nodes != null)
                        b_nodes.ForEach(x =>
                        {
                            infoDic.Add(x.panelName, x);
                        });
                    if (r_nodes != null)
                        r_nodes.ForEach(x =>
                        {
                            infoDic.Add(x.panelName, x);
                        });
                    if (p_nodes != null)
                        p_nodes.ForEach(x =>
                        {
                            infoDic.Add(x.panelName, x);
                        });
                }
                return infoDic;
            }
        }
        public List<Model.PrefabUIInfo> P_nodes
        {
            get
            {
                if (p_nodes == null)
                {
                    p_nodes = new List<PrefabUIInfo>();
                }
                return p_nodes;
            }
        }
        public List<Model.BundleUIInfo> B_nodes
        {
            get
            {
                if (b_nodes == null)
                {
                    b_nodes = new List<BundleUIInfo>();
                }
                return b_nodes;
            }
        }
        public List<Model.ResourceUIInfo> R_nodes
        {
            get
            {
                if (r_nodes == null)
                {
                    r_nodes = new List<ResourceUIInfo>();
                }
                return r_nodes;
            }
        }
        public List<string> GraphList
        {
            get
            {
                if (graphList == null)
                {
                    graphList = new List<string>();
                }
                return graphList;
            }
        }

        protected override void Awake()
        {
            UnityEngine.Debug.Log("PanelGroup.Awake");
            base.Awake();
            LunchPanelGroupSystem();
        }

        public string GetGraphAtIndex(int id)
        {
            if (graphList != null && graphList.Count > id && id >= 0)
            {
                return graphList[id];
            }
            return null;
        }
    }
}