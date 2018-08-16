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

public class folderToTree : MonoBehaviour {
    public TreeSelector treeSelector;
    private string folderPath;
    void Start()
    {
        folderPath = Application.dataPath;
        TreeNode tree = new ModelTreeCreater().EncodedTree(folderPath);// CreateTree();
        treeSelector.CreateTree(tree);
        Debug.Log(tree.childern.Length);
    }
}
