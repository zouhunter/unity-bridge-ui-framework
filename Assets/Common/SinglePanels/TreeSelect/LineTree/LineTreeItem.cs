using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
namespace BridgeUI.Common.Tree
{
    public class LineTreeItem : MonoBehaviour
    {
        [SerializeField]
        private Toggle toggle;
        [SerializeField]
        private LayoutElement layout;

        private Transform childContent;
        private LineTreeRule rule;
        private TreeNode node;
        private LineTreeItemCreater creater;

        public UnityAction<List<string>> onSelection { get; set; }
        public LineTreeItemCreater Creater { get { return creater; } }
        private bool settingToggle;//防止设置toggle值时回调
        private Text title;
        void Awake()
        {
            toggle.onValueChanged.AddListener(OnToggleSwitch);
        }
        public void SetGroup(ToggleGroup group)
        {
            toggle.group = group;
        }
        public void InitTreeSelecter(int deepth, TreeNode node, LineTreeOption option)
        {
            this.node = node;
            var ruleget = option.ruleGetter;
            this.rule = ruleget(deepth);

            if (node.childern != null && node.childern.Count > 0)
            {
                InitContent(option.axisType);
                creater = new LineTreeItemCreater(deepth, childContent, option);
                var items = creater.CreateTreeSelectItems(node.childern.ToArray());
                foreach (var item in items)
                {
                    item.onSelection = OnSelection;
                }
            }
            else
            {
                //toggle.group = option.leafGroup;
            }

            ChargeRule();
        }

        internal void SetToggle(bool v, bool triggerInfo = false)
        {
            if (triggerInfo)
            {
                toggle.isOn = v;
            }
            else
            {
                settingToggle = true;

                toggle.isOn = v;

                settingToggle = false;
            }

        }

        private void OnSelection(List<string> path)
        {
            SetToggle(true);
            path.Insert(0, node.name);
            if (this.onSelection != null)
            {
                onSelection.Invoke(path);
            }
        }

        private void ChargeRule()
        {
            if (rule.normal) (toggle.targetGraphic as Image).sprite = rule.normal;
            if (rule.mask) (toggle.graphic as Image).sprite = rule.mask;
            title = (toggle.GetComponentInChildren<Text>());
            title.text = node.name;
            title.fontSize = rule.fontSize;
            title.color = toggle.isOn ? rule.fontColor_mask : rule.fontColor_normal;
            if (rule.font) title.font = rule.font;
            layout.preferredWidth = rule.horizontal;
            layout.preferredHeight = rule.vertical;
        }

        private void InitContent(GridLayoutGroup.Axis axis)
        {
            if (childContent == null)
            {
                var type = axis == GridLayoutGroup.Axis.Horizontal ? typeof(HorizontalLayoutGroup) : typeof(VerticalLayoutGroup);
                childContent = new GameObject(name + "_content", typeof(RectTransform), type).transform;
                childContent.transform.SetParent(transform.parent, false);
                childContent.SetSiblingIndex(transform.GetSiblingIndex() + 1);
                var group = childContent.GetComponent<HorizontalOrVerticalLayoutGroup>();
                group.childForceExpandHeight = false;
                group.childForceExpandWidth = false;
                group.spacing = rule.spacing;
            }
        }

        private void OnToggleSwitch(bool isOn)
        {
            if (!settingToggle && isOn && onSelection != null)
            {
                onSelection(new List<string> { node.name });
            }

            if (childContent && rule.childCloseAble)
            {
                childContent.gameObject.SetActive(isOn);
            }

            if (title)
            {
                title.color = isOn ? rule.fontColor_mask : rule.fontColor_normal;
            }

            if (creater != null && creater.Group != null && creater.Group.AnyTogglesOn())
            {
                LineTreeItem item = creater.Group.ActiveToggles().First().GetComponentInParent<LineTreeItem>();
                item.SetToggleForce(isOn);
            }
        }

        private void SetToggleForce(bool isOn)
        {
            var group = toggle.group;
            toggle.group = null;

            settingToggle = true;

            toggle.isOn = isOn;

            settingToggle = false;

            toggle.group = group;
        }
    }

}