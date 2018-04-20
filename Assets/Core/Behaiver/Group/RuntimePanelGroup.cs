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

namespace BridgeUI
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

        private AssetBundleLoader loader;
        private PanelGroupObj groupObj;
        public override string Menu { get { return groupObj.menu; } }

        public override bool ResetMenu { get { return groupObj.resetMenu; } }

        public override LoadType LoadType { get { return groupObj.loadType; } }

        public override List<BundleUIInfo> B_Nodes { get { return groupObj.b_nodes; } }

        public override List<PrefabUIInfo> P_Nodes { get { return groupObj.p_nodes; } }

        public override List<BridgeInfo> Bridges { get { return groupObj.bridges; } }

        /// <summary>
        /// 在start中调用防止线程等待（webgl）
        /// </summary>
        private void Start()
        {
            LoadPanelGroupAsync();
        }

        private void LoadPanelGroupAsync()
        {
            if(resetMenu)
            {
                loader = AssetBundleLoader.GetInstance(GetUrl(), menu);
            }
            else
            {
                loader = AssetBundleLoader.Instence;
            }
            loader.LoadAssetFromUrlAsync<Model.PanelGroupObj>(assetbundle, groupasset, OnLoad);
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

        void OnLoad(Model.PanelGroupObj obj)
        {
            this.groupObj = obj;
            if (obj)
            {
                LunchPanelGroupSystem();
            }
        }

    }
}
