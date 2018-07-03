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
using Object = UnityEngine.Object;

namespace BridgeUI.CodeGen
{
    [System.Serializable]
    public class BindingEvent
    {
        public BindingType type;
        public string bindingSource;
        public string bindingTarget;
        public TypeInfo bindingTargetType;
    }

    public enum BindingType
    {
        NoBinding = 0,
        Normal = 1,
        WithTarget = 2
    }
}