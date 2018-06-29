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

namespace BridgeUI.CodeGen
{

    [System.Serializable]
    public class GenCodeRule
    {
        public int baseTypeIndex;
        public bool canInherit;
        public bool bindingAble;
        public bool generateViewModel;
    }
}