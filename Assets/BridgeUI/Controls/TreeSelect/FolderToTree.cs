#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-05-16 11:38:12
    * 说    明：       
* ************************************************************************************/
#endregion
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BridgeUI.Control.Tree
{
    /// <summary>
    /// 从文件夹创建树形资源
    /// <summary>
    public abstract class FolderToTree<T> where T :TreeNode,new()
    {
        public T EncodedTree(string folder)
        {
            var rootNode = new T();
            EncodedTree(folder, rootNode);
            return rootNode;
        }
        protected void EncodedTree(string folder, TreeNode node)
        {
            var directory = Directory.CreateDirectory(folder);
            if (directory.Exists)
            {
                node.name = directory.Name;
                var files = directory.GetFiles();
                RecrodFiles(files, node);
                var subDirectorys = directory.GetDirectories();

                foreach (var item in subDirectorys)
                {

                    var child = node.InsetChild();
                    if(child != null)
                    {
                        Debug.Log(child);
                        EncodedTree(item.FullName, child);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        protected virtual void RecrodFiles(FileInfo[] files, TreeNode node) { }
    }
}