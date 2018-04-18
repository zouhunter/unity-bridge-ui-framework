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

namespace BridgeUI.Common.Tree
{
    /// <summary>
    /// 树形选择器
    /// </summary>
    public class LineTreeSelector : TreeSelector
    {
        [SerializeField, Header("[支持最多七层的规则]")]
        private List<LineTreeRule> rules = new List<LineTreeRule>();
        [SerializeField]
        private LineTreeOption option;
        [SerializeField]
        private HorizontalOrVerticalLayoutGroup root;

        private LineTreeItemCreater creater;

        protected override void Awake()
        {
            base.Awake();
            InitRoot();
        }
        /// <summary>
        /// 创建树型ui
        /// </summary>
        /// <param name="nodeBase"></param>
        public override void CreateTree(TreeNode nodeBase)
        {
            base.CreateTree(nodeBase);
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
        public override void SetSelect(params string[] path)
        {
            var list = new List<string>(path);
            var idPath = GetIDPath(list);
            SetSelect(idPath);
        }
        /// <summary>
        /// 设置目标为选中
        /// </summary>
        /// <param name="path"></param>
        public override void SetSelect(params int[] path)
        {
            var list = new List<int>(path);
            creater.SetChildActive(list);
        }

        public override void AutoSelectFirst()
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
        /// 初始化环境
        /// </summary>
        private void InitRoot()
        {
            option.ruleGetter = GetRule;
            option.axisType = root is HorizontalLayoutGroup ? GridLayoutGroup.Axis.Horizontal : GridLayoutGroup.Axis.Vertical;
            creater = new LineTreeItemCreater(0, root.transform, option);
        }

        /// <summary>
        /// 获取指定层级的规则
        /// </summary>
        /// <param name="deepth"></param>
        /// <returns></returns>
        private LineTreeRule GetRule(int deepth)
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
                    rule = new LineTreeRule();
                    rule.deepth = deepth;
                }
            }
            return rule;
        }
    }
}