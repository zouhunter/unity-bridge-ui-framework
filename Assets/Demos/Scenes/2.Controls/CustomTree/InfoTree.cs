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
using BridgeUI.Control.Tree;

[System.Serializable]
public class InfoTree1 : TreeNode<InfoTree2>
{
}

[System.Serializable]
public class InfoTree2 : TreeNodeLeaf
{
    public string infomation;
}
