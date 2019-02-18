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

namespace BridgeUI.Binding
{


    [BridgeUI.Attributes.PanelParent]
    public abstract class BindingViewBase : ViewBase
    {
        protected Binding.UIPanelBinder _binder;
        public virtual Binding.UIPanelBinder Binder
        {
            get
            {
                if (_binder == null)
                {
                    _binder = new Binding.UIPanelBinder(this);
                }
                return _binder;
            }
        }
        public bool Initialized { get; private set; }

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

        protected override void HandleData(object data)
        {
            base.HandleData(data);

            if (VM != null && VM is IDataReceiver)
            {
                (VM as IDataReceiver).HandleData(data);
            }
        }
    }
}