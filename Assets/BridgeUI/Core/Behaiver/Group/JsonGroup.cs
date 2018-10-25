using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BridgeUI.Graph;
using BridgeUI;
using System;
using NodeGraph;

namespace BridgeUI
{
    public class JsonGroup : PanelGroupBase
    {
        public enum ResourceType
        {
            OriginLink,
            StreamingFile,
            WebFile,
#if AssetBundleTools
            AssetBundle,
#endif
            Resource,
        }

        [HideInInspector]
        public ResourceType resourceType;
        [HideInInspector]
        public TextAsset graphText;//直接关联加载
        [HideInInspector]
        public string streamingPath;//streamingasset路径加载
        [HideInInspector]
        public string webUrl;//网络路径
        [HideInInspector]
        public string assetBundleName;//资源包
        [HideInInspector]
        public string assetName;//资源名
        [HideInInspector]
        public bool resetMenu;//自定义菜单
        [HideInInspector]
        public string menu;//菜单
        [HideInInspector]
        public string resourcePath;
        private List<UIGraph> _graphList;
        protected PanelCreater _panelCreater;
        protected override IPanelCreater creater
        {
            get
            {
                if (_panelCreater == null)
                {
                    if (ResetMenu)
                    {
                        _panelCreater = new PanelCreater(Menu);
                    }
                    else
                    {
                        _panelCreater = new PanelCreater();
                    }
                }
                return _panelCreater;
            }
        }

        public override List<UIGraph> GraphList
        {
            get
            {
                return _graphList;
            }
        }

        public override string Menu
        {
            get
            {
                return menu;
            }
        }

        public override bool ResetMenu
        {
            get
            {
                return resetMenu;
            }
        }

        private void Awake()
        {
            StartCoroutine(DelyLoadGraphList());
        }

        private IEnumerator DelyLoadGraphList()
        {
            switch (resourceType)
            {
                case ResourceType.OriginLink:
                    LoadGraphAndLunch(graphText.text);
                    break;
                case ResourceType.StreamingFile:
                    var url =
#if UNITY_EDITOR || UNITY_STANDALONE
                    "file:///" +
#endif
                    Application.streamingAssetsPath + "/" + streamingPath;
                    yield return LoadText(url, LoadGraphAndLunch);
                    break;
                case ResourceType.WebFile:
                    yield return LoadText(webUrl, LoadGraphAndLunch);
                    break;
#if AssetBundleTools
                case ResourceType.AssetBundle:
                    LoadFromBundle(LoadGraphAndLunch);
                    break;
#endif
                case ResourceType.Resource:
                    LoadGraphAndLunch(Resources.Load<TextAsset>(resourcePath).text);
                    break;
                default:
                    break;
            }
        }
#if AssetBundleTools

        private void LoadFromBundle(Action<string> onOk)
        {
            AssetBundleLoader loader = null;
            if (string.IsNullOrEmpty(menu))
            {
                loader = AssetBundleLoader.Instence;
            }
            else
            {
                string url =
#if UNITY_EDITOR || UNITY_STANDALONE
                    "file:///" +
#endif
                    Application.streamingAssetsPath + "/" + menu;

                loader = AssetBundleLoader.GetInstance(url, menu);
            }
            loader.LoadAssetFromUrlAsync<TextAsset>(assetBundleName, assetName, (asset) =>
            {
                if (asset != null)
                {
                    onOk(asset.text);
                }
            });
        }
#endif

        private IEnumerator LoadText(string url, Action<string> onOK)
        {
            WWW request = new WWW(url);
            yield return request;
            if (request.error != null)
            {
                Debug.LogError(request.error);
            }
            else
            {
                onOK(request.text);
            }
        }

        private void LoadGraphAndLunch(string json)
        {
            _graphList = NodeGraph.SerializeUtil.DeserializeGraphs<UIGraph>(json);
            if (_graphList != null)
            {
                LunchPanelGroupSystem();
            }
        }
    }
}