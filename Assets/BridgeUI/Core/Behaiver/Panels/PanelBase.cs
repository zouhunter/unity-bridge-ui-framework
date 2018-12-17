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
    [BridgeUI.Attributes.PanelParent]
    public class PanelBase : PanelCore, IBindingContext
    {
        protected Binding.PropertyBinder _binder;
        public virtual Binding.PropertyBinder Binder
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
        [SerializeField, Attributes.DefultViewModel(true)]
        private ScriptableObject defultViewModel;
        private IViewModel _viewModel;
        public IViewModel ViewModel
        {
            get
            {
                if(_viewModel == null)
                {
                    if (defultViewModel != null)
                    {
                        _viewModel = defultViewModel as IViewModel;
                    }
                    if (_viewModel == null)
                    {
                        _viewModel = new ViewModel();
                    }
                }
                return _viewModel;
            }
            set
            {
                _viewModel = value;
                OnViewModelChanged(value);
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
            Binder.Bind(ViewModel);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Binder.Unbind();
        }

        protected virtual void InitComponents() { }

        protected virtual void PropBindings() { }

        public virtual void OnViewModelChanged(IViewModel newValue)
        {
            Binder.Unbind();
            Binder.Bind(newValue);
        }

        protected override void HandleData(object data)
        {
            base.HandleData(data);

            if (data is IViewModel)
            {
                ViewModel = data as IViewModel;
            }
            else if (data is IDictionary)
            {
                LoadPropDictionary(ViewModel, data as IDictionary);
            }
        }

        protected virtual void LoadPropDictionary(IViewModel viewModel, IDictionary dataDic)
        {
            var keys = dataDic.Keys;
            foreach (var key in keys)
            {
                var value = dataDic[key];
                if (value != null)
                {
                    var prop = viewModel.GetBindableProperty(key.ToString(), value.GetType());
                    if (prop != null)
                    {
                        prop.ValueBoxed = value;
                    }
                }
            }
        }
    }
}