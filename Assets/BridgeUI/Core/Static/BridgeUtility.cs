using UnityEngine;
using System.Collections;
using BridgeUI.Model;
using UnityEngine.Events;
namespace BridgeUI
{
    public static class Utility
    {

        #region Transform And Layer
        public static void SetTranform(Transform item, UILayerType layer, int layerIndex, Transform parent)
        {
            if(parent == null)
            {
                return;
            }

            string rootName = LayerToString(layer);
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

            var childCount = root.childCount;
            int id = 0;
            for (int i = 0; i < childCount; i++)
            {
                var obj = root.GetChild(i);
                var panel = obj.GetComponent<IUIPanel>();
                if (panel == null || obj == item || panel.UType.layerIndex <= layerIndex)
                {
                    id = i;
                }
                else
                {
                    break;
                }
            }

            item.SetSiblingIndex(id);
        }
        public static string LayerToString(UILayerType layer, bool showint = true)
        {
            string str = "";
            if (showint) str += (int)layer + "|";

            switch (layer)
            {
                case UILayerType.Base:
                    str += "[B]";
                    break;
                case UILayerType.Tip:
                    str += "[T]";
                    break;
                case UILayerType.Warning:
                    str += "[W]";
                    break;
                default:
                    break;
            }
            return str;
        }
        #endregion


    }
}