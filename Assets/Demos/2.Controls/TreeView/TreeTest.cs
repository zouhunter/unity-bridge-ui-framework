using UnityEngine;
using System.Collections.Generic;
using BridgeUI.Control.Tree;
using System;

public class TreeTest : MonoBehaviour {
    public TreeNode1 node;
    public List<TreeSelector> selectors;

	// Use this for initialization
	void Start () {

        foreach (var selector in selectors)
        {
            selector.Initialize(this);
            selector.onSelect = OnSelect;
            selector.onSelectID = OnSelectId;
            selector.CreateTree(node);
        }
        
    }
    void OnGUI()
    {
        if (GUILayout.Button("Set:0/1"))
        {
            foreach (var selector in selectors)
            {
                selector.SetSelect(0, 1);// = OnSelect;
            }
        }
        if (GUILayout.Button("AutoSelectFirset"))
        {
            foreach (var selector in selectors)
            {
                selector.AutoSelectFirst();// = OnSelect;
            }
        }
    }

    private void OnSelectId(int[] path)
    {
        Debug.Log("selectedID:" + string.Join("/",Array.ConvertAll<int,string>( path,x=>x.ToString())));
    }

    // Update is called once per frame
    void OnSelect (string[] path) {
        Debug.Log("selected:" + string.Join("/",path));
    }
}
