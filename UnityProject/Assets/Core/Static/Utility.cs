using UnityEngine;
using System.Collections;

public static class Utility {
    public static void SetTranform(Transform item, UIType type, Transform parent)
    {
        string rootName = LayerToString(type.layer);
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
    public static string LayerToString(UILayerType layer, bool showint = true)
    {
        string str = "";
        if (showint) str += (int)layer + "|";

        switch (layer)
        {
            case UILayerType.Bottom:
                str += "[B]";
                break;
            case UILayerType.Middle:
                str += "[M]";
                break;
            case UILayerType.Top:
                str += "[T]";
                break;
            default:
                break;
        }
        return str;
    }
}
