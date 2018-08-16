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



public abstract class ModelTree : TreeNode
{
    public string infomation;
    public GameObject prefab;
}

public class ModelTree<T> : ModelTree where T : ModelTree, new()
{
    [SerializeField]
    private List<T> _childNodes;
    public List<T> childNodes { get { if (_childNodes == null) _childNodes = new List<T>(); return _childNodes; } }
    public override TreeNode[] childern
    {
        get
        {
            return childNodes.ToArray();
        }
    }
    public override TreeNode InsetChild()
    {
        var child = new T();
        childNodes.Add(child);
        return child;
    }
}

[System.Serializable]
public class ModelTree0 : ModelTree<ModelTree1>
{
}
[System.Serializable]
public class ModelTree1 : ModelTree<ModelTree2>
{
}
[System.Serializable]
public class ModelTree2 : ModelTree
{
    public override TreeNode[] childern { get { return null; } }

    public override TreeNode InsetChild()
    {
        return null;
    }
}

