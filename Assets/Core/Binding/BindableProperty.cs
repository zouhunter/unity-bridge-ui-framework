using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine.Events;

namespace BridgeUI.Binding
{
    public interface IBindableProperty
    {
        object Value { get; set; }
        void Notify();
    }

    public class BindableProperty<T> : IBindableProperty
    {
        public event UnityAction<T> onValueChanged = delegate { };

        private T _value = default(T);
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!Equals(_value, value))
                {
                    T old = _value;
                    _value = value;
                    ValueChanged(_value);
                }
            }
        }

        object IBindableProperty.Value
        {
            get { return Value; }
            set { Value = (T)value; }
        }
        private void ValueChanged(T value)
        {
            if (onValueChanged != null)
                onValueChanged.Invoke(value);
        }
        public void RegistValueChanged(UnityAction<T> OnValueChanged)
        {
            this.onValueChanged += OnValueChanged;
        }
        public void RemoveValueChanged(UnityAction<T> OnValueChanged)
        {
            this.onValueChanged -= OnValueChanged;
        }
        public void Notify()
        {
            ValueChanged(Value);
        }
        public override string ToString()
        {
            return (Value != null ? Value.ToString() : "null");
        }

        public void Clear()
        {
            _value = default(T);
        }

      
    }

}
