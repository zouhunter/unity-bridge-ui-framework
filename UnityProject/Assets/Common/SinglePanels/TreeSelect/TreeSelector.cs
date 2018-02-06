#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 01:13:36
    * 说    明：       1.传入树型结构数据
                       2.注册选择事件
                       3.配制显示信息
* ************************************************************************************/
#endregion

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BridgeUI.Common
{
    /// <summary>
    /// 树形选择器
    /// </summary>
    public class TreeSelector : MonoBehaviour
    {
        [SerializeField, Header("[支持最多七层的规则]")]
        private List<TreeSelectRule> rules = new List<TreeSelectRule>();
        [SerializeField]
        private TreeOption option;
        [SerializeField]
        private HorizontalOrVerticalLayoutGroup root;

        private TreeNodeCreater creater;
        private TreeNode rootNode;

        public UnityAction<int[]> onSelectID { get; set; }
        public UnityAction<string[]> onSelect { get; set; }
        private void Awake()
        {
            InitRoot();
        }
        /// <summary>
        /// 创建树型ui
        /// </summary>
        /// <param name="nodeBase"></param>
        public void CreateTree(TreeNode nodeBase)
        {
            this.rootNode = nodeBase;
            var created = creater.CreateTreeSelectItems(nodeBase.childern.ToArray());
            foreach (var item in created)
            {
                item.onSelection = OnSelectionChanged;
            }
        }

        /// <summary>
        /// 设置目标为选中
        /// </summary>
        /// <param name="path"></param>
        public void SetSelect(params string[] path)
        {
            var list = new List<string>(path);
            var idPath = GetIDPath(list);
            SetSelect(idPath);
        }
        /// <summary>
        /// 设置目标为选中
        /// </summary>
        /// <param name="path"></param>
        public void SetSelect(params int[] path)
        {
            var list = new List<int>(path);
            creater.SetChildActive(list);
        }

        /// <summary>
        /// 当选中对象发生变化时
        /// </summary>
        /// <param name="path"></param>
        private void OnSelectionChanged(List<string> path)
        {
            if (onSelect != null)
            {
                onSelect.Invoke(path.ToArray());
            }

            if (onSelectID != null)
            {
                onSelectID(GetIDPath(path));
            }
        }

        public void AutoSelectFirst()
        {
            var currentCreater = creater;
            while (currentCreater != null && currentCreater.CreatedItems != null && currentCreater.CreatedItems.Count() > 0)
            {
                var firstChild = currentCreater.CreatedItems[0];
                firstChild.SetToggle(true,true);
                currentCreater = firstChild.Creater;
            }
        }

        /// <summary>
        /// 转换为id路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private int[] GetIDPath(List<string> path)
        {
            var idList = new List<int>();
            var parent = rootNode;
            for (int i = 0; i < path.Count; i++)
            {
                var id = parent.childern.FindIndex(x => x.name == path[i]);
                idList.Add(id);
                parent = parent.childern[id];
            }
            return idList.ToArray();
        }

        /// <summary>
        /// 初始化环境
        /// </summary>
        private void InitRoot()
        {
            option.ruleGetter = GetRule;
            option.axisType = root is HorizontalLayoutGroup ? GridLayoutGroup.Axis.Horizontal : GridLayoutGroup.Axis.Vertical;
            creater = new TreeNodeCreater(0, root.transform, option);
        }

        /// <summary>
        /// 获取指定层级的规则
        /// </summary>
        /// <param name="deepth"></param>
        /// <returns></returns>
        private TreeSelectRule GetRule(int deepth)
        {
            var rule = rules.Find(x => x.deepth == deepth);
            if (rule == null)
            {
                if (rules.Count > 0)
                {
                    rule = rules[rules.Count - 1].CreateCopy(deepth);
                }
                else
                {
                    rule = new TreeSelectRule();
                    rule.deepth = deepth;
                }
            }
            return rule;
        }
    }
}