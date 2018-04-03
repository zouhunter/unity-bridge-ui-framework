using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;

namespace BridgeUI.Binding
{
    public class PropertyBinder
    {
        private readonly List<BindHandler> _binders = new List<BindHandler>();
        private readonly List<UnBindHandler> _unbinders = new List<UnBindHandler>();

        public void Record<T>(string name, ValueChangedHandler1<T> valueChangedHandler)
        {
            _binders.Add(viewModel =>
            {
                var prop = viewModel.GetBindableProperty<T>(name);
                valueChangedHandler.Invoke(prop.Value);
                prop.OnValueChanged += valueChangedHandler;
            });

            _unbinders.Add(viewModel =>
            {
                var prop = viewModel.GetBindableProperty<T>(name);
                prop.OnValueChanged -= valueChangedHandler;
            });

        }
        public void Bind(ViewModelBase viewmodel)
        {
            if (viewmodel != null)
            {
                for (int i = 0; i < _binders.Count; i++)
                {
                    _binders[i](viewmodel);
                }
            }
        }
        public void Unbind(ViewModelBase viewmodel)
        {
            if (viewmodel != null)
            {
                for (int i = 0; i < _unbinders.Count; i++)
                {
                    _unbinders[i](viewmodel);
                }
            }
        }
    }
}
