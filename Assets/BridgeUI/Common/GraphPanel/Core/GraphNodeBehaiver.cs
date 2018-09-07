using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Linq;

namespace BridgeUI.Common
{
    public class GraphNodeBehaiver : UIBehaviour
    {
        [SerializeField]
        private Button m_addItem;//添加子项
        [SerializeField]
        private Button m_insert;//插入节点
        [SerializeField]
        private Button m_destory;//删除子项
        [SerializeField]
        private InputField info;//节点名称
        [SerializeField]
        private Toggle m_auto;

        [SerializeField]
        private Transform m_content;

        public Transform Content { get { return m_content; } }//子树父级
        public string Info { get { return info.text; } set { info.text = value; } }//详细信息
        public bool Description { get { return m_auto.isOn; } }//是否用于描述

        private int deepth { get; set; }

        private System.Func<int, Transform, GraphNodeBehaiver> GetInstence { get; set; }

        protected override void Start()
        {
            base.Start();
            RegisterEvent();
        }

        /// <summary>
        /// 注册快捷事件
        /// </summary>
        private void RegisterEvent()
        {
            m_addItem.onClick.AddListener(() => AddItem());
            m_destory.onClick.AddListener(DestroyItem);
            m_insert.onClick.AddListener(InsertItem);
        }


        /// <summary>
        /// 插入节点
        /// </summary>
        public void InsertItem()
        {
            var maxDeepth = FindMaxLeafIndex();
            var instence = GetInstence(maxDeepth + 1, m_content);
            SetAsChildItem(instence);

            if (instence != null)
            {
                for (int i = 0; i < Content.childCount; i++)
                {
                    var childItem = Content.GetChild(i).GetComponent<GraphNodeBehaiver>();
                    if (childItem == null || childItem == instence) continue;
                    childItem.transform.SetParent(instence.Content);
                }

                AppendChildDeepth(1, instence.Content);
            }
        }

        private void SetAsChildItem(GraphNodeBehaiver instence)
        {
            if (instence != null)
            {
                instence.deepth = deepth + 1;
                instence.GetInstence = GetInstence;
                Content.gameObject.SetActive(true);
            }
        }

        private int FindMaxLeafIndex()
        {
            var root = GetComponentsInChildren<GraphNodeBehaiver>();
            return Mathf.Max(root.Select(x => x.deepth).ToArray());
        }

        private void AppendChildDeepth(int offset, Transform content)
        {
            var childItems = new List<GraphNodeBehaiver>();
            for (int i = 0; i < content.childCount; i++)
            {
                var item = content.GetChild(i).GetComponent<GraphNodeBehaiver>();
                if (item == null) continue;
                childItems.Add(item);
            }

            for (int i = 0; i < childItems.Count; i++)
            {
                childItems[i].deepth += offset;
                AppendChildDeepth(offset, childItems[i].Content);
            }

        }

        /// <summary>
        /// 添加
        /// </summary>
        public GraphNodeBehaiver AddItem()
        {
            var instence = GetInstence(deepth + 1, m_content);
            SetAsChildItem(instence);
            return instence;
        }


        /// <summary>
        /// 删除节点
        /// </summary>
        public void DestroyItem()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// 创建根节点并传入子树数据源
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="getInstenceItem"></param>
        /// <param name="selectorDatas"></param>
        public void SetAsRoot(SelectorData data,Func<int, Transform, GraphNodeBehaiver> getInstenceItem)
        {
            this.GetInstence = getInstenceItem;
            m_auto.gameObject.SetActive(false);
            InstanceChildItems(data.SelectorDatas);
        }

        /// <summary>
        /// 利用数据源初始化子树
        /// </summary>
        /// <param name="datas"></param>
        private void InstanceChildItems(SelectorData[] datas)
        {
            if (datas == null) return;

            bool single = datas.Length == 1;

            if (datas.Length == 0) return;

            for (int i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                var node = GetInstence(deepth + 1, Content);
                if (node == null)
                {
                    return;
                }
                else
                {
                    node.deepth = deepth + 1;
                }

                node.gameObject.SetActive(true);
                node.GetInstence = GetInstence;
                node.info.text = data.info;

                if (single)
                {
                    node.m_auto.gameObject.SetActive(true);
                    node.m_auto.isOn = data.description;
                }
                else
                {
                    data.description = false;
                    node.m_auto.gameObject.SetActive(false);
                }

                if (data.SelectorDatas != null && data.SelectorDatas.Length != 0)
                {
                    node.InstanceChildItems(data.SelectorDatas);
                }
            }
        }
    }
}