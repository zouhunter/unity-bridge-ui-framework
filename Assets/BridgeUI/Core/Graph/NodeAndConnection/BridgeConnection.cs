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
using NodeGraph;
using NodeGraph.DataModel;
using BridgeUI;
using BridgeUI.Binding;
using System;
namespace BridgeUI.Graph
{
    [CustomConnection("bridge")]
    public class BridgeConnection : Connection
    {
        public bool blocking;//堵塞
        public ShowMode show = new ShowMode();
        public ScriptableObject viewModel;
    }
}