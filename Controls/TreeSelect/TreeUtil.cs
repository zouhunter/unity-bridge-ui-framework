using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
namespace BridgeUI.Control.Tree
{
    public class TreeUtil
    {
        public static void CollectTreeLeaf<T>(T root, ref List<T> leafs) where T:TreeNode
        {
            if (leafs == null) leafs = new List<T>();
            if (root.childern == null && !leafs.Contains(root))
            {
                leafs.Add(root);
            }
            else
            {
                foreach (var child in root.childern)
                {
                    CollectTreeLeaf(child as T, ref leafs);
                }
            }
        }
    }
}