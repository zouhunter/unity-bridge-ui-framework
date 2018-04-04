using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace BridgeUI.Binding
{
    public class PropertyBinder: IPropertyChanged
    {
        public object Context { get; private set; }
        public object Target { get; private set; }
        private readonly List<BindHandler> _binders = new List<BindHandler>();
        private readonly List<UnBindHandler> _unbinders = new List<UnBindHandler>();
        public event PropertyChanged OnPropertyChanged = delegate { };

        public PropertyBinder(PanelBase context)
        {
            this.Context = context;
        }

        public void Bind(ViewModelBase viewmodel)
        {
            this.Target = viewmodel;
            if (viewmodel != null)
            {
                for (int i = 0; i < _binders.Count; i++){
                    _binders[i](viewmodel);
                }
                ((IPropertyChanged)Target).OnPropertyChanged += RaiseEvent;
            }
        }
        public void Unbind(ViewModelBase viewmodel)
        {
            this.Target = null;
            if (viewmodel != null)
            {
                for (int i = 0; i < _unbinders.Count; i++){
                    _unbinders[i](viewmodel);
                }
                ((IPropertyChanged)Target).OnPropertyChanged -= RaiseEvent;
            }
        }

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

        public void Record(UIBehaviour behaviour,string name)
        {

        }

        public void BindingText(Text m_title, string name)
        {
            Record<string>(name, (value) => { m_title.text = value; });
        }
        public void BindingButton(Button button, string methodName)
        {
            UnityAction action = ()=> { Invoke(methodName, button, new RoutedEventArgs(Context)); };
           
            _binders.Add(viewModel =>
            {
                button.onClick.AddListener(action);
            });

            _unbinders.Add(viewModel =>
            {
                button.onClick.RemoveListener(action);
            });
        }
        public T Get<T>(string memberName)
        {
            //TODO sanity
            //TODO Cache Reflection
            //TODO Conversion
            var temps = Target.GetType().GetMember(memberName);
            var temp = temps[0];
            if (temp is FieldInfo)
            {
                return (T)(temp as FieldInfo).GetValue(Target);
            }
            else if (temp is PropertyInfo)
            {
                return (T)(temp as PropertyInfo).GetValue(Target, null);
            }
            else
            {
                return (T)(temp as MethodInfo).Invoke(Target, null);
            }
        }
        public void Set<T>(string memberName, T value)
        {
            var temps = Target.GetType().GetMember(memberName);
            var temp = temps[0];
            if (temp is FieldInfo)
            {
                (temp as FieldInfo).SetValue(Target, value);
            }
            else if (temp is PropertyInfo)
            {
                (temp as PropertyInfo).SetValue(Target, value, null);
            }
            else
            {
                (temp as MethodInfo).Invoke(Target, new object[] { value });
            }
        }
        public void Invoke(string memberName, params object[] value)
        {
            var temps = Target.GetType().GetMember(memberName);
            var temp = temps[0];
            if (temp is MethodInfo)
            {
                (temp as MethodInfo).Invoke(Target, value);
            }
        }
        private void RaiseEvent(string memberName)
        {
            OnPropertyChanged(memberName);
        }

    }
}
