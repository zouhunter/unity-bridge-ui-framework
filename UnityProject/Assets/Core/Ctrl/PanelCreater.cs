using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelCreater:IPanelCreater
{
#if AssetBundleTools
        private AssetBundleLoader assetLoader;
#endif
    private List<string> _loadingKeys = new List<string>();
    private List<string> _cansaleKeys = new List<string>();
    private Transform _root;

    public PanelCreater(Transform root)
    {
        _root = root;
#if AssetBundleTools
            assetLoader = AssetBundleLoader.Instence;
#endif
    }
    public PanelCreater(string url, string menu, Transform root)
    {
        _root = root;
#if AssetBundleTools
            assetLoader = AssetBundleLoader.GetInstance(url, menu);
#endif
    }

    /// <summary>
    /// 创建对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="panelName"></param>
    /// <param name="onCreate"></param>
    public void CreatePanel(UINodeBase itemInfo)
    {
        if (_cansaleKeys.Contains(itemInfo.panelName)) _cansaleKeys.RemoveAll(x => x == itemInfo.panelName);

        if (!_loadingKeys.Contains(itemInfo.IDName))
        {
            _loadingKeys.Add(itemInfo.IDName);
            var bInfo = itemInfo as BundleUINode;
            var pInfo = itemInfo as PrefabUINode;

            if (bInfo != null)
            {
                GetGameObjectInfo(bInfo);
            }
            else if (pInfo != null)
            {
                GetGameObjectInfo(pInfo);
            }
        }
    }
    /// <summary>
    /// 取消创建对象
    /// </summary>
    /// <param name="panelName"></param>
    public void CansaleCreate(string panelName)
    {
        _cansaleKeys.Add(panelName);
    }
    /// <summary>
    /// BundleUINode创建对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="panelName"></param>
    /// <param name="onCreate"></param>
    private void GetGameObjectInfo(BundleUINode itemInfo)
    {
        var trigger = itemInfo as BundleUINode;
#if AssetBundleTools
            assetLoader.LoadAssetFromUrlAsync<GameObject>(trigger.bundleName, trigger.panelName, (x) =>
            {
                if (_root == null)
                {
                    Debug.Log("父节点已销毁");
                }
                else if (x != null)
                {
                    CreateInstance(x, trigger);
                    _loadingKeys.Remove(trigger.IDName);
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
    private void GetGameObjectInfo(PrefabUINode iteminfo)
    {
        var trigger = iteminfo as PrefabUINode;

        if (trigger.prefab != null)
        {
            CreateInstance(trigger.prefab, trigger);
            _loadingKeys.Remove(trigger.IDName);
        }
        else
        {
            Debug.Log(trigger.panelName + "-->空");
        }
    }
    /// <summary>
    /// 获取对象实例
    /// </summary>
    private void CreateInstance(GameObject prefab, UINodeBase trigger)
    {
        if (_cansaleKeys.Contains(trigger.panelName))
        {
            _cansaleKeys.Remove(trigger.panelName);
            return;
        }

        if (prefab == null || trigger == null)
        {
            return;
        }

        GameObject go = GameObject.Instantiate(prefab);

        go.SetActive(true);
        Utility.SetTranform(go, trigger.type, _root);

        if (trigger.OnCreate != null)
            trigger.OnCreate(go);
    }

  
}
