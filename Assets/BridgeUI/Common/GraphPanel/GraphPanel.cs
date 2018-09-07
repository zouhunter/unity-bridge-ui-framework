/*************************************************************************************   
    * 作    者：       DefaultCompany
    * 创建时间：       2018-05-29 03:01:09
    * 说    明：       1.部分代码自动生成
                       2.尽量使用MVVM模式
                       3.宏定义内会读成注释
* ************************************************************************************/using BridgeUI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
namespace BridgeUI.Common
{
    ///<summary>
    ///[代码说明信息]
    ///<summary>
    [Attributes.PanelParent]
    public class GraphPanel : BridgeUI.SinglePanel
    {
        [SerializeField]
        protected Button m_save;
        [SerializeField]
        protected Transform content;
        [SerializeField]
        public GraphNodeBehaiver m_nodePrefab;
        protected SelectorData selector;
        protected string path;
        protected GraphNodeBehaiver rootItem;

        protected override void Start()
        {
            base.Start();
            m_nodePrefab.gameObject.SetActive(false);
            m_save.onClick.AddListener(OnSave);
        }

        protected override void HandleData(object data)
        {
            base.HandleData(data);
            selector = data as SelectorData;
            if(selector == null){
                selector = new SelectorDataRoot();
            }
            InitGraph();
        }

        protected void InitGraph()
        {
            if(rootItem != null){
                GameObject.Destroy(rootItem.gameObject);
            }
            rootItem = GetInstenceItem(0, content);
            rootItem.SetAsRoot(selector,GetInstenceItem);
        }

        protected GraphNodeBehaiver GetInstenceItem(int deepth, Transform parent)
        {
            if (deepth < 7)
            {
                GraphNodeBehaiver horizontal = Instantiate(m_nodePrefab, parent, false);
                horizontal.gameObject.SetActive(true);
                return horizontal;
            }
            else
            {
                return null;
            }
        }

        protected void OnSave()
        {
            if(rootItem != null && rootItem.gameObject)
            {
                SelectorDataRoot selector = new SelectorDataRoot();
                SaveComplic(rootItem, selector);
                CallBack(selector);
            }
        }

        protected void SaveComplic(GraphNodeBehaiver nodeBehaiver, SelectorData parent)
        {
            //保存信息
            parent.info = nodeBehaiver.Info;
            parent.description = nodeBehaiver.Description;

            for (int i = 0; i < nodeBehaiver.Content.childCount; i++)
            {
                var item = nodeBehaiver.Content.GetChild(i).GetComponent<GraphNodeBehaiver>();

                if (item == null) continue;

                SelectorData data = parent.InsetChild();
                
                if (item.Content.childCount != 0 && data != null)
                {
                    SaveComplic(item, data);
                }
            }
        }

    }
}