using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
namespace BridgeUI.Common
{
    public abstract class TreeNode
    {
        public string name;
        public object content;
        public abstract List<TreeNode> childern { get; }
        public abstract TreeNode InsetChild();
    }

    public abstract class TreeNode<T> : TreeNode where T : TreeNode, new()
    {
        [SerializeField]
        private List<T> _childNodes;
        public List<T> childNodes { get { if (_childNodes == null) _childNodes = new List<T>(); return _childNodes; } }
        public override List<TreeNode> childern
        {
            get
            {
                return childNodes.ConvertAll<TreeNode>(x => x);
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
    public class TreeNode1 : TreeNode<TreeNode2>
    {

    }
    [System.Serializable]
    public class TreeNode2 : TreeNode<TreeNode3>
    {

    }
    [System.Serializable]
    public class TreeNode3 : TreeNode<TreeNode4>
    {

    }
    [System.Serializable]
    public class TreeNode4 : TreeNode<TreeNode5>
    {

    }
    [System.Serializable]
    public class TreeNode5 : TreeNode<TreeNode6>
    {

    }
    [System.Serializable]
    public class TreeNode6 : TreeNode<TreeNode7>
    {

    }
    [System.Serializable]
    public class TreeNode7 : TreeNode
    {
        public override List<TreeNode> childern { get { return null; } }

        public override TreeNode InsetChild()
        {
            return null;
        }
    }
}
