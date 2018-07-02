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
public class TreeNode1 : TreeNode<TreeNode2> { }
[System.Serializable]
public class TreeNode2 : TreeNode<TreeNode3> { }
[System.Serializable]
public class TreeNode3 : TreeNode<TreeNode4> { }
[System.Serializable]
public class TreeNode4 : TreeNode<TreeNodeLeaf> { }
