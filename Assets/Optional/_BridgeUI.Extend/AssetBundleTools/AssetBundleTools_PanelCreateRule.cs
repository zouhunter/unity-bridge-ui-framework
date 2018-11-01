using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
using BridgeUI.Model;
using System;
using UnityEngine.Events;

namespace BridgeUI.Extend.AssetBundleTools
{
    public class AssetBundleTools_PanelCreateRule : BundleLoader
    {
        public bool resetMenu;
        public string menu;
        private AssetBundleLoader assetLoader;
        public override void InitEnviroment()
        {
            if (!resetMenu)
            {
                assetLoader = AssetBundleLoader.Instence;
            }
            else
            {

                var url =
#if UNITY_WEBGL
           
#else
            "file:///" +
#endif
            Application.streamingAssetsPath + "/" + menu;

                assetLoader = AssetBundleLoader.GetInstance(url, menu);
            }
        }

        /// <summary>
        /// BundleUINode创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        /// <param name="onLoad"></param>
        public override void LoadAssetAsync<T>(string assetBundleName, string assetName, UnityAction<T> onLoad) 
        {
            assetLoader.LoadAssetFromUrlAsync<T>(assetBundleName, assetName, (x) =>
            {
                if (x != null)
                {
                    if (onLoad != null) onLoad(x);
                }
                else
                {
                    Debug.Log(assetBundleName + ".." + assetName + "-->空");
                }
            });
        }
    }
}