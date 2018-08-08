using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace BridgeUI.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class PortAttribute : PropertyAttribute
    {
        public string portInfo;
        public PortAttribute(string portInfo)
        {
            this.portInfo = portInfo;
        }
    }
}