using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
namespace BridgeUI.Common
{
    public class TreeSelectItem : MonoBehaviour
    {
        [SerializeField]
        private Toggle toggle;
        [SerializeField]
        private LayoutElement layout;

        private int deepth;
        private Transform childContent;
        private TreeSelectRule rule;
        private TreeNode node;
        private TreeNodeCreater creater;
        public UnityAction<List<string>> onSelection { get; set; }

        void Awake()
        {
            toggle.onValueChanged.AddListener(OnToggleSwitch);
        }

        public void InitTreeSelecter(GridLayoutGroup.Axis axis,int deepth, Func<int,TreeSelectRule> ruleget, TreeNode node,TreeSelectItem prefab)
        {
            this.deepth = deepth;
            this.node = node;
            this.rule = ruleget(deepth);

            if(node.childern != null && node.childern.Count > 0)
            {
                InitContent(axis);
                creater = new TreeNodeCreater(ruleget, deepth, childContent, prefab);
                var items = creater.CreateTreeSelectItems(axis, node.childern.ToArray());
                foreach (var item in items)
                {
                    item.onSelection = OnSelection;
                }
            }

            ChargeRule();
        }

        private void OnSelection(List<string> path)
        {
            path.Insert(0, node.name);
            if(this.onSelection != null)
            {
                onSelection.Invoke(path);
            }
        }

        private void ChargeRule()
        {
            if(rule.normal) (toggle.graphic as Image).sprite = rule.normal;
            if(rule.mask) (toggle.targetGraphic as Image).sprite = rule.mask;
            var title = (toggle.GetComponentInChildren<Text>());
            title.text = node.name;
            title.fontSize = rule.fontSize;
            layout.preferredWidth = rule.horizontal;
            layout.preferredHeight = rule.vertical;
            if(toggle.isOn != rule.defultOpen)
            {
                toggle.isOn = rule.defultOpen;
            }
            else
            {
                OnToggleSwitch(toggle.isOn);
            }
        }

        private void InitContent(GridLayoutGroup.Axis axis)
        {
            if(childContent == null)
            {
                var type = axis == GridLayoutGroup.Axis.Horizontal ? typeof(HorizontalLayoutGroup) : typeof(VerticalLayoutGroup);
                childContent = new GameObject(name + "_content", typeof(RectTransform), type).transform;
                childContent.transform.SetParent(transform.parent);
                childContent.SetSiblingIndex(transform.GetSiblingIndex() + 1);
                var group = childContent.GetComponent<HorizontalOrVerticalLayoutGroup>();
                group.childForceExpandHeight = false;
                group.childForceExpandWidth = false;
                group.spacing = rule.spacing;
            }
        }

        private void OnToggleSwitch(bool isOn)
        {
            if(isOn && onSelection != null)
            {
                onSelection(new List<string> { node.name });
            }

            if (childContent)
            {
                childContent.gameObject.SetActive(isOn);
            }
            else
            {
                if(isOn)
                {
                    toggle.isOn = false;
                }
            }
        }

    }

}