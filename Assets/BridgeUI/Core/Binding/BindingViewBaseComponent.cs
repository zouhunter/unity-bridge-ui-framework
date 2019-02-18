using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeUI.Binding
{
    public abstract class BindingViewBaseComponent : ViewBaseComponent
    {
        protected Binding.UIPanelBinder _binder;
        public virtual Binding.UIPanelBinder Binder
        {
            get
            {
                if (_binder == null)
                {
                    _binder = new Binding.UIPanelBinder(view);
                }
                return _binder;
            }
        }
        private IViewModel _vm;
        public IViewModel VM
        {
            get
            {
                return _vm;
            }
            set
            {
                _vm = value;
                OnViewModelChanged(value);
            }
        }

        public virtual void OnViewModelChanged(IViewModel newValue)
        {
            Binder.Unbind();
            Binder.Bind(newValue);
        }

        protected override void OnMessageReceive(object message)
        {
            base.OnMessageReceive(message);
   
            base.HandleData(message);

            if (VM != null && VM is IDataReceiver)
            {
                (VM as IDataReceiver).HandleData(message);
            }
        }
        protected override void OnRecover()
        {
            Binder.Unbind();
        }
    }
}