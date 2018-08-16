using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BridgeUI;

namespace BridgeUI.Common
{

    public abstract class SelectorData
    {
        public string info;
        public bool description;
        public abstract SelectorData[] SelectorDatas { get; }
        public abstract SelectorData InsetChild();
    }

    public abstract class SelectorData<T> : SelectorData where T : SelectorData, new()
    {
        [SerializeField]
        private List<T> selectorDatas;
        public List<T> childNodes { get { if (selectorDatas == null) selectorDatas = new List<T>(); return selectorDatas; } }
        public override SelectorData[] SelectorDatas
        {
            get
            {
                return childNodes.ToArray();
            }
        }
        public override SelectorData InsetChild()
        {
            var child = new T();
            childNodes.Add(child);
            return child;
        }
    }


    [Serializable]
    public class SelectorDataRoot : SelectorData<SelectorData1> { }

    [Serializable]
    public class SelectorDataLeaf : SelectorData
    {
        public override SelectorData[] SelectorDatas
        {
            get
            {
                return null;
            }
        }

        public override SelectorData InsetChild()
        {
            return null;
        }
    }

    [Serializable]
    public class SelectorData1 : SelectorData<SelectorData2> { }
    [Serializable]
    public class SelectorData2 : SelectorData<SelectorData3> { }
    [Serializable]
    public class SelectorData3 : SelectorData<SelectorData4> { }
    [Serializable]
    public class SelectorData4 : SelectorData<SelectorData5> { }
    [Serializable]
    public class SelectorData5 : SelectorData<SelectorDataLeaf> { }
}