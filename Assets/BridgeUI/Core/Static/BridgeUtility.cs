using System;
using System.Collections.Generic;
using UnityEngine;
namespace BridgeUI
{
    public static class Utility
    {
        private static List<IPanelGroup> panelGroups = new List<IPanelGroup>();

        public static IPanelGroup[] GetActivePanelGroups()
        {
            return panelGroups.ToArray();
        }

        public static void RegistPanelGroup(IPanelGroup group)
        {
            if (group == null) return;

            if(!panelGroups.Contains(group))
            {
                panelGroups.Add(group);
            }
        }
        public static void RemovePanelGroup(IPanelGroup group)
        {
            if (group == null) return;
            panelGroups.Remove(group);
        }

        public static void SetTranform(Transform item, UILayerType layer, int layerIndex,Transform rootTrans, Transform parent,Dictionary<int,Transform> rootDic,Dictionary<int,IUIPanel> transRefDic)
        {
            if (parent == null) return;
           
            var layerID = (int)layer;
            Transform root = null;
            if(!rootDic.TryGetValue(layerID, out root) || root == null)
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

                rootDic[layerID] = root;
            }

 
            item.transform.SetParent(root, !(item.GetComponent<Transform>() is RectTransform));

            int id = 0;

            if (transRefDic != null)
            {
                var childCount = root.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var obj = root.GetChild(i);
                    IUIPanel panel;
                    if(!transRefDic.TryGetValue(obj.GetInstanceID(), out panel) || obj == item || panel.UType.layerIndex <= layerIndex)
                    {
                        id = i;
                    }
                    else
                    {
                        break;
                    }
                }
            }
           
            item.SetSiblingIndex(id);
        }


        public static GameObjectPool CreatePool(Transform parent, out GameObject target)
        {
            target = new GameObject("ItemPool");
            var poolTrans = target.GetComponent<Transform>();
            poolTrans.SetParent(parent);
            var objPool = new GameObjectPool(poolTrans);
            return objPool;
        }
    }
}