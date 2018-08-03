using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace BridgeUI.Control
{
    public class FolderSelector : Tree.TreeSelector
    {
        [SerializeField]
        private ButtonListSelector result;
        [SerializeField]
        private ToggleListSelector creater;

        private List<string> currentSelection = new List<string>();
        private Tree.TreeNode active;

        protected override void Awake()
        {
            base.Awake();
            result.onSelectID.AddListener(OnHeadSelect);
            creater.onSelectID.AddListener(OnBodySelect);
            creater.singleChoise = true;
        }

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
            while (node.childern != null && node.childern.Count > 0)
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
                        var index = node.childern.IndexOf(child);
                        creater.SetSelect(index, true);
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
                if (node.childern != null && node.childern.Count > path[i])
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
        public override void ClearTree()
        {
            result.options = null;
            creater.options = null;
        }
        /// <summary>
        /// 显示已经选择的状态列表
        /// </summary>
        /// <param name="keys"></param>
        private void ChargeHeadOption(string[] keys)
        {
            if (keys != null)
            {
                result.options = keys;
            }
        }

        /// <summary>
        /// 创建当前可选择的列表
        /// </summary>
        /// <param name="node"></param>
        private void ChargeBodyOption(Tree.TreeNode node)
        {
            if (node.childern != null && node.childern.Count > 0)
            {
                string[] selectItems = node.childern.ConvertAll<string>(x => x.name).ToArray();
                creater.options = selectItems;
            }
        }

        /// <summary>
        /// 选择标题列表
        /// </summary>
        /// <param name="dp"></param>
        private void OnHeadSelect(int dp)
        {
            active = rootNode;
            for (int i = 0; i < dp; i++)
            {
                var key = currentSelection[i];
                active = active.childern.Find(x => x.name == key);
            }

            //更新标头列表
            if (currentSelection.Count > dp)
                currentSelection.RemoveRange(dp, currentSelection.Count - dp);

            ChargeHeadOption(currentSelection.ToArray());

            //更新选择列表
            ChargeBodyOption(active);
        }


        /// <summary>
        /// 可选择项选中后，
        /// 判断是否需要继续选择，或者是触发事件
        /// </summary>
        /// <param name="key"></param>
        private void OnBodySelect(int key)
        {
            var child = active.GetChildItem(key);
            if (child != null && child.childern != null && child.childern.Count > 0)
            {
                //激活当前节点
                active = child;

                //更新选择列表
                ChargeBodyOption(active);

                //更新标头列表
                currentSelection.Add(active.name);
                ChargeHeadOption(currentSelection.ToArray());
            }
            else
            {
                Debug.Assert(active != null, "active :Null", gameObject);
                OnSelectionChanged(child.FullPath);
            }
        }
    }
}