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

    public PanelCreater(IPanelGroup group)
    {
        _root = group.Trans;
    }

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
    public void GetGameObjectInfo(UINodeBase itemInfo)
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
    /// BundleUINode创建对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="panelName"></param>
    /// <param name="onCreate"></param>
    public void GetGameObjectInfo(BundleUINode itemInfo)
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
    public void GetGameObjectInfo(PrefabUINode iteminfo)
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
    /// 取消创建对象
    /// </summary>
    /// <param name="panelName"></param>
    public void CansaleLoadObject(string panelName)
    {
        _cansaleKeys.Add(panelName);
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
        SetTranform(go, trigger.type, _root);

        if (trigger.OnCreate != null) trigger.OnCreate(go);
    }

    public static void SetTranform(GameObject item, UIType type, Transform parent)
    {
        string rootName = LayerToString(type);
        var root = parent.transform.Find(rootName);
        if (root == null)
        {
            root = new GameObject(rootName).transform;
            if (parent is RectTransform)
            {
                var rectParent = root.gameObject.AddComponent<RectTransform>();
                rectParent.anchorMin = Vector2.zero;
                rectParent.anchorMax = Vector2.one;
                rectParent.offsetMin = Vector3.zero;
                rectParent.offsetMax = Vector3.zero;
                root = rectParent;
                root.SetParent(parent, false);
            }
            else
            {
                root.SetParent(parent, true);
            }

            if (rootName.StartsWith("-1"))
            {
                root.SetAsLastSibling();
            }
            else
            {
                int i = 0;
                for (; i < parent.childCount; i++)
                {
                    var ritem = parent.GetChild(i);
                    if (ritem.name.StartsWith("-1"))
                    {
                        break;
                    }
                    if (string.Compare(rootName, ritem.name) < 0)
                    {
                        break;
                    }
                }
                root.SetSiblingIndex(i);
            }
        }
        item.transform.SetParent(root, !(item.GetComponent<Transform>() is RectTransform));
    }

    public static string LayerToString(UIType layer, bool showint = true)
    {
        string str = "";
        if (showint) str += (int)layer + "|";

        switch (layer)
        {
            case UIType.Bottom:
                str += "[Bottom]";
                break;
            case UIType.Heap:
                str += "[H]";
                break;
            case UIType.Pop:
                str += "[P]";
                break;
            case UIType.Tip:
                str += "[T]";
                break;
            default:
                break;
        }
        return str;
    }

    /// <summary>
    /// 重新排序
    /// </summary>
    /// <param name="parentDic"></param>
    private void ResortParents(Dictionary<int, Transform> parentDic)
    {
        int[] keys = new int[parentDic.Count];
        parentDic.Keys.CopyTo(keys, 0);
        System.Array.Sort(keys);
        for (int i = 0; i < keys.Length; i++)
        {
            parentDic[keys[i]].SetAsLastSibling();
        }
        if (parentDic.ContainsKey(-1))
        {
            parentDic[-1].SetAsLastSibling();
        }
    }
}
