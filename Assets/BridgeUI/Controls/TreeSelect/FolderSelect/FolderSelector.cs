using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BridgeUI.Control.Tree
{
    public class FolderSelector : Tree.TreeSelector
    {
        [SerializeField]
        private ToggleListSelector creater;
        public Events.StringArrayEvent onSelectionChanged;

        private List<string> currentSelection = new List<string>();
        private Tree.TreeNode active;
        public ToggleListSelector Selector { get { return creater; } }
        public int deepth
        {
            get
            {
                return currentSelection.Count();
            }
        }


        #region public
        public override void CreateTree(Tree.TreeNode nodeBase)
        {
            base.CreateTree(nodeBase);
            active = nodeBase;
            ClearTree();
            OnHeadSelect(0);
        }
        public override void AutoSelectFirst()
        {
            ClearTree();
            OnHeadSelect(0);
            Tree.TreeNode node = rootNode;
            while (node.childern != null && node.childern.Length > 0)
            {
                node = node.childern[0];
                creater.SetSelect(0, true);
            }
        }
        public override void SetSelect(params string[] path)
        {
            ClearTree();
            OnHeadSelect(0);
            Tree.TreeNode node = rootNode;
            for (int i = 0; i < path.Length; i++)
            {
                if (node.childern != null)
                {
                    var child = node.GetChildItem(path[i]);
                    if (child != null)
                    {
                        var index =System.Array.IndexOf( node.childern,(child));
                        creater.SetSelect(index, true);
                        node = node.childern[i];
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        public override void SetSelect(params int[] path)
        {
            ClearTree();
            OnHeadSelect(0);

            Tree.TreeNode node = rootNode;
            for (int i = 0; i < path.Length; i++)
            {
                if (node.childern != null && node.childern.Length > path[i])
                {
                    creater.SetSelect(path[i], true);
                    node = node.childern[i];
                }
                else
                {
                    break;
                }
            }
        }
        public void OnHeadSelect(int dp)
        {
            active = rootNode;
            for (int i = 0; i < dp; i++)
            {
                var key = currentSelection[i];
                active =System.Array.Find( active.childern,x => x.name == key);
            }
            
            //更新选择列表
            ChargeBodyOption(active);
            //更新view
            UpdateCurrentSelectFromActiveNode(active);
        }

        public void BackOneSelect()
        {
            if (active.ParentItem != null)
            {
                active = active.ParentItem;
                               //更新选择列表
                ChargeBodyOption(active);
                UpdateCurrentSelectFromActiveNode(active);
            }
        }
        public override void ClearTree()
        {
            onSelectionChanged.Invoke(null);
            creater.options = null;
        }
        #endregion
        /// <summary>
        /// 显示已经选择的状态列表
        /// </summary>
        /// <param name="keys"></param>
        private void ChargeHeadOption(string[] keys)
        {
            onSelectionChanged.Invoke(keys);
        }

        /// <summary>
        /// 创建当前可选择的列表
        /// </summary>
        /// <param name="node"></param>
        private void ChargeBodyOption(Tree.TreeNode node)
        {
            if (node.childern != null && node.childern.Length > 0)
            {
                string[] selectItems = node.childern.Select(x => x.name).ToArray();
                creater.options = selectItems;
            }
        }


        /// <summary>
        /// 可选择项选中后，
        /// 判断是否需要继续选择，或者是触发事件
        /// </summary>
        /// <param name="key"></param>
        private void OnBodySelect(int key)
        {
            //最底层的选择逻辑
            if ((active.childern == null || active.childern.Length == 0) && active.ParentItem != null)
            {
                active = active.ParentItem;
            }

            var child = active.GetChildItem(key);
            if (child != null)
            {
                //激活当前节点
                active = child;

                if (child.childern != null && child.childern.Length > 0)
                {
                    //更新选择列表
                    ChargeBodyOption(active);
                }
                else
                {
                    Debug.Assert(active != null, "active :Null", gameObject);
                    OnSelectionChanged(child.FullPath);
                }

                //更新标头列表
                UpdateCurrentSelectFromActiveNode(active);
            }

        }

        private void UpdateCurrentSelectFromActiveNode(TreeNode node)
        {
            currentSelection.Clear();
            while (node.ParentItem != null)
            {
                if (node.ParentItem != null)
                {
                    currentSelection.Add(node.name);
                }
                node = node.ParentItem;
            }
            currentSelection.Reverse();

            //更新显示
            ChargeHeadOption(currentSelection.ToArray());
        }

        protected override void OnInititalize()
        {
            creater.Initialize(context);
            creater.onSelectID.AddListener(OnBodySelect);
            creater.SetSingleSelect();
        }

        protected override void OnUnInitialize()
        {
            creater.onSelectID.RemoveListener(OnBodySelect);
        }
    }
}