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
    public abstract class PanelBase : UIBehaviour, IUIPanel, IBindingContext
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
        public IUIPanel Parent { get; set; }
        public virtual Transform Content { get { return transform; } }
        public Transform Root { get { return transform.parent.parent; } }
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
        private AnimPlayer _enterAnim;
        protected AnimPlayer enterAnim
        {
            get
            {
                if (_enterAnim == null)
                {
                    _enterAnim = UType.enterAnim;
                    _enterAnim.SetContext(this);
                }
                return _enterAnim;
            }
        }
        private AnimPlayer _quitAnim;
        protected AnimPlayer quitAnim
        {
            get
            {
                if (_quitAnim == null)
                {
                    _quitAnim = UType.quitAnim;
                    _quitAnim.SetContext(this);
                }
                return _quitAnim;

            }
        }


        protected Bridge bridge;
        protected List<IUIPanel> childPanels;
        public event PanelCloseEvent onDelete;

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
        [SerializeField,Attributes. DefultViewModel]
        private Binding.ViewModel _viewModel;
        private Binding.ViewModel _defultViewModel;
        protected Binding.ViewModel defultViewModel
        {
            get
            {
                if (_defultViewModel == null)
                {
                    _defultViewModel = ScriptableObject.CreateInstance<ViewModel>();
                }
                return _defultViewModel;
            }
        }
        public Binding.ViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
            set
            {
                _viewModel = value;
                OnViewModelChanged(_viewModel);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            InitComponents();
            PropBindings();

            if (_viewModel != null){
                OnViewModelChanged(_viewModel);
            }
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
                onDelete.Invoke(this, true);
            }

            Binder.Unbind();
        }
        protected virtual void InitComponents() { }

        protected virtual void PropBindings() { }

        public virtual void OnViewModelChanged(Binding.ViewModel newValue)
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
            if (data is Binding.ViewModel)
            {
                ViewModel = data as Binding.ViewModel;
            }
            else if (data is IDictionary)
            {
                var currentViewModel = ViewModel == null ? defultViewModel:ViewModel;
                LoadPropDictionary(currentViewModel, data as IDictionary);
                ViewModel = currentViewModel;
            }
        }
        protected virtual void LoadPropDictionary(ViewModel viewModel,IDictionary dataDic)
        {
            var keys = dataDic.Keys;
            foreach (var key in keys)
            {
                var value = dataDic[key];
                if(value != null)
                {
                    var prop = viewModel.GetBindablePropertySelfty(key.ToString(), value.GetType());
                    if(prop != null)
                    {
                        prop.ValueBoxed = value;
                    }
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

        public virtual void OnDestroyHide()
        {
            _isShowing = false;
            gameObject.SetActive(false);
            if (onDelete != null)
            {
                onDelete.Invoke(this, false);
            }
        }

        public virtual void UnHide()
        {
            gameObject.SetActive(true);
            if(UType.hideRule == HideRule.AlaphGameObject) {
                AlaphGameObject(false);
            }
            _isShowing = true;
            OnOpenInternal();
        }
        public virtual void Close()
        {
            if (IsShowing && UType.quitAnim != null)
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
                    OnDestroyHide();
                    break;
                default:
                    break;
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
        private void OnRemoveChild(IUIPanel childPanel, bool remove)
        {
            if (childPanels != null && childPanels.Contains(childPanel) && remove)
            {
                childPanels.Remove(childPanel);
            }
        }
        private void OnOpenInternal()
        {
            if (UType.enterAnim != null)
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