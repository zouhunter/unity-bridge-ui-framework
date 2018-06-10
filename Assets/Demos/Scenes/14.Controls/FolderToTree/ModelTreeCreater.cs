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
using System.IO;

public class ModelTreeCreater : FolderToTree<ModelTreeRuntime>
{
    protected override void RecrodFiles(FileInfo[] files, TreeNode node)
    {
        base.RecrodFiles(files, node);
        if (files == null || files.Length == 0) return;

        var treeNode = node as ModelTreeRuntime;
        var infomationFile = System.Array.Find(files, x => x.Name == node.name);
        if (infomationFile != null)
        {
            var text = System.IO.File.ReadAllText(infomationFile.FullName);
            treeNode.infomation = text;
        }
    }
}
