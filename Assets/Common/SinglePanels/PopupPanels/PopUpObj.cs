using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using System;
namespace BridgeUI.Common
{
    [CreateAssetMenu(menuName = "CustomAsset/PopUpObj")]
    public class PopUpObj : ScriptableObject
    {
        [System.Serializable]
        public class PopData
        {
            public PopUpType type;
            public string title;
            [Multiline(2)]
            public string info;

            public PopData(PopUpType popType)
            {
                this.type = popType;
            }
        }
#if UNITY_EDITOR
        public UnityEditor.MonoScript popTypeBehaiver;
#endif
        public List<PopData> popDatas = new List<PopData>();

        public PopData GetPopData(PopUpType type)
        {
            var item = popDatas.Find(x => x.type == type);
            return item;
        }

        public void Reset()
        {
            var types = System.Enum.GetNames(typeof(PopUpType));
            foreach (var item in types)
            {
                var popType = (PopUpType)System.Enum.Parse(typeof(PopUpType), item);
                var pitem = popDatas.Find(x => x.type == popType);
                if (pitem == null)
                {
                    popDatas.Add(new PopUpObj.PopData(popType));
                }
            }
        }
    }
}