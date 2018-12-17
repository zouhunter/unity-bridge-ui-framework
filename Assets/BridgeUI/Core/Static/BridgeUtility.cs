using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using BridgeUI.Model;
using UnityEngine.Events;
namespace BridgeUI
{
    public static class Utility
    {
        public static void SetTranform(Transform item, UILayerType layer, int layerIndex,Transform rootTrans, Transform parent,ref Dictionary<int,Transform> childDic)
        {
            if(parent == null)
            {
                return;
            }

            if(childDic == null)
            {
                childDic = new Dictionary<int, Transform>();
            }

            var layerID = (int)layer;
            Transform root = null;
            if(!childDic.TryGetValue(layerID, out root) || root == null)
            {
                string rootName = string.Format("{0}|{1}", layerID, layer);
                root = parent.transform.Find(rootName);

                if (root == null)
                {
                    root = new GameObject(rootName).transform;
                    root.gameObject.layer = 5;

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

                    root.position = rootTrans.TransformPoint(0, 0, layerID * -1000);

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

                childDic[layerID] = root;
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
    }
}