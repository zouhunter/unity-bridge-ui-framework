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

namespace BridgeUI
{
    public abstract class PanelBase : UIBehaviour, IPanelBase
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
        public abstract Transform Content { get; }
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

        protected UIFacade selfFacade;
        protected Bridge bridge;
        protected List<IPanelBase> childPanels;
        public event UnityAction<IPanelBase> onDelete;
        protected IAnimPlayer animPlayer;
        private bool _isShowing = true;
        private bool _isAlive = true;
        private  bool autoCharge;
        private Dictionary<string, MemberInfo> fieldDic;
        protected UnityAction onChargeComplete { get; set; }
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
                    if(data != null){
                        HandleData(data);
                    }
                }
            }
        }

        protected virtual void HandleData(object data)
        {
            if (data is Hashtable && autoCharge){
                LoadData(data as Hashtable);
            }
        }

        private void InitChargeDic()
        {
            if (fieldDic == null)
            {
                var fields = this.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty);
                if (fields.Length > 0) fieldDic = new Dictionary<string, MemberInfo>();
                foreach (var field in fields)
                {
                    var atts = field.GetCustomAttributes(typeof(Charge), true);
                    if (atts.Length > 0)
                    {
                        var key = (atts[0] as Charge).key;
                        if(string.IsNullOrEmpty(key)){
                            key = field.Name;
                        }
                        fieldDic.Add(key, field);
                        autoCharge = true;
                    }
                }
            }
          
        }
        private void LoadData(Hashtable data)
        {
            if (fieldDic != null)
            {
                foreach (var item in data.Keys)
                {
                    var key = (string)item;
                    if (fieldDic.ContainsKey(key))
                    {
                        var member = fieldDic[key];
                        if (member is FieldInfo)
                        {
                            (member as FieldInfo).SetValue(this, data[item]);
                        }
                        else if (member is PropertyInfo)
                        {
                            (member as PropertyInfo).SetValue(this, data[item], new object[0]);
                        }
                    }
                }
            }

        }
        protected override void Awake()
        {
            base.Awake();
            selfFacade = UIFacade.CreatePanelFacade(this);
            InitChargeDic();
        }
        protected override void Start()
        {
            base.Start();
            AppendComponentsByType();
            if (bridge != null)
            {
                bridge.OnCreatePanel(this);
            }
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
            _isShowing = true;
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
        }

        public virtual void Close()
        {
            if (IsShowing && UType.quitAnim != UIAnimType.NoAnim)
            {
                var tweenPanel = GetComponent<AnimPlayer>();
                if (tweenPanel == null)
                {
                    tweenPanel = gameObject.AddComponent<AnimPlayer>();
                }
                tweenPanel.QuitAnim(UType.quitAnim, CloseInternal);
            }
            else
            {
                CloseInternal();
            }
        }

        private void CloseInternal()
        {
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
                    gameObject.SetActive(false);
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
        }

        private void AppendComponentsByType()
        {
            if (UType == null)
            {
                UType = new UIType();
            }
            if (UType.form == UIFormType.DragAble)
            {
                if (gameObject.GetComponent<DragPanel>() == null)
                {
                    gameObject.AddComponent<DragPanel>();
                }
            }

            if (UIAnimType.NoAnim != UType.enterAnim)
            {
                animPlayer = GetComponent<AnimPlayer>();
                if (animPlayer == null){
                    animPlayer = gameObject.AddComponent<AnimPlayer>();
                }
                animPlayer.EnterAnim(UType.enterAnim, null);
            }

        }
        public void Cover()
        {
            var covername = Name + "_Cover";
            var trans = new GameObject(covername).transform;
            trans.SetParent(transform, false);
            trans.SetSiblingIndex(0);
            var rectt = trans.gameObject.AddComponent<RectTransform>();
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

        private void AlaphGameObject(bool hide)
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null){
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