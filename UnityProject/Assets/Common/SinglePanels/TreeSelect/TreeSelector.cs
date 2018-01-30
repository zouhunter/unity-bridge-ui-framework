using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace BridgeUI.Common
{

    public class TreeSelector : MonoBehaviour
    {
        [SerializeField, Header("[支持最多七层的规则]")]
        private List<TreeSelectRule> rules = new List<TreeSelectRule>();
        [SerializeField]
        private GridLayoutGroup.Axis axisType;
        [SerializeField]
        private Transform root;
        [SerializeField]
        private TreeSelectItem prefab;
        private TreeNodeCreater creater;
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
            var created = creater.CreateTreeSelectItems(axisType, nodeBase.childern.ToArray());
            foreach (var item in created)
            {
                item.onSelection = OnSelectionChanged;
            }
        }

        /// <summary>
        /// 设置目标为选中
        /// </summary>
        /// <param name="path"></param>
        public void SetSelect(string[] path)
        {

        }

        /// <summary>
        /// 当选中对象发生变化时
        /// </summary>
        /// <param name="path"></param>
        private void OnSelectionChanged(string[] path)
        {

        }

        /// <summary>
        /// 初始化环境
        /// </summary>
        private void InitRoot()
        {
            creater = new TreeNodeCreater(GetRule,0, root, prefab);
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