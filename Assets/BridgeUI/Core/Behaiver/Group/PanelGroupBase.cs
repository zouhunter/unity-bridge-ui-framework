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
using BridgeUI.Model;

namespace BridgeUI
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PanelGroupBase : MonoBehaviour, IPanelGroup
    {
        public Transform Trans { get { return transform; } }
        public List<UIInfoBase> Nodes { get { return activeNodes; } }
        protected BridgeInfo defultBridge;
        protected Dictionary<BridgeInfo, BridgePool> poolDic = new Dictionary<BridgeInfo, BridgePool>();
        protected List<IUIPanel> createdPanels = new List<IUIPanel>();
        protected Dictionary<IUIPanel, Stack<IUIPanel>> hidedPanelStack = new Dictionary<IUIPanel, Stack<IUIPanel>>();
        protected Dictionary<IUIPanel, Bridge> bridgeDic = new Dictionary<IUIPanel, Bridge>();
        protected List<UIInfoBase> activeNodes;
        protected IPanelCreater creater;
        protected event UnityAction onDestroy;
        private List<BundleUIInfo> _b_nodes;
        private List<PrefabUIInfo> _p_nodes;
        private List<BridgeInfo> _bridges;
        public UIBindingController bindingCtrl { get; private set; }
        public abstract string Menu { get; }
        public abstract bool ResetMenu { get; }
        public abstract List<Graph.UIGraph> GraphList { get; }
        public List<BundleUIInfo> B_Nodes
        {
            get
            {
                if (_b_nodes == null)
                {
                    _b_nodes = new List<BundleUIInfo>();
                    foreach (var item in GraphList)
                    {
                        _b_nodes.AddRange(item.b_nodes);
                    }
                }
                return _b_nodes;
            }
        }
        public List<PrefabUIInfo> P_Nodes
        {
            get
            {
                if (_p_nodes == null)
                {
                    _p_nodes = new List<PrefabUIInfo>();
                    foreach (var graph in GraphList)
                    {
                        _p_nodes.AddRange(graph.p_nodes);
                    }
                }
                return _p_nodes;
            }
        }
        public List<BridgeInfo> Bridges
        {
            get
            {
                if (_bridges == null)
                {
                    _bridges = new List<BridgeInfo>();
                    foreach (var graph in GraphList)
                    {
                        _bridges.AddRange(graph.bridges);
                    }
                }
                return _bridges;
            }
        }

        private void OnEnable()
        {
            var created = createdPanels.ToArray();
            TryOpenPanels(created);
            UIFacade.RegistGroup(this);
        }
        protected void OnDisable()
        {
            var created = createdPanels.ToArray();
            foreach (var item in created)
            {
                item.Hide();
            }
        }
        protected virtual void OnDestroy()
        {
            if (onDestroy != null)
            {
                onDestroy.Invoke();
            }
            UIFacade.UnRegistGroup(this);
        }

        protected void LunchPanelGroupSystem()
        {
            InitCreater();
            RegistUINodes();
            RegistBridgePool();
            TryAutoOpen(Trans);
            RegistUIEvents();
        }

        public Bridge InstencePanel(IUIPanel parentPanel, string panelName, Transform root)
        {
            Bridge bridge = null;
            UIInfoBase uiNode = null;
            if (TryMatchPanel(parentPanel, panelName, out bridge, out uiNode))
            {
                uiNode.OnCreate = (go) =>
                {
                    Utility.SetTranform(go.transform, uiNode.type.layer, uiNode.type.layerIndex, root == null ? Trans : root);
                    go.SetActive(true);
                    var panel = go.GetComponent<IUIPanel>();
                    if (panel != null)
                    {
                        createdPanels.Add(panel);
                        if (parentPanel != null)
                        {
                            parentPanel.RecordChild(panel);
                        }
                        bridgeDic.Add(panel, bridge);
                        InitPanel(panel, bridge, uiNode);
                        HandBridgeOptions(panel, bridge);
                    }
                };
                creater.CreatePanel(uiNode);
            }
            return bridge;
        }

        public List<IUIPanel> RetrivePanels(string panelName)
        {
            var panels = createdPanels.FindAll(x => x.Name == panelName);
            return panels;
        }

        #region protected Functions

        /// <summary>
        /// 初始化面板创建器
        /// </summary>
        protected void InitCreater()
        {
            if (ResetMenu)
            {
                creater = new PanelCreater(Menu);
            }
            else
            {
                creater = new PanelCreater();
            }

        }
        /// <summary>
        /// 处理面板打开规则
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="bridge"></param>
        protected void HandBridgeOptions(IUIPanel panel, Bridge bridge)
        {
            TryChangeParentState(panel, bridge.Info);
            TryHideMutexPanels(panel, bridge.Info);
            TryHideGroup(panel, bridge.Info);
            TryMakeCover(panel, bridge.Info);
            TryAutoOpen(panel.Content, panel);
        }

        /// <summary>
        /// 建立遮罩
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="info"></param>
        protected void TryMakeCover(IUIPanel panel, BridgeInfo info)
        {
            if (info.showModel.cover)
            {
                panel.Cover();
            }
        }
        /// <summary>
        /// 隐藏整个面板中其他的ui界面
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="bridge"></param>
        protected void TryHideGroup(IUIPanel panel, BridgeInfo bridge)
        {
            if ((bridge.showModel.single))
            {
                var parent = createdPanels.Find(x => x.Name == bridge.inNode);
                if (parent != null)
                {
                    panel.SetParent(Trans);
                }
                foreach (var oldPanel in createdPanels)
                {
                    if (oldPanel != panel)
                    {
                        HidePanelInteral(panel, oldPanel);
                    }
                }
            }
        }

        /// <summary>
        /// 互斥面板自动隐藏
        /// </summary>
        /// <param name="childPanel"></param>
        /// <param name=""></param>
        /// <param name="bridge"></param>
        protected void TryHideMutexPanels(IUIPanel childPanel, BridgeInfo bridge)
        {
            if (bridge.showModel.mutex != MutexRule.NoMutex)
            {
                if (bridge.showModel.mutex == MutexRule.SameParentAndLayer)
                {
                    var mayBridges = Bridges.FindAll(x => x.inNode == bridge.inNode);
                    foreach (var bg in mayBridges)
                    {
                        var mayPanels = createdPanels.FindAll(x => x.Name == bg.outNode && x.UType.layer == childPanel.UType.layer && x != childPanel && !IsChildOfPanel(childPanel, x));
                        foreach (var mayPanel in mayPanels)
                        {
                            if (mayPanel != null && mayPanel.IsShowing)
                            {
                                HidePanelInteral(childPanel, mayPanel);
                            }
                        }

                    }
                }
                else if (bridge.showModel.mutex == MutexRule.SameLayer)
                {
                    var mayPanels = createdPanels.FindAll(x => x.UType.layer == childPanel.UType.layer && x != childPanel && !IsChildOfPanel(childPanel, x));
                    foreach (var mayPanel in mayPanels)
                    {
                        if (mayPanel != null && mayPanel.IsShowing)
                        {
                            HidePanelInteral(childPanel, mayPanel);
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 判断面板的父子关系
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        protected bool IsChildOfPanel(IUIPanel current, IUIPanel target)
        {
            if (current.Parent == null)
            {
                return false;
            }
            if (current.Parent == target)
            {
                return true;
            }
            else
            {
                return IsChildOfPanel(current.Parent, target);
            }
        }

        ///  <summary>
        /// 自动打开子面板
        /// </summary>
        /// <param name="content"></param>
        /// <param name="parentPanel"></param>
        protected void TryAutoOpen(Transform content, IUIPanel parentPanel = null)
        {
            var panelName = parentPanel == null ? "" : parentPanel.Name;
            var autoBridges = Bridges.FindAll(x => CompareName(x.inNode, panelName) && x.showModel.auto);
            if (autoBridges != null)
            {
                foreach (var autoBridge in autoBridges)
                {
                    InstencePanel(parentPanel, autoBridge.outNode, content);
                }
            }
        }
        /// <summary>
        /// 名称比较
        /// </summary>
        /// <param name="nameA"></param>
        /// <param name="nameB"></param>
        /// <returns></returns>
        protected bool CompareName(string nameA, string nameB)
        {
            if (string.IsNullOrEmpty(nameA))
            {
                return string.IsNullOrEmpty(nameB);
            }
            return string.Compare(nameA, nameB) == 1;
        }

        /// <summary>
        /// 注册所有ui节点信息
        /// </summary>
        protected void RegistUINodes()
        {
            activeNodes = new List<UIInfoBase>();
            activeNodes.AddRange(B_Nodes.ConvertAll<UIInfoBase>(x => x));
            activeNodes.AddRange(P_Nodes.ConvertAll<UIInfoBase>(x => x));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="needHidePanel"></param>
        protected void HidePanelInteral(IUIPanel panel, IUIPanel needHidePanel)
        {
            if (needHidePanel.IsShowing)
            {
                needHidePanel.Hide();
            }
            if (!hidedPanelStack.ContainsKey(panel))
            {
                hidedPanelStack[panel] = new Stack<IUIPanel>();
            }
            //Debug.Log("push:" + needHidePanel);
            hidedPanelStack[panel].Push(needHidePanel);
        }
        /// <summary>
        /// 按规则设置面板及父亲面板的状态
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="bridge"></param>
        /// <param name="uiNode"></param>
        protected void InitPanel(IUIPanel panel, Bridge bridge, UIInfoBase uiNode)
        {
            panel.UType = uiNode.type;
            panel.Group = this;
            panel.onDelete += OnDeletePanel;
            panel.HandleData(bridge);

        }
        /// <summary>
        /// 选择性隐藏父级
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="bridge"></param>
        protected void TryChangeParentState(IUIPanel panel, BridgeInfo bridge)
        {
            if (bridge.showModel.baseShow == BaseShow.Hide)
            {
                var parent = panel.Parent;
                if (parent != null)
                {
                    panel.SetParent(parent.Root);
                    HidePanelInteral(panel, parent);
                }
            }

            if (bridge.showModel.baseShow == BaseShow.Destroy)
            {
                var parent = panel.Parent;
                if (parent != null && parent.ChildPanels.Count > 0)
                {
                    panel.SetParent(parent.Root);
                    parent.ChildPanels.Remove(panel);

                    if (hidedPanelStack.ContainsKey(parent))
                    {
                        if (!hidedPanelStack.ContainsKey(panel))
                        {
                            hidedPanelStack[panel] = new Stack<IUIPanel>();
                        }
                        while (hidedPanelStack[parent].Count > 0)
                        {
                            hidedPanelStack[panel].Push(hidedPanelStack[parent].Pop());
                        }
                    }

                    parent.Close();
                }
            }
        }

        /// <summary>
        /// 找到信息源和bridge
        /// </summary>
        /// <param name="parentPanel"></param>
        /// <param name="panelName"></param>
        /// <param name="bridgeObj"></param>
        /// <param name="uiNode"></param>
        /// <returns></returns>
        protected bool TryMatchPanel(IUIPanel parentPanel, string panelName, out Bridge bridgeObj, out UIInfoBase uiNode)
        {
            uiNode = Nodes.Find(x => x.panelName == panelName);

            if (uiNode != null)
            {
                if (uiNode.type.form == UIFormType.Fixed)
                {
                    var oldPanel = createdPanels.Find(x => x.Name == panelName);
                    if (oldPanel != null)
                    {
                        bridgeObj = bridgeDic[oldPanel];
                        if (!oldPanel.IsShowing)
                        {
                            oldPanel.UnHide();
                            HandBridgeOptions(oldPanel, bridgeObj);
                        }
                        return false;
                    }
                }
            }

            if (uiNode == null)
            {
                bridgeObj = null;
                return false;
            }

            bridgeObj = GetBridgeClamp(parentPanel, panelName);
            return uiNode != null && bridgeObj != null;
        }

        /// <summary>
        /// 获取可用的bridge
        /// </summary>
        /// <param name="parentPanel"></param>
        /// <param name="panelName"></param>
        /// <returns></returns>
        protected Bridge GetBridgeClamp(IUIPanel parentPanel, string panelName)
        {
            Bridge bridge = null;
            var parentName = parentPanel == null ? "" : parentPanel.Name;
            var mayInfos = Bridges.FindAll(x => x.outNode == panelName);
            var baseInfos = mayInfos.FindAll(x => x.inNode == parentName);
            BridgeInfo? bridgeInfo = null;
            if (baseInfos.Count > 0)
            {
                bridgeInfo = baseInfos[0];
            }
            else
            {
                var usefulInfos = mayInfos.FindAll(x => string.IsNullOrEmpty(x.inNode));
                if (usefulInfos.Count > 0)
                {
                    bridgeInfo = usefulInfos[0];
                }
            }

            if (bridgeInfo == null)
            {
                bridge = poolDic[defultBridge].Allocate(parentPanel);
                var show = new ShowMode(false, MutexRule.NoMutex, false, BaseShow.NoChange, false);
                bridge.Info = new BridgeInfo(parentName, panelName, show, 0);
            }
            else
            {
                bridge = poolDic[(BridgeInfo)bridgeInfo].Allocate(parentPanel);
            }
            return bridge;
        }

        /// <summary>
        /// bridge生成池
        /// </summary>
        protected virtual void RegistBridgePool()
        {
            foreach (var item in Bridges)
            {
                poolDic[item] = new BridgePool(item);
            }
            defultBridge = new BridgeInfo();
            poolDic[defultBridge] = new BridgePool(defultBridge);
        }

        /// <summary>
        /// 当删除一个面板时触发一些事
        /// </summary>
        /// <param name="panel"></param>
        protected void OnDeletePanel(IUIPanel panel, bool remove)
        {
            if (remove)
            {
                //从缓存中移除
                if (createdPanels.Contains(panel))
                {
                    createdPanels.Remove(panel);
                }

                //移除连通器
                if (bridgeDic.ContainsKey(panel))
                {
                    bridgeDic.Remove(panel);
                }

                //关闭子面板
                if (panel.ChildPanels != null)
                {
                    var childs = panel.ChildPanels.ToArray();
                    foreach (var item in childs)
                    {
                        if (item.IsAlive)
                        {
                            item.Close();
                        }
                    }
                }
            }

            //显示隐藏面板
            if (hidedPanelStack.ContainsKey(panel))
            {
                var mayactive = new List<IUIPanel>();
                var stack = hidedPanelStack[panel];
                if (stack != null)
                {
                    while (stack.Count > 0)
                    {
                        var item = stack.Pop();
                        mayactive.Add(item);
                    }
                }
                hidedPanelStack.Remove(panel);
                TryOpenPanels(mayactive.ToArray());
            }
        }
        /// <summary>
        /// 尝试打开隐藏的面板
        /// （如果没有被占用，则可以打开）
        /// </summary>
        /// <param name="panels"></param>
        protected void TryOpenPanels(IUIPanel[] panels)
        {
            bool canActive = true;
            foreach (var item in panels)
            {
                canActive = true;
                foreach (var panelStack in hidedPanelStack)
                {
                    if (panelStack.Value.Contains(item))
                    {
                        canActive = false;
                        break;
                    }
                }

                if (canActive && item.IsAlive && !item.IsShowing)
                {
                    item.UnHide();
                }
            }
        }
        #region 图形化界面关联
        /// <summary>
        /// 注册ui事件
        /// </summary>
        protected void RegistUIEvents()
        {
            if (bindingCtrl == null) bindingCtrl = new UIBindingController();
            foreach (var item in Bridges)
            {
                var bridgeInfo = item;

                if (!string.IsNullOrEmpty(bridgeInfo.inNode) && !string.IsNullOrEmpty(bridgeInfo.outNode))
                {
                    UIBindingItem bindingItem = new UIBindingItem();

                    bindingItem.openAction = (x, y) =>
                    {
                        var parentPanel = x;
                        var panelName = bridgeInfo.outNode;
                        return UIFacade.Instence.Open(parentPanel, panelName, y);
                    };

                    bindingItem.closeAction = () =>
                    {
                        var panelName = bridgeInfo.outNode;
                        UIFacade.Instence.Close(this, panelName);
                    };

                    bindingItem.hideAction = () =>
                    {
                        var panelName = bridgeInfo.outNode;
                        UIFacade.Instence.Hide(this, panelName);
                    };

                    bindingItem.isOpenAction = () =>
                    {
                        var panelName = bridgeInfo.outNode;
                        return UIFacade.Instence.IsPanelOpen(this, panelName);
                    };

                    bindingCtrl.RegistPanelEvent(bridgeInfo.inNode, bridgeInfo.index, bindingItem);

                    this.onDestroy += () =>
                    {
                        //在本组合关闭时销毁事件
                        bindingCtrl.RemovePanelEvent(bridgeInfo.inNode, bridgeInfo.index, bindingItem);
                    };
                }

            }
        }
        #endregion
        #endregion
    }
}