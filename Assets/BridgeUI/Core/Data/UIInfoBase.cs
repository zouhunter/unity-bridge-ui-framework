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
using BridgeUI.Model;
namespace BridgeUI
{
    /// <summary>
    /// ui节点信息的抽象父级
    /// [记录ui打开的规则]
    /// </summary>
    public abstract class UIInfoBase
    {
//#if UNITY_EDITOR
        public int instanceID;
        public string discription;
//#endif
        public string panelName;
        public UIType type;
        public abstract string IDName { get; }
    }
}