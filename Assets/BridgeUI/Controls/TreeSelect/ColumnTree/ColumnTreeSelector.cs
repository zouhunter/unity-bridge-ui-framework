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
using System;
using System.Linq;

namespace BridgeUI.Control.Tree
{
    public class ColumnTreeSelector : TreeSelector
    {
        [SerializeField, Header("[支持最多七层的规则]")]
        private List<ColumnTreeRule> rules = new List<ColumnTreeRule>();
        private ColumnTreeItemCreater[] creaters;
        private ColumnTreeItemCreater rootCreater { get { if (creaters == null || creaters.Length == 0) return null; return creaters[0]; } }
        private bool treeCreating;
        protected override void Awake()
        {
            base.Awake();
            InitCreaters();
        }

        public override void CreateTree(TreeNode nodeBase)
        {
            base.CreateTree(nodeBase);
            AutoSelectFirst();
        }

        private void OpenSelect(ColumnTreeItemCreater creater, TreeNode node)
        {
            if (creater != null)
            {
                var options = node.childern.Select(x => x.name).ToArray();

                creater.OpenSelect(options);
                if (options.Length > 0)
                {
                    creater.SetActiveItem(options[0], true);
                }
            }
        }

        private void InitCreaters()
        {
            creaters = new ColumnTreeItemCreater[rules.Count];
            for (int i = 0; i < creaters.Length; i++)
            {
                creaters[i] = new ColumnTreeItemCreater(rules[i]);
                var index = i + 1;
                creaters[i].onChoise = (type) => { OnItemClicked(index, type); };
            }
        }

        public override void AutoSelectFirst()
        {
            if (rootCreater != null)
            {
                treeCreating = true;
                OpenSelect(rootCreater, rootNode);
                treeCreating = false;
            }
            OnSelectionChanged(GetCurrentSelected(creaters.Length));
        }

        public override void SetSelect(params string[] path)
        {
            for (int i = 0; i < creaters.Length; i++)
            {
                var creater = creaters[i];
                var node = GetTreeNode(rootNode, path,i);
                var options = node.childern.Select(x => x.name).ToArray();
                creater.OpenSelect(options);

                if (options.Length > 0)
                {
                    creater.SetActiveItem(path[i], path.Length <= i);
                }

            }

        }

        public override void SetSelect(params int[] path)
        {
            for (int i = 0; i < creaters.Length; i++)
            {
                var creater = creaters[i];
                var node = GetTreeNode(rootNode, path,i);
                var options = node.childern.Select(x => x.name).ToArray();
                creater.OpenSelect(options);

                if (options.Length > 0)
                {
                    creater.SetActiveItem(path[i], path.Length <= i);
                }
            }
        }


        private void OnItemClicked(int layer, string type)
        {
            List<string> selected = GetCurrentSelected(layer);

            if (layer < creaters.Length)
            {
                var root = GetTreeNode(rootNode, selected.ToArray());
                var creater = creaters[layer];
                Debug.Assert(root != null, "root empty");
                OpenSelect(creater, root);
            }

            if (!treeCreating) OnSelectionChanged(GetCurrentSelected(creaters.Length));
        }
        private List<string> GetCurrentSelected(int deep)
        {
            List<string> selected = new List<string>();
            for (int i = 0; i < deep; i++)
            {
                selected.Add(creaters[i].Selected);
            }
            return selected;
        }

        private TreeNode GetTreeNode(TreeNode node, string[] selection,int deepth = -1)
        {
            var root = node;
            if (deepth < 0) deepth = selection.Length;
            for (int i = 0; i < deepth; i++)
            {
                root = Array.Find( root.childern,x => x.name == selection[i]);
            }
            return root;
        }
        private TreeNode GetTreeNode(TreeNode node, int[] selection,int deepth = -1)
        {
            var root = node;
            if (deepth < 0) deepth = selection.Length;
            for (int i = 0; i < deepth; i++)
            {
                if(root.childern != null && root.childern.Length > selection[i])
                root = root.childern[selection[i]];
            }
            return root;
        }

        public override void ClearTree()
        {
            foreach (var item in creaters)
            {
                item.Clear();
            }
        }
    }

}