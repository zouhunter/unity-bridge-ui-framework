using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

namespace BridgeUI
{
    [BridgeUI.Attributes.PanelParent]
    public class ViewBase : IUIPanel
    {
        public event PanelCloseEvent onClose;

        #region Propertys
        public GameObject Target { get { return target; } }
        public int InstenceID { get { return target.GetInstanceID(); } }
        public string Name { get { return name; } }
        public virtual Transform Content { get { return Group.Trans; } }

        public IPanelGroup Group
        {
            get
            {
                if (group == null)
                    group = target.GetComponentInParent<IPanelGroup>();
                return group;
            }
            set { group = value; }
        }
        public IUIPanel Parent { get; set; }
        public Transform Root { get { return target.transform.parent.parent; } }
        public UIType UType { get; set; }
        public List<IUIPanel> ChildPanels
        {
            get
            {
                return childPanels;
            }
        }
        public bool IsShowing
        {
            get
            {
                return target && target.activeSelf && !hide;
            }
        }
        public bool IsAlive
        {
            get
            {
                return target && viewBaseBinding;
            }
        }
        #endregion

        protected GameObject target;
        protected IPanelGroup group;
        protected Model.Bridge bridge;
        protected List<IUIPanel> childPanels;
        protected event UnityAction<object> onReceive;
        protected Dictionary<int, Transform> childDic;
        protected List<IUIControl> uiControls;
        protected DestroyMonitor viewBaseBinding;
        protected bool hide;
        private string name;

        #region BridgeAPI

        public void Binding(GameObject target)
        {
            if (this.target != target && target != null)
            {
                if (this.target)
                    OnUnBinding(target);
                OnBinding(target);
                AppendComponentsByType();
                OnOpenInternal();
                TryMakeCover();
            }
        }

        public void RegistOnRecevie(UnityAction<object> onReceive)
        {
            this.onReceive += onReceive;
        }

        public void RemoveOnRecevie(UnityAction<object> onReceive)
        {
            this.onReceive += onReceive;
        }

        public void SetParent(Transform Trans,Dictionary<int,Transform> transDic,Dictionary<int,IUIPanel> transRefDic)
        {
            Utility.SetTranform(target.transform, UType.layer, UType.layerIndex, group.Trans, Trans, transDic, transRefDic);
        }

        public void CallBack(object data)
        {
            if (bridge != null)
            {
                bridge.CallBack(this, data);
            }
        }

        public void HandleData(Model.Bridge bridge)
        {
            this.bridge = bridge;
            if (bridge != null)
            {
                HandleData(bridge.dataQueue);
                bridge.onGet = HandleData;
            }
        }

        public void Hide()
        {
            hide = true;
            switch (UType.hideRule)
            {
                case HideRule.AlaphGameObject:
                    AlaphGameObject(true);
                    break;
                case HideRule.HideGameObject:
                    target.SetActive(false);
                    break;
                default:
                    break;
            }

            OnRelease();
        }

        public void OnDestroyHide()
        {
            target.SetActive(false);
            CloseCallBack(false);
        }

        public void UnHide()
        {
            hide = false;
            target.SetActive(true);
            if (UType.hideRule == HideRule.AlaphGameObject)
            {
                AlaphGameObject(false);
            }
            OnInitialize();
            OnOpenInternal();
        }

        public virtual void Close()
        {
            if (IsShowing && UType.quitAnim != null)
            {
                var quitAnim = Object.Instantiate(UType.quitAnim);
                quitAnim.SetContext(target.GetComponent<MonoBehaviour>());
                quitAnim.PlayAnim(OnCloseInternal);
            }
            else
            {
                OnCloseInternal();
            }
        }

        public void RecordChild(IUIPanel childPanel)
        {
            if (childPanels == null)
            {
                childPanels = new List<IUIPanel>();
            }
            if (!childPanels.Contains(childPanel))
            {
                childPanel.onClose += OnRemoveChild;
                childPanels.Add(childPanel);
            }
            childPanel.Parent = this;
        }

        public Image Cover()
        {
            var covername = Name + "_Cover";
            var rectt = new GameObject(covername, typeof(RectTransform)).GetComponent<RectTransform>();
            rectt.gameObject.layer = 5;
            rectt.SetParent(target.transform, false);
            rectt.SetSiblingIndex(0);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 10000);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10000);
            var img = rectt.gameObject.AddComponent<Image>();
            img.color = UType.maskColor;
            img.raycastTarget = true;
            return img;
        }
        #endregion

        #region Extend Of Open Close
        public void Open(string panelName, object data = null)
        {
            UIFacade.Instence.Open(this, panelName, data);
        }

        public void Open(int index, object data = null)
        {
            Group.BindingCtrl.OpenRegistedPanel(this, index, data);
        }
        public void Hide(string panelName)
        {
            UIFacade.Instence.Hide(Group, panelName);
        }
        public void Hide(int index)
        {
            Group.BindingCtrl.HideRegistedPanel(this, index);
        }

        public void Close(string panelName)
        {
            UIFacade.Instence.Close(Group, panelName);
        }
        public void Close(int index)
        {
            Group.BindingCtrl.CloseRegistedPanel(this, index);
        }
        public bool IsOpen(int index)
        {
            return Group.BindingCtrl.IsRegistedPanelOpen(this, index);
        }
        public bool IsOpen(string panelName)
        {
            var panels = Group.RetrivePanels(panelName);
            return (panels != null && panels.Count > 0);
        }
        #endregion

        #region Protected
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="target"></param>
        protected virtual void OnBinding(GameObject target)
        {
            this.target = target;
            name = target.name;
            viewBaseBinding = target.AddComponent<DestroyMonitor>();
            viewBaseBinding.onDestroy = OnTargetDestroy;
        }
        /// <summary>
        /// 去除绑定
        /// </summary>
        /// <param name="target"></param>
        protected virtual void OnUnBinding(GameObject target)
        {
            if (target)
            {
                viewBaseBinding = target.GetComponent<DestroyMonitor>();
                if (viewBaseBinding){
                    GameObject.Destroy(viewBaseBinding);
                }
            }
            this.target = null;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void OnInitialize()
        {
            InitializeChildControls();
        }
        /// <summary>
        /// 1.(关闭，丢失)删除对象
        /// 2.(关闭)隐藏对象
        /// </summary>
        protected virtual void OnRelease()
        {
            if (UType.closeRule != CloseRule.HideGameObject && bridge != null)
            {
                bridge.Release();
            }
            OnReleaseChildContrls();
        }

    

        protected virtual void OnTargetDestroy()
        {
            OnUnBinding(target);
            OnRelease();
            CloseCallBack(true);
        }

        protected virtual void CloseCallBack(bool destory)
        {
            if (onClose != null)
            {
                onClose.Invoke(this, destory);
            }
        }

        protected void HandleData(Queue<object> dataQueue)
        {
            if (dataQueue != null)
            {
                while (dataQueue.Count > 0)
                {
                    var data = dataQueue.Dequeue();
                    if (data != null)
                    {
                        HandleData(data);
                    }
                }
            }
        }

        protected virtual void HandleData(object data)
        {
            if (this.onReceive != null)
            {
                onReceive.Invoke(data);
            }
        }
        protected void OnOpenInternal()
        {
            hide = false;

            if (UType.enterAnim != null)
            {
                var enterAnim = Object.Instantiate(UType.enterAnim);
                var behaiver = target.GetComponent<MonoBehaviour>();
                Debug.Log(behaiver);

                enterAnim.SetContext(behaiver);
                enterAnim.PlayAnim(null);
            }
            OnInitialize();
        }

        protected void OnCloseInternal()
        {
            switch (UType.closeRule)
            {
                case CloseRule.DestroyImmediate:
                    Object.DestroyImmediate(target);
                    break;
                case CloseRule.DestroyDely:
                    Object.Destroy(target, 0.02f);
                    break;
                case CloseRule.DestroyNoraml:
                    Object.Destroy(target);
                    break;
                case CloseRule.HideGameObject:
                    OnDestroyHide();
                    break;
                default:
                    break;
            }


            OnRelease();
        }

        protected void AppendComponentsByType()
        {
            if (UType.form == UIFormType.DragAble)
            {
                if (target.GetComponent<DragPanel>() == null)
                {
                    target.AddComponent<DragPanel>();
                }
            }
        }

        protected void OnRemoveChild(IUIPanel childPanel, bool remove)
        {
            if (childPanels != null && childPanels.Contains(childPanel) && remove)
            {
                childPanels.Remove(childPanel);
            }
        }

        /// <summary>
        /// 建立遮罩
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="info"></param>
        protected void TryMakeCover()
        {
            switch (UType.cover)
            {
                case UIMask.None:
                    break;
                case UIMask.Normal:
                    Cover();
                    break;
                case UIMask.ClickClose:
                    var img = Cover();
                    var btn = img.gameObject.AddComponent<Button>();
                    btn.onClick.AddListener(Close);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 将面板透明处理
        /// </summary>
        /// <param name="hide"></param>
        protected void AlaphGameObject(bool hide)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
            }

            if (hide)
            {
                canvasGroup.alpha = UType.hideAlaph;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }

        /// <summary>
        /// 注册UI子控件
        /// </summary>
        /// <param name="control"></param>
        protected void RegistUIControl(BridgeUI.IUIControl control)
        {
            if (this.uiControls == null)
            {
                uiControls = new List<IUIControl>() { control };
            }
            else if (!uiControls.Contains(control))
            {
                uiControls.Add(control);
            }
        }

        #endregion

        #region Private Functions
        private void InitializeChildControls()
        {
            if (uiControls != null)
            {
                using (var enumerator = uiControls.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (!enumerator.Current.Initialized)
                        {
                            enumerator.Current.Initialize(this);
                        }
                    }
                }
            }
        }

        private void OnReleaseChildContrls()
        {
            if (uiControls != null)
            {
                using (var enumerator = uiControls.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.Initialized)
                        {
                            enumerator.Current.Recover();
                        }
                    }
                }
            }
        }
        #endregion Private Functions

    }
}