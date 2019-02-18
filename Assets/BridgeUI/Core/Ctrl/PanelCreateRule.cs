using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BridgeUI.Model
{
    public class PanelCreateRule: IPanelCreater
    {
        protected List<string> _loadingKeys = new List<string>();
        protected List<string> _cansaleKeys = new List<string>();
        protected BundleLoader bundleLoader;

        public PanelCreateRule(BundleLoader bundlePanelCreater)
        {
            this.bundleLoader = bundlePanelCreater;
            if (bundlePanelCreater != null)
                bundlePanelCreater.InitEnviroment();
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panelName"></param>
        /// <param name="onCreate"></param>
        public void CreatePanel(UIInfoBase itemInfo, UnityEngine.Events.UnityAction<GameObject> onCreate)
        {
            if (_cansaleKeys.Contains(itemInfo.panelName))
                _cansaleKeys.RemoveAll(x => x == itemInfo.panelName);

            if (!_loadingKeys.Contains(itemInfo.IDName))
            {
                _loadingKeys.Add(itemInfo.IDName);
                var bInfo = itemInfo as BundleUIInfo;
                var pInfo = itemInfo as PrefabUIInfo;
                var rInfo = itemInfo as ResourceUIInfo;

                if (pInfo != null)
                {
                    var go = GetGameObjectInfo(pInfo);
                    if (onCreate != null) onCreate.Invoke(go);
                }
                else if(rInfo!= null)
                {
                    var go = GetGameObjectInfo(rInfo);
                    if (onCreate != null) onCreate.Invoke(go);
                }
                else if (bInfo != null)
                {
                    if (bundleLoader != null)
                    {
                        bundleLoader.LoadAssetAsync<GameObject>(bInfo.bundleName,bInfo.panelName, (x)=> {
                            if(x != null)
                            {
                                var instence = CreateInstance(x, bInfo);
                                if (onCreate != null) onCreate.Invoke(instence);
                                _loadingKeys.Remove(bInfo.IDName);
                            }
                        });
                    }
                    else
                    {
                        Debug.LogError("请先设置AssetBundle加载时的规则！");
                    }
                }
            }
        }

        /// <summary>
        /// 取消创建对象
        /// </summary>
        /// <param name="panelName"></param>
        public void CansaleCreatePanel(string panelName)
        {
            _cansaleKeys.Add(panelName);
        }

        /// <summary>
        /// PrefabUINode创建对象
        /// </summary>
        /// <param name="iteminfo"></param>
        protected GameObject GetGameObjectInfo(PrefabUIInfo iteminfo)
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
        /// ResourceUINode创建对象
        /// </summary>
        /// <param name="iteminfo"></param>
        protected GameObject GetGameObjectInfo(ResourceUIInfo iteminfo)
        {
            var trigger = iteminfo as ResourceUIInfo;

            var prefab = Resources.Load<GameObject>(iteminfo.resourcePath);
            if (prefab != null)
            {
                var instence = CreateInstance(prefab, trigger);
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
        protected GameObject CreateInstance(GameObject prefab, UIInfoBase nodeInfo)
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
    }

}
