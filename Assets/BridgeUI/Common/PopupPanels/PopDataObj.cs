using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
namespace BridgeUI.Common
{
    public class PopDataObj : ScriptableObject
    {
        [System.Serializable]
        public class PopData
        {
            public int typeInt;
            public string typeName;
            public string title;
            public bool donthide;
            public string info;

            public PopData(int typeInt, string typeName)
            {
                this.typeInt = typeInt;
                this.typeName = typeName;
            }
        }

#if UNITY_EDITOR
        public UnityEditor.MonoScript popEnum;
#endif
        public string enumType;
        public List<PopData> popDatas = new List<PopData>();
        public string descrition;

        public PopData GetPopData(int typeInt)
        {
            var item = popDatas.Find(x => x.typeInt == typeInt);
            return item;
        }
    }
}