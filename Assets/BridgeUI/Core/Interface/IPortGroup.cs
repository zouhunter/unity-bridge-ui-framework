using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace BridgeUI
{
    public interface IPortGroup
    {
        string[] Ports { get; }
    }
}