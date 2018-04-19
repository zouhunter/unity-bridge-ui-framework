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
#if AssetBundleTools
using AssetBundles;
namespace BridgeUI
{
    /// <summary>
    /// 编辑器直接操作预制体
    /// 而运行时动态创建
    /// </summary>
    public class RuntimePanelGroup : MonoBehaviour
    {
        //面板组的guid
        public string groupGuid;
        //加载路径
        public string url;
        //加载菜单
        public string menu;
        //资源包名
        public string asssetbundle;
        //资源名
        public string groupasset;

        private AssetBundleLoader loader;
        /// <summary>
        /// 在start中调用防止线程等待（webgl）
        /// </summary>
        void Start()
        {
            LoadPanelGroupAsync();
        }

        void LoadPanelGroupAsync()
        {
            loader = AssetBundleLoader.GetInstance(url, menu);
            loader.LoadAssetFromUrlAsync<Model.PanelGroupObj>(asssetbundle, groupasset, OnLoad);
        }
        string ClampUrl()
        {
            if (url.StartsWith("file:///", System.StringComparison.CurrentCulture) || url.StartsWith("http:///", System.StringComparison.CurrentCulture))
            {
                return url;
            }
            var newurl =

#if UNITY_EDITOR || UNITY_STANDALONE
                  "file:///" +
#endif
                  Application.streamingAssetsPath + url;
            return newurl;
        }

        void OnLoad(Model.PanelGroupObj obj)
        {

        }
    }

}
#endif
