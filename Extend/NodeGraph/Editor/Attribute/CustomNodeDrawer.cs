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
    public class CustomNodeView : Attribute
    {
        public List<Type> targetTypes;
        public CustomNodeView(params Type[] types)
        {
            targetTypes = new List<Type>();
            this.targetTypes.AddRange(types);
        }
    }

}