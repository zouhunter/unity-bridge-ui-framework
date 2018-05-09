#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 11:27:06
    * 说    明：       
* ************************************************************************************/
#endregion
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
using System;
using BridgeUI.Model;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using BridgeUI.Binding;

namespace BridgeUI
{
    [PanelParent]
    public abstract class PanelBase : UIBehaviour, IPanelBase, Binding.IBindingContext
    {
        public int InstenceID
        {
            get
            {
                return GetInstanceID();
            }
        }
        public string Name { get { return name; } }
        public IPanelGroup Group { get; set; }
        public IPanelBase Parent { get; set; }
        public virtual Transform Content { get { return transform; } }
        public Transform Root { get { return transform.parent.parent; } }
        public UIType UType { get; set; }
        public List<IPanelBase> ChildPanels
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
                return _isShowing && !IsDestroyed();
            }
        }
        public bool IsAlive
        {
            get
            {
                return _isAlive && !IsDestroyed();
            }
        }
        private IAnimPlayer _enterAnim;
        protected IAnimPlayer enterAnim
        {
            get
            {
                if (_enterAnim == null)
                {
                    var enterAnimType = UType.enterAnim.type;
                    if (enterAnimType != null)
                    {
                        _enterAnim = gameObject.GetComponent(enterAnimType) as IAnimPlayer;
                        if (_enterAnim == null)
                        {
                            _enterAnim = gameObject.AddComponent(enterAnimType) as IAnimPlayer;
                        }
                    }

                }
                return _enterAnim;

            }
        }
        private IAnimPlayer _quitAnim;
        protected IAnimPlayer quitAnim
        {
            get
            {
                if (_quitAnim == null)
                {
                    var quitAnimType = UType.quitAnim.type;
                    if (quitAnimType != null)
                    {
                        _quitAnim = gameObject.GetComponent(quitAnimType) as IAnimPlayer;
                        if (_quitAnim == null)
                        {
                            _quitAnim = gameObject.AddComponent(quitAnimType) as IAnimPlayer;
                        }
                    }

                }
                return _quitAnim;

            }
        }


        protected Bridge bridge;
        protected List<IPanelBase> childPanels;
        public event UnityAction<IPanelBase> onDelete;
        public event PropertyChangedHand onPropertyChanged;

        private bool _isShowing = true;
        private bool _isAlive = true;
        private Binding.PropertyBinder _binder;
        protected virtual Binding.PropertyBinder Binder
        {
            get
            {
                if (_binder == null)
                {
                    _binder = new Binding.PropertyBinder(this);
                }
                return _binder;
            }
        }
        private Binding.ViewModelBase _viewModel;
        private Binding.ViewModelBase _defultViewModel;
        protected Binding.ViewModelBase defultViewModel
        {
            get
            {
                if (_defultViewModel == null)
                    _defultViewModel = new ViewModelBase();
                return _defultViewModel;
            }
        }
        public Binding.ViewModelBase BindingContext
        {
            get
            {
                return _viewModel;
            }
            set
            {
                _viewModel = value;
                OnBindingContextChanged(_viewModel);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            InitComponents();
            PropBindings();
        }
        protected override void Start()
        {
            base.Start();
            if (bridge != null)
            {
                bridge.OnCreatePanel(this);
            }
            AppendComponentsByType();
            OnOpenInternal();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _isAlive = false;
            _isShowing = false;
            if (bridge != null)
            {
                bridge.Release();
            }

            if (onDelete != null)
            {
                onDelete.Invoke(this);
            }

            Binder.Unbind();
        }
        protected virtual void InitComponents()
        {

        }
        protected virtual void PropBindings()
        {

        }
        protected void OnPropertyChanged(string memberName)
        {
            if (onPropertyChanged != null)
                onPropertyChanged.Invoke(memberName);
        }

        public virtual void OnBindingContextChanged(Binding.ViewModelBase newValue)
        {
            Binder.Unbind();
            Binder.Bind(newValue);
        }
        public void SetParent(Transform Trans)
        {
            Utility.SetTranform(transform, UType.layer, UType.layerIndex, Trans);
        }
        public void CallBack(object data)
        {
            if (bridge != null)
            {
                bridge.CallBack(this, data);
            }
        }
        public void HandleData(Bridge bridge)
        {
            this.bridge = bridge;
            if (bridge != null)
            {
                HandleData(bridge.dataQueue);
                bridge.onGet = HandleData;
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
            if (data is Binding.ViewModelBase)
            {
                BindingContext = data as Binding.ViewModelBase;
            }
            else
            {
                if (data is IDictionary)
                {
                    LoadIDictionary(data as IDictionary);
                }
            }
        }
        private void LoadIDictionary(IDictionary data)
        {
            BindingContext = defultViewModel;
            foreach (var item in data.Keys)
            {
                var key = item.ToString();
                var fullType = typeof(B_Property<>).MakeGenericType(data[item].GetType());
                var prop = BindingContext.GetUsefulBindTarget(key, fullType);
                if (prop != null)
                {
                    prop.Target = data[item];
                }
            }
        }
        public virtual void Hide()
        {
            _isShowing = false;
            switch (UType.hideRule)
            {
                case HideRule.AlaphGameObject:
                    AlaphGameObject(true);
                    break;
                case HideRule.HideGameObject:
                    gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
        public virtual void UnHide()
        {
            switch (UType.hideRule)
            {
                case HideRule.AlaphGameObject:
                    AlaphGameObject(false);
                    break;
                case HideRule.HideGameObject:
                    gameObject.SetActive(true);
                    break;
                default:
                    break;
            }

            _isShowing = true;
            OnOpenInternal();
        }
        public virtual void Close()
        {
            if (IsShowing && UType.quitAnim.type != null)
            {
                quitAnim.PlayAnim(false, CloseInternal);
            }
            else
            {
                CloseInternal();
            }
        }
        private void CloseInternal()
        {
            _isShowing = false;

            switch (UType.closeRule)
            {
                case CloseRule.DestroyImmediate:
                    DestroyImmediate(gameObject);
                    break;
                case CloseRule.DestroyDely:
                    Destroy(gameObject, 0.02f);
                    break;
                case CloseRule.DestroyNoraml:
                    Destroy(gameObject);
                    break;
                case CloseRule.HideGameObject:
                    Hide();
                    break;
                default:
                    break;
            }
        }
        public void RecordChild(IPanelBase childPanel)
        {
            if (childPanels == null)
            {
                childPanels = new List<IPanelBase>();
            }
            if (!childPanels.Contains(childPanel))
            {
                childPanel.onDelete += OnRemoveChild;
                childPanels.Add(childPanel);
            }
            childPanel.Parent = this;
        }
        private void AppendComponentsByType()
        {
            if (UType.form == UIFormType.DragAble)
            {
                if (gameObject.GetComponent<DragPanel>() == null)
                {
                    gameObject.AddComponent<DragPanel>();
                }
            }
        }
        public void Cover()
        {
            var covername = Name + "_Cover";
            var rectt = new GameObject(covername, typeof(RectTransform)).GetComponent<RectTransform>();
            rectt.gameObject.layer = 5;
            rectt.SetParent(transform, false);
            rectt.SetSiblingIndex(0);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 10000);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10000);
            var img = rectt.gameObject.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0.01f);
            img.raycastTarget = true;
        }
        private void OnRemoveChild(IPanelBase childPanel)
        {
            if (childPanels != null && childPanels.Contains(childPanel))
            {
                childPanels.Remove(childPanel);
            }
        }
        private void OnOpenInternal()
        {
            if (UType.enterAnim.type != null)
            {
                enterAnim.PlayAnim(true, null);
            }
        }
        private void AlaphGameObject(bool hide)
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
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
    }
}