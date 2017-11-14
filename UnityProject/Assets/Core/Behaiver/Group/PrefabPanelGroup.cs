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

public class PrefabPanelGroup : PanelGroup {
    public List<PrefabUINode> nodes;
    public override List<UINodeBase> Nodes { get { return nodes.ConvertAll<UINodeBase>(x => x); } }
}
