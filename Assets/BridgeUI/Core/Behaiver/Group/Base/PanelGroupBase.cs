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
using System;

namespace BridgeUI
{
    public struct UICreateHandle
    {
        public delegate void CreateCallBack(GameObject go, UIInfoBase uiNode, Bridge bridge, Transform parent, IUIPanel parentPanel);
        public CreateCallBack onCreate { get; set; }
        public UIInfoBase uiNode { get; set; }
        public Bridge bridge { get; set; }
        public Transform parent { get; set; }
        public IUIPanel parentPanel { get; set; }

        public void OnCreate(GameObject go)
        {
            if (onCreate != null)
            {
                if (go != null)
                {
                    onCreate(go, uiNode, bridge, parent, parentPanel);
                }
                else
                {
                    throw new Exception("资源为空！");
                }
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public abstract class PanelGroupBase : MonoBehaviour, IPanelGroup
    {
        [SerializeField]
        private BundleLoader bundleCreateRule;

        #region Propertys
        public Transform Trans
        {
            get { return transform; }
        }
        public UIBindingController BindingCtrl
        {
            get
            {
                return bindingCtrl;
            }
        }
        public abstract List<BridgeInfo> Bridges { get; }
        public abstract Dictionary<string, UIInfoBase> Nodes { get; }

        #endregion

        #region Protected
        protected BridgePool bridgePool;
        protected List<IUIPanel> createdPanels;
        protected Dictionary<string, List<IUIPanel>> panelPool;
        protected List<Bridge> createdBridges;
        protected Dictionary<IUIPanel, Stack<IUIPanel>> hidedPanelStack;
        protected Dictionary<int, IUIPanel> transRefDic;//通过transform 的 id 查找UIPanel
        protected PanelCreateRule createRule;
        protected event UnityAction onDestroy;
        protected UIBindingController bindingCtrl;
        protected Dictionary<Transform, Dictionary<int, Transform>> transDicCatch;
        #endregion

        protected virtual void Awake()
        {
            bridgePool = new BridgePool();
            transDicCatch = new Dictionary<Transform, Dictionary<int, Transform>>();
            createdPanels = new List<IUIPanel>();
            panelPool = new Dictionary<string, List<IUIPanel>>();
            createdBridges = new List<Bridge>();
            hidedPanelStack = new Dictionary<IUIPanel, Stack<IUIPanel>>();
            transRefDic = new Dictionary<int, IUIPanel>();
            createRule = new PanelCreateRule(bundleCreateRule);
            bindingCtrl = new UIBindingController();
            Utility.RegistPanelGroup(this);
        }

        protected virtual void OnDestroy()
        {
            if (onDestroy != null)
            {
                onDestroy.Invoke();
            }

            Utility.RemovePanelGroup(this);
        }

        protected void LunchPanelGroupSystem()
        {
            TryAutoOpen();
            RegistUIEvents();
        }

        public bool CreateInfoAndBridge(string panelName, IUIPanel parentPanel, int index, UIInfoBase uiInfo, out Bridge bridgeObj)
        {
            if (uiInfo == null)
            {
                bridgeObj = null;
                return false;
            }

            bridgeObj = GetBridgeClamp(parentPanel, panelName, index);
            createdBridges.Add(bridgeObj);
            return uiInfo != null && bridgeObj != null;
        }

        public bool TryOpenOldPanel(string panelName, UIInfoBase uiNode, IUIPanel parentPanel, out Bridge bridgeObj)
        {
            bridgeObj = null;

            if (uiNode == null)
            {
                return false;
            }

            var oldPanels = createdPanels.FindAll(x => x.Name == panelName);

            for (int i = 0; i < oldPanels.Count; i++)
            {
                var oldPanel = oldPanels[i];

                if (oldPanel != null)
                {
                    bridgeObj = createdBridges.Find(x => x.OutPanel == oldPanel);

                    if (bridgeObj != null)
                    {
                        if (oldPanel.UType.form == UIFormType.Fixed)
                        {
                            bridgeObj.SetInPanel(parentPanel);

                            if (parentPanel != null)
                                parentPanel.RecordChild(oldPanel);

                            if (!oldPanel.IsShowing)
                            {
                                oldPanel.UnHide();
                            }
                            HandBridgeOptions(oldPanel, bridgeObj);
                            return true;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Debug.LogError("目标面板信息丢失,请检查逻辑！！！" + parentPanel);
                        for (int j = 0; j < createdBridges.Count; j++)
                        {
                            var item = createdBridges[j];
                            Debug.Log(item.OutPanel.Name);
                        }
                    }
                }
            }

            return false;
        }

        public void CreatePanel(UIInfoBase uiNode, Bridge bridge, IUIPanel parentPanel)
        {
            Transform root = parentPanel == null ? Trans : parentPanel.Content;

            var createUIHandle = new UICreateHandle();
            createUIHandle.parentPanel = parentPanel;
            createUIHandle.uiNode = uiNode;
            createUIHandle.bridge = bridge;
            createUIHandle.parent = root;
            createUIHandle.onCreate = CreateUI_Internal;
            createRule.CreatePanel(uiNode, createUIHandle.OnCreate);
        }

        private void CreateUI_Internal(GameObject go, UIInfoBase uiNode, Bridge bridge, Transform parent, IUIPanel parentPanel)
        {
            if (go == null) return;

            var parentDic = GetParentDic(parent);

            Utility.SetTranform(go.transform, uiNode.type.layer, uiNode.type.layerIndex, Trans, parent, parentDic, transRefDic);

            go.name = uiNode.panelName;
            go.SetActive(true);

            IUIPanel panel = GetPanelFromPool(go);
            InitPanelInformation(panel, uiNode);
            panel.Binding(go);
            panel.HandleData(bridge);
            transRefDic.Add(go.transform.GetInstanceID(), panel);
            createdPanels.Add(panel);

            if (parentPanel != null)
            {
                parentPanel.RecordChild(panel);
            }

            if (bridge != null)
            {
                bridge.OnCreatePanel(panel);
            }

            HandBridgeOptions(panel, bridge);
        }


        public void CansaleInstencePanel(string panelName)
        {
            createRule.CansaleCreatePanel(panelName);
        }

        public List<IUIPanel> RetrivePanels(string panelName)
        {
            var panels = createdPanels.FindAll(x => x.Name == panelName);
            return panels;
        }

        #region protected Functions

        /// <summary>
        /// 处理面板打开规则
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="bridge"></param>
        protected void HandBridgeOptions(IUIPanel panel, Bridge bridge)
        {
            Debug.Assert(bridge != null, "信息不应当为空，请检查！");
            TryChangeParentState(panel, bridge.Info);
            TryHandleMutexPanels(panel, bridge.Info);
            TryHideGroup(panel, bridge.Info);
            TryAutoOpen(panel);
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
                    var parentDic = GetParentDic(Trans);
                    panel.SetParent(Trans, parentDic, transRefDic);
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
        protected void TryHandleMutexPanels(IUIPanel childPanel, BridgeInfo bridge)
        {
            if (bridge.showModel.mutex != MutexRule.NoMutex)
            {
                if (bridge.showModel.mutex == MutexRule.SameParentAndLayer)
                {
                    var mayBridges = Bridges.FindAll(x => x.inNode == bridge.inNode);

                    foreach (var bg in mayBridges)
                    {
                        if (bg.showModel.mutex != MutexRule.SameParentAndLayer) continue;

                        var mayPanels = createdPanels.FindAll(x =>
                        x.Name == bg.outNode &&
                        x.UType.layer == childPanel.UType.layer &&
                        x != childPanel &&
                        !IsChildOfPanel(childPanel, x));

                        foreach (var mayPanel in mayPanels)
                        {
                            if (mayPanel != null && mayPanel.IsShowing)
                            {
                                if (mayPanel.UType.layerIndex > childPanel.UType.layerIndex)
                                {
                                    HidePanelInteral(mayPanel, childPanel);
                                }
                                else
                                {
                                    HidePanelInteral(childPanel, mayPanel);
                                }
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
                            if (mayPanel.UType.layerIndex > childPanel.UType.layerIndex)
                            {
                                HidePanelInteral(mayPanel, childPanel);
                            }
                            else
                            {
                                HidePanelInteral(childPanel, mayPanel);
                            }
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
        protected void TryAutoOpen(IUIPanel parentPanel = null)
        {
            var panelName = parentPanel == null ? "" : parentPanel.Name;
            var autoBridges = Bridges.FindAll(x => CompareName(x.inNode, panelName) && x.showModel.auto);
            if (autoBridges != null)
            {
                foreach (var autoBridge in autoBridges)
                {
                    panelName = autoBridge.outNode;

                    UIInfoBase uiNode;

                    Nodes.TryGetValue(panelName, out uiNode);

                    if (uiNode == null)
                    {
                        Debug.LogError("无配制信息：" + panelName);
                        continue;
                    }

                    Bridge bridge;
                    if (!TryOpenOldPanel(panelName, uiNode, parentPanel, out bridge))
                    {
                        if (CreateInfoAndBridge(panelName, parentPanel, -1, uiNode, out bridge))
                        {
                            CreatePanel(uiNode, bridge, parentPanel);
                        }
                        else
                        {
                            Debug.LogError("找不到信息：" + panelName);
                        }
                    }
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
            return string.Compare(nameA, nameB) == 0;
        }

        /// <summary>
        /// 隐藏面板
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
        protected void InitPanelInformation(IUIPanel panel, UIInfoBase uiNode)
        {
            panel.UType = uiNode.type;
            panel.Group = this;
            panel.onClose += OnDeletePanel;
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
                    var parentDic = GetParentDic(parent.Root);
                    panel.SetParent(parent.Root, parentDic, transRefDic);
                    HidePanelInteral(panel, parent);
                }
            }

            if (bridge.showModel.baseShow == BaseShow.Destroy)
            {
                var parent = panel.Parent;
                if (parent != null && parent.ChildPanels.Count > 0)
                {
                    var parentDic = GetParentDic(parent.Root);
                    panel.SetParent(parent.Root, parentDic, transRefDic);
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
        /// 获取可用的bridge
        /// </summary>
        /// <param name="parentPanel"></param>
        /// <param name="panelName"></param>
        /// <returns></returns>
        protected Bridge GetBridgeClamp(IUIPanel parentPanel, string panelName, int index)
        {
            Bridge bridge = null;
            var parentName = parentPanel == null ? "" : parentPanel.Name;
            var mayInfos = Bridges.FindAll(x => x.outNode == panelName && (x.index == index || index == -1));//所有可能的
            var baseInfos = mayInfos.FindAll(x => x.inNode == parentName);//所有父级名相同的
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
                var show = new ShowMode();
                var info = new BridgeInfo(parentName, panelName, show, 0);
                bridge = bridgePool.Allocate(info, parentPanel);
            }
            else
            {
                bridge = bridgePool.Allocate((BridgeInfo)bridgeInfo, parentPanel);
            }
            return bridge;
        }

        /// <summary>
        /// 当删除一个面板时触发一些事
        /// </summary>
        /// <param name="panel"></param>
        protected void OnDeletePanel(IUIPanel panel, bool remove)
        {
            if (remove)
            {
                //移到缓存池
                RecoverPanel(panel);

                //移除id字典
                if (panel.UType.closeRule != CloseRule.HideGameObject)
                {
                    //移除连通器
                    createdBridges.RemoveAll(x => x.OutPanel == panel);

                    if (panel.Content && panel.Content != Trans)
                    {
                        transDicCatch.Remove(panel.Content);
                    }
                }
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
                panel.ChildPanels.Clear();
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

        /// <summary>
        /// 从对象池或创建代码
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public IUIPanel GetPanelFromPool(GameObject target)
        {
            var panelName = target.name;
            IUIPanel panel = null;
            List<IUIPanel> panels = null;
            if (!panelPool.TryGetValue(panelName, out panels))
            {
                panelPool[panelName] = panels = new List<IUIPanel>();
            }

            if (panels.Count > 0)
            {
                panel = panels[0];
                panels.RemoveAt(0);
            }

            if (panel == null)
            {
                var panelRef = target.GetComponent<IUIPanelReference>();

                if (panelRef != null)
                {
                    var type = panelRef.CetPanelScriptType();
                    if (type != null)
                    {
                        panel = System.Activator.CreateInstance(type) as IUIPanel;
                    }
                }

                if (panel == null)
                {
                    panel = new ViewBase_Diffuse();
                }
            }
            return panel;
        }

        /// <summary>
        /// 回收面板脚本
        /// </summary>
        /// <param name="panel"></param>
        public void RecoverPanel(IUIPanel panel)
        {
            if (createdPanels.Contains(panel))
            {
                createdPanels.Remove(panel);
            }
            var panelName = panel.Name;
            List<IUIPanel> panels = null;
            if (!panelPool.TryGetValue(panelName, out panels))
            {
                panelPool[panelName] = panels = new List<IUIPanel>();
            }
            panels.Add(panel);
        }
        protected Dictionary<int, Transform> GetParentDic(Transform parent)
        {
            if (parent == null) return null;

            if (!transDicCatch.ContainsKey(parent))
            {
                transDicCatch[parent] = new Dictionary<int, Transform>();
            }

            return transDicCatch[parent];
        }

        #region 图形化界面关联
        /// <summary>
        /// 注册ui事件
        /// </summary>
        protected void RegistUIEvents()
        {

            foreach (var item in Bridges)
            {
                var bridgeInfo = item;

                if (!string.IsNullOrEmpty(bridgeInfo.inNode) && !string.IsNullOrEmpty(bridgeInfo.outNode))
                {
                    UIBindingItem bindingItem = new UIBindingItem();

                    var index = item.index;

                    bindingItem.openAction = (x, y) =>
                    {
                        var parentPanel = x;
                        var panelName = bridgeInfo.outNode;
                        UIFacade.Instence.Open(parentPanel, panelName, index, y);
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


        #endregion protected Functions



    }
}