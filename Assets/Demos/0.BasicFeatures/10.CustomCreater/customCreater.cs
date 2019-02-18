using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BridgeUI.Model;
using UnityEngine.Events;

public class customCreater : BundleLoader
{
    public string url;

    public override void InitEnviroment()
    {
        Debug.Log("初始化Bundle创建器");
    }
    public override void LoadAssetAsync<T>(string assetBundleName, string assetName, UnityAction<T> onLoad)
    {
        Debug.LogFormat("从{0}:{1}加载出预制体并返回", assetBundleName, assetName);
    }
}
