using System;
using System.Collections;
using System.Collections.Generic;
using BridgeUI.Model;
using UnityEngine;
using UnityEngine.Scripting;

namespace BridgeUI
{

    public class ViewBase_Diffuse : ViewBase
    {
        private ICustomUI component;
        public override Transform Content
        {
            get
            {
                if (component == null|| component.Content == null)
                    return base.Content;
                return component.Content;
            }
        }

        protected override void OnBinding(GameObject target)
        {
            base.OnBinding(target);
            component = target.GetComponent<ICustomUI>();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            if (component != null)
            {
                if (component is BridgeUI.IDataReceiver)
                {
                    RegistOnRecevie((component as BridgeUI.IDataReceiver).HandleData);
                }
                component.Initialize(this);
            }
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            if (component != null)
            {
                if (component is BridgeUI.IDataReceiver)
                {
                    RemoveOnRecevie((component as BridgeUI.IDataReceiver).HandleData);
                }
                component.Recover();
            }
        }
    }

    public abstract class ViewBaseComponent : MonoBehaviour, IDataReceiver, ICustomUI, IDiffuseView
    {
        protected IUIPanel view;
        protected bool Initialized;

        public virtual Transform Content
        {
            get
            {
                return null;
            }
        }

        protected virtual void Start()
        {
            if(view == null)
            {
                var panel = new ViewBase_Diffuse();
                panel.Binding(gameObject);
            }
        }

        public void Initialize(IUIPanel diffusePanel)
        {
            if (!Initialized)
            {
                this.view = diffusePanel;
                Initialized = true;
                OnInitialize();
            }
        }

        public void Recover()
        {
            if (Initialized)
            {
                Initialized = false;
                OnRecover();
                view = null;
            }
        }

        protected abstract void OnInitialize();

        protected abstract void OnRecover();

        protected virtual void OnMessageReceive(object message)
        {
        }

        public void HandleData(object arg0)
        {
            if (Initialized)
            {
                OnMessageReceive(arg0);
            }
        }

        public virtual void Close()
        {
            if (Initialized)
            {
                view.Close();
            }
        }

        public virtual void CallBack(object arg0)
        {
            if (Initialized)
            {
                view.CallBack(arg0);
            }
        }

        public virtual void Close(int v)
        {
            if (Initialized)
            {
                view.Close(v);
            }
        }

        public virtual void Open(int v1, object v2)
        {
            if (Initialized)
            {
                view.Open(v1, v2);
            }
        }

        public virtual void Open(string v1, object v2)
        {
            if (Initialized)
            {
                view.Open(v1, v2);
            }
        }

        public void Hide(string panelName)
        {
            if (Initialized)
            {
                view.Hide(panelName);
            }
        }

        public void Hide(int index)
        {
            if (Initialized)
            {
                view.Hide(index);
            }
        }

        public void Close(string panelName)
        {
            if (Initialized)
            {
                view.Close(panelName);
            }
        }

        public bool IsOpen(int index)
        {
            if (Initialized)
            {
                return view.IsOpen(index);
            }
            return false;
        }
        public bool IsOpen(string panelName)
        {
            if (Initialized)
            {
                return view.IsOpen(panelName);
            }
            return false;
        }
    }
}