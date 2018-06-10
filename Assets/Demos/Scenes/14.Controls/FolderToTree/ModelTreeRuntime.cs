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

public class ModelTreeRuntime : TreeNode
{
    public string infomation;
    public List<ModelTreeRuntime> childNodes = new List<ModelTreeRuntime>();
    public override List<TreeNode> childern
    {
        get
        {
            return childNodes.ConvertAll<TreeNode>(x => x);
        }
    }
    public override TreeNode InsetChild()
    {
        var child = new ModelTreeRuntime();
        childNodes.Add(child);
        return child;
    }
}