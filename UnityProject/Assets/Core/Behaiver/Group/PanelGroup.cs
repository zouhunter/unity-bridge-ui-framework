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
using System;
using BridgeUI.Model;

namespace BridgeUI
{
    /// <summary>
    /// 用于标记ui打开的父级
    /// [3维场景中可能有多个地方需要打开用户界面]
    /// </summary>
    public class PanelGroup : MonoBehaviour, IPanelGroup
    {
#if UNITY_EDITOR
        public List<GraphWorp> graphList;
#endif
        public bool resetMenu;
        public string menu;
        public LoadType loadType = LoadType.Prefab;
        public List<BundleUIInfo> b_nodes;
        public List<PrefabUIInfo> p_nodes;
        public List<BridgeInfo> bridges;
        public Transform Trans { get { return transform; } }
        public List<UIInfoBase> Nodes { get { return activeNodes; } }
        private BridgeInfo defultBridge;
        private Dictionary<BridgeInfo, BridgePool> poolDic = new Dictionary<BridgeInfo, BridgePool>();
        private List<IPanelBase> createdPanels = new List<IPanelBase>();
        private Dictionary<IPanelBase, Stack<IPanelBase>> hidedPanelStack = new Dictionary<IPanelBase, Stack<IPanelBase>>();
        private Dictionary<IPanelBase, Bridge> bridgeDic = new Dictionary<IPanelBase, Bridge>();
        private List<UIInfoBase> activeNodes;
        private IPanelCreater creater;

        void Awake()
        {
            InitCreater();
            RegistUINodes();
            RegistBridgePool();
            TryAutoOpen(Trans);
            UIFacade.RegistGroup(this);
        }

        private void OnEnable()
        {
            var created = createdPanels.ToArray();
            TryOpenPanels(created);
            Debug.Log("OnEnable");
        }

        private void OnDisable()
        {
            var created = createdPanels.ToArray();
            foreach (var item in created)
            {
                item.Hide();
            }
        }
        protected virtual void OnDestroy()
        {
            UIFacade.UnRegistGroup(this);
        }



        public Bridge InstencePanel(IPanelBase parentPanel, string panelName, Transform root)
        {
            Bridge bridge = null;
            UIInfoBase uiNode = null;
            if (TryMatchPanel(parentPanel, panelName, out bridge, out uiNode))
            {
                uiNode.OnCreate = (go) =>
                {
                    Utility.SetTranform(go.transform, uiNode.type.layer, uiNode.type.layerIndex, root == null ? Trans : root);
                    go.SetActive(true);
                    var panel = go.GetComponent<IPanelBase>();
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

        public List<IPanelBase> RetrivePanels(string panelName)
        {
            var panels = createdPanels.FindAll(x => x.Name == panelName);
            return panels;
        }

        #region private Functions

        private void InitCreater()
        {
            if (resetMenu && loadType == LoadType.Bundle)
            {
                creater = new PanelCreater(menu);
            }
            else
            {
                creater = new PanelCreater();
            }

        }
        private void HandBridgeOptions(IPanelBase panel, Bridge bridge)
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
        private void TryMakeCover(IPanelBase panel, BridgeInfo info)
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
        private void TryHideGroup(IPanelBase panel, BridgeInfo bridge)
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
        private void TryHideMutexPanels(IPanelBase childPanel, BridgeInfo bridge)
        {
            if (bridge.showModel.mutex != MutexRule.NoMutex)
            {
                if (bridge.showModel.mutex == MutexRule.SameParentAndLayer)
                {
                    var mayBridges = bridges.FindAll(x => x.inNode == bridge.inNode);
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

        private bool IsChildOfPanel(IPanelBase current, IPanelBase target)
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

        /// <summary>
        /// 自动打开子面板
        /// </summary>
        /// <param name="panel"></param>
        private void TryAutoOpen(Transform content, IPanelBase parentPanel = null)
        {
            var panelName = parentPanel == null ? "" : parentPanel.Name;
            var autoBridges = bridges.FindAll(x => x.inNode == panelName && x.showModel.auto);
            if (autoBridges != null)
            {
                foreach (var autoBridge in autoBridges)
                {
                    InstencePanel(parentPanel, autoBridge.outNode, content);
                }
            }
        }
        /// <summary>
        /// 注册所有ui节点信息
        /// </summary>
        private void RegistUINodes()
        {
            activeNodes = new List<UIInfoBase>();

            if ((loadType & LoadType.Bundle) == LoadType.Bundle)
            {
                activeNodes.AddRange(b_nodes.ConvertAll<UIInfoBase>(x => x));
            }

            if ((loadType & LoadType.Prefab) == LoadType.Prefab)
            {
                activeNodes.AddRange(p_nodes.ConvertAll<UIInfoBase>(x => x));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="needHidePanel"></param>
        private void HidePanelInteral(IPanelBase panel, IPanelBase needHidePanel)
        {
            if (needHidePanel.IsShowing)
            {
                needHidePanel.Hide();
            }
            if (!hidedPanelStack.ContainsKey(panel))
            {
                hidedPanelStack[panel] = new Stack<IPanelBase>();
            }
            //Debug.Log("push:" + needHidePanel);
            hidedPanelStack[panel].Push(needHidePanel);
        }
        /// <summary>
        /// 按规则设置面板及父亲面板的状态
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="root"></param>
        /// <param name="bridge"></param>
        private void InitPanel(IPanelBase panel, Bridge bridge, UIInfoBase uiNode)
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
        private void TryChangeParentState(IPanelBase panel, BridgeInfo bridge)
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
                            hidedPanelStack[panel] = new Stack<IPanelBase>();
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
        /// <param name="parentName"></param>
        /// <param name="panelName"></param>
        /// <param name="bridgeObj"></param>
        /// <param name="uiNode"></param>
        /// <returns></returns>
        private bool TryMatchPanel(IPanelBase parentPanel, string panelName, out Bridge bridgeObj, out UIInfoBase uiNode)
        {
            uiNode = Nodes.Find(x => x.panelName == panelName);

            if (uiNode != null)// && uiNode.type.form == UIFormType.Fixed
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
        /// <param name="parentName"></param>
        /// <param name="panelName"></param>
        /// <returns></returns>
        private Bridge GetBridgeClamp(IPanelBase parentPanel, string panelName)
        {
            Bridge bridge = null;
            var parentName = parentPanel == null ? "" : parentPanel.Name;
            var mayInfos = bridges.FindAll(x => x.outNode == panelName);
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
                bridge.Info = new BridgeInfo(parentName, panelName, show);
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
            foreach (var item in bridges)
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
        private void OnDeletePanel(IPanelBase panel)
        {
            if (createdPanels.Contains(panel))
            {
                createdPanels.Remove(panel);
            }

            if (bridgeDic.ContainsKey(panel))
            {
                bridgeDic.Remove(panel);
            }

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

            if (hidedPanelStack.ContainsKey(panel))
            {
                var mayactive = new List<IPanelBase>();
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
        private void TryOpenPanels(IPanelBase[] panels)
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
        #endregion
    }
}