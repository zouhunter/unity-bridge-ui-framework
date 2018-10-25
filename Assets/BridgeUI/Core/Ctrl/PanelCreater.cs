using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BridgeUI.Model;
using System;

namespace BridgeUI
{
    public class PanelCreater : IPanelCreater
    {
#if AssetBundleTools
        private AssetBundleLoader assetLoader;
#endif
        private List<string> _loadingKeys = new List<string>();
        private List<string> _cansaleKeys = new List<string>();

        public PanelCreater()
        {
#if AssetBundleTools
            assetLoader = AssetBundleLoader.Instence;
#endif
        }
        public PanelCreater(string menu)
        {
#if AssetBundleTools

            var url =
#if UNITY_WEBGL
           
#else
            "file:///" +
#endif
            Application.streamingAssetsPath + "/" + menu;

            assetLoader = AssetBundleLoader.GetInstance(url, menu);
#endif
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panelName"></param>
        /// <param name="onCreate"></param>
        public void CreatePanel(UIInfoBase itemInfo, UnityEngine.Events.UnityAction<GameObject> onCreate)
        {
            if (_cansaleKeys.Contains(itemInfo.panelName)) _cansaleKeys.RemoveAll(x => x == itemInfo.panelName);

            if (!_loadingKeys.Contains(itemInfo.IDName))
            {
                _loadingKeys.Add(itemInfo.IDName);
                var bInfo = itemInfo as BundleUIInfo;
                var pInfo = itemInfo as PrefabUIInfo;

                if (bInfo != null)
                {
                    GetGameObjectInfo(bInfo,onCreate);
                }
                else if (pInfo != null)
                {
                    var go = GetGameObjectInfo(pInfo);
                    if (onCreate != null) onCreate.Invoke(go);
                }
            }
        }

        /// <summary>
        /// BundleUINode创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panelName"></param>
        /// <param name="onCreate"></param>
        private void GetGameObjectInfo(BundleUIInfo itemInfo, UnityEngine.Events.UnityAction<GameObject> onCreate)
        {
#if AssetBundleTools
            var trigger = itemInfo as BundleUIInfo;
            assetLoader.LoadAssetFromUrlAsync<GameObject>(trigger.bundleName, trigger.panelName, (x) =>
            {
                if (x != null)
                {
                   var instence = CreateInstance(x, trigger);
                    _loadingKeys.Remove(trigger.IDName);
                    if (onCreate != null) onCreate(instence);
                }
                else
                {
                    Debug.Log(trigger.bundleName + ".." + trigger.panelName + "-->空");
                }
            });
#endif
        }
        /// <summary>
        /// PrefabUINode创建对象
        /// </summary>
        /// <param name="iteminfo"></param>
        private GameObject GetGameObjectInfo(PrefabUIInfo iteminfo)
        {
            var trigger = iteminfo as PrefabUIInfo;

            if (trigger.prefab != null)
            {
                var instence = CreateInstance(trigger.prefab, trigger);
                _loadingKeys.Remove(trigger.IDName);
                return instence;
            }
            else
            {
                Debug.Log(trigger.panelName + "-->空");
                return null;
            }
        }
        /// <summary>
        /// 获取对象实例
        /// </summary>
        private GameObject CreateInstance(GameObject prefab, UIInfoBase nodeInfo)
        {
            if (_cansaleKeys.Contains(nodeInfo.panelName))
            {
                _cansaleKeys.Remove(nodeInfo.panelName);
                return null;
            }

            if (prefab == null || nodeInfo == null)
            {
                return null;
            }

            GameObject go = GameObject.Instantiate(prefab);

            go.name = nodeInfo.panelName;

            go.SetActive(true);

            return go;
        }

        /// <summary>
        /// 取消创建对象
        /// </summary>
        /// <param name="panelName"></param>

        public void CansaleCreatePanel(string panelName)
        {
            _cansaleKeys.Add(panelName);
        }
    }
}