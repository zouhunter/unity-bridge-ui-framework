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
using BridgeUI.Model;
using BridgeUI;

namespace BridgeUI.Extend.AssetBundleTools
{
    /// <summary>
    /// 编辑器直接操作预制体
    /// 而运行时动态创建
    /// </summary>
    public class RuntimePanelGroup : PanelGroupBase
    {
        //面板组的guid
        public string groupGuid;
        //加载目录
        public string menu;
        //资源包名
        public string assetbundle;
        //资源名
        public string groupasset;
        //重置menu
        public bool resetMenu;
#if AssetBundleTools
        private AssetBundleLoader loader;
#endif
        private PanelGroupObj groupObj;

        public List<BridgeUI.Graph.UIGraph> GraphList
        {
            get
            {
                return groupObj.graphList;
            }
        }

        private List<BridgeInfo> bridgeInfos;
        private Dictionary<string, UIInfoBase> infoDic;
        public override List<BridgeInfo> Bridges
        {
            get
            {
                if (bridgeInfos == null)
                {
                    bridgeInfos = new List<BridgeInfo>();
                    if (GraphList != null)
                    {
                        foreach (var graph in GraphList)
                        {
                            bridgeInfos.AddRange(graph.bridges);
                        }
                    }
                }
                return bridgeInfos;
            }
        }
        public override Dictionary<string, UIInfoBase> Nodes
        {
            get
            {
                if (infoDic == null)
                {
                    infoDic = new Dictionary<string, UIInfoBase>();
                    if (GraphList != null)
                    {
                        foreach (var graph in GraphList)
                        {
                            if (graph.b_nodes != null)
                                graph.b_nodes.ForEach(x =>
                                {
                                    infoDic.Add(x.panelName, x);
                                });
                            if (graph.r_nodes != null)
                                graph.r_nodes.ForEach(x =>
                                {
                                    infoDic.Add(x.panelName, x);
                                });
                            if (graph.p_nodes != null)
                                graph.p_nodes.ForEach(x =>
                                {
                                    infoDic.Add(x.panelName, x);
                                });
                        }
                    }

                }
                return infoDic;
            }
        }

        /// <summary>
        /// 在start中调用防止线程等待（webgl）
        /// </summary>
        private void Start()
        {
            LoadPanelGroupAsync();
        }

        private void LoadPanelGroupAsync()
        {
#if AssetBundleTools
            if(resetMenu)
            {
                loader = AssetBundleLoader.GetInstance(GetUrl(), menu);
            }
            else
            {
                loader = AssetBundleLoader.Instence;
            }
            loader.LoadAssetFromUrlAsync<BridgeUI.Model.PanelGroupObj>(assetbundle, groupasset, OnLoad);
#endif
        }

        private string GetUrl()
        {
            string url =
#if UNITY_EDITOR || UNITY_STANDALONE
            "file:///" +
#endif
                Application.streamingAssetsPath + "/" + menu;
            return url;

        }

        void OnLoad(BridgeUI.Model.PanelGroupObj obj)
        {
            Debug.Log("onload:" + obj);
            this.groupObj = obj;
            if (obj)
            {
                LunchPanelGroupSystem();
            }
        }

    }
}