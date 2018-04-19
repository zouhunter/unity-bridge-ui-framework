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
#if AssetBundleTools
using AssetBundles;
namespace BridgeUI
{
    /// <summary>
    /// 编辑器直接操作预制体
    /// 而运行时动态创建
    /// </summary>
    public class RuntimePanelGroup : PanelGroupBase
    {
        public enum BundleUrl
        {
            Defult,
            HttpOrFile,
            StreamingFolder,
        }
        //面板组的guid
        public string groupGuid;
        //加载路径
        public string url;
        //加载目录
        public string menu;
        //资源加载类型
        public BundleUrl urlType;
        //资源包名
        public string asssetbundle;
        //资源名
        public string groupasset;

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
            loader = AssetBundleLoader.GetInstance(ClampUrl(), menu);
            loader.LoadAssetFromUrlAsync<Model.PanelGroupObj>(asssetbundle, groupasset, OnLoad);
        }

        private string ClampUrl()
        {
            switch (urlType)
            {
                case BundleUrl.HttpOrFile:
                    return url;
                case BundleUrl.StreamingFolder:
                    var newurl =

#if UNITY_EDITOR || UNITY_STANDALONE
                  "file:///" +
#endif
                Application.streamingAssetsPath + "/" + url;
                    return newurl;
                default:
                    return url;
            }

        }

        void OnLoad(Model.PanelGroupObj obj)
        {
            this.groupObj = obj;
            if(obj)
            {
                Debug.Log("onLoad:" + obj);
                InitCreater();
                RegistUINodes();
                RegistBridgePool();
                TryAutoOpen(Trans);
                RegistUIEvents();
            }
        }

    }
}
#endif
