using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
namespace BridgeUI.Control.Tree
{
    public abstract class TreeNode
    {
        public string name;
        public object content { get; set; }
        public abstract List<TreeNode> childern { get; }
        public TreeNode ParentItem { get; internal set; }
        public string[] FullPath
        {
            get
            {
                var path = CalcutePath();
                return path;
            }
        }
        public abstract TreeNode InsetChild();
        public TreeNode GetChildItem(string key)
        {
            if (childern == null)
            {
                return null;
            }
            return childern.Find(x => x.name == key);
        }
        public TreeNode GetChildItem(int index)
        {
            if (childern == null || childern.Count <= index)
            {
                return null;
            }
            return childern[index];
        }
        private string[] CalcutePath()
        {
            var list = new List<string>();
            var item = this;
            while (item.ParentItem != null)
            {
                var nameCurrent = item.name;
                item = item.ParentItem;
                if(item != null)
                {
                    list.Add(nameCurrent);
                }
            }
            list.Reverse();
            return list.ToArray();
        }
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
    public class TreeNodeLeaf : TreeNode
    {
        public override List<TreeNode> childern { get { return null; } }
        public override TreeNode InsetChild()
        {
            return null;
        }
    }
}
