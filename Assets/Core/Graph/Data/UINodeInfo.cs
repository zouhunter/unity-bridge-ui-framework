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
namespace BridgeUI.Model
{
    [System.Serializable]
    public class NodeInfo
    {
        public UIType uiType = new UIType();
        public GameObject prefab;
        public string discription;
        public bool vm_script;
    }
}
