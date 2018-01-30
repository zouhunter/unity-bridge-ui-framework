using UnityEngine;
using System.Collections;
using BridgeUI.Common;
using System;

public class TreeTest : MonoBehaviour {
    public TreeNode1 node;
    public TreeSelector selector;
	// Use this for initialization
	void Start () {
        selector.CreateTree(node);
        selector.onSelect = OnSelect;
        selector.onSelectID = OnSelectId;
    }

    private void OnSelectId(int[] arg0)
    {
        foreach (var item in arg0)
        {
            print(item);
        }
    }

    // Update is called once per frame
    void OnSelect (string[] path) {
        foreach (var item in path)
        {
            print(item);
        }
	}
}
