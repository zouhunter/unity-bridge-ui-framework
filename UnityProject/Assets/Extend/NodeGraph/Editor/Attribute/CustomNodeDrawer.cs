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

using System;
namespace NodeGraph
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomNodeGraphDrawer : Attribute
    {
        public Type targetType;
        public CustomNodeGraphDrawer(Type type)
        {
            this.targetType = type;
        }
    }

}