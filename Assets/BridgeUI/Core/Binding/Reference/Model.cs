using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeUI.Binding
{

    public class ReferenceCatchBehaiver : MonoBehaviour
    {
        [HideInInspector]
        public List<SerializeableReferenceItem> cacheItems;

        public void SetReferenceItems(List<ReferenceItem> items)
        {
            cacheItems = new List<SerializeableReferenceItem>();
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var cacheItem = new SerializeableReferenceItem();
                if (item.type != null)
                {
                    cacheItem.assembleName = item.type.Assembly.FullName;
                    cacheItem.typeName = item.type.FullName;
                }

                cacheItem.name = item.name;
                cacheItem.isArray = item.isArray;

                if (item.isArray)
                {
                    if (item.referenceTargets != null)
                    {
                        cacheItem.referenceInstenceIDs = new List<int>();
                        item.referenceTargets.ForEach(x =>
                        {
                            if (x)
                            {
                                cacheItem.referenceInstenceIDs.Add(x.GetInstanceID());
                            }
                        });
                    }
                    cacheItem.values = item.values;
                }
                else
                {
                    cacheItem.value = item.value;
                    if (item.referenceTarget)
                    {
                        cacheItem.refereneceInstenceID = item.referenceTarget.GetInstanceID();
                    }
                }
                cacheItems.Add(cacheItem);
            }
        }

        public List<ReferenceItem> GetReferenceItems()
        {
            var items = new List<ReferenceItem>();
            for (int i = 0; i < cacheItems.Count; i++)
            {
                var cacheItem = cacheItems[i];
                var item = new ReferenceItem();
                item.name = cacheItem.name;
                item.isArray = cacheItem.isArray;

                if (!string.IsNullOrEmpty(cacheItem.assembleName) && !string.IsNullOrEmpty(cacheItem.typeName))
                {
                    item.type = System.Reflection.Assembly.Load(cacheItem.assembleName).GetType(cacheItem.typeName);
                }

                if (item.isArray)
                {
                    item.referenceTargets = new List<Object>();
                    if (cacheItem.referenceInstenceIDs != null)
                    {
                        cacheItem.referenceInstenceIDs.ForEach(x =>
                        {
#if UNITY_EDITOR
                            item.referenceTargets.Add(UnityEditor. EditorUtility.InstanceIDToObject(x));
#endif
                        });
                    }
                    item.values = cacheItem.values;
                }
                else
                {
#if UNITY_EDITOR
                    item.referenceTarget = UnityEditor.EditorUtility.InstanceIDToObject(cacheItem.refereneceInstenceID);
#endif
                    item.value = cacheItem.value;
                }

                items.Add(item);
            }
            return items;
        }
    }

    [System.Serializable]
    public class SerializeableReferenceItem
    {
        public string name;
        public string assembleName;
        public string typeName;
        public int refereneceInstenceID;
        public string value;
        public bool isArray;
        public List<int> referenceInstenceIDs;
        public List<string> values;
    }

    public class ReferenceItem
    {
        public string name;
        public Object referenceTarget;
        public string value;
        public System.Type type;
        public List<Object> referenceTargets;
        public List<string> values;
        public bool isArray;
    }

    public enum ReferenceItemType
    {
        Reference,
        ConventAble,
        Struct,
        ReferenceArray,
        ConventAbleArray,
        StructArray,
    }

}
