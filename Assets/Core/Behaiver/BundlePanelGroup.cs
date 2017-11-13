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

public class BundlePanelGroup : PanelGroup
{
    public List<BundleUINode> nodes;
    public override List<UINodeBase> Nodes { get { return nodes.ConvertAll<UINodeBase>(x => x); } }
}
