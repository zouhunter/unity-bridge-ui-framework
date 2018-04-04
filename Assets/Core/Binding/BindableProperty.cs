using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace BridgeUI.Binding
{
    public interface IBindableProperty: IPropertyChanged
    {
        object Value { get; set; }
    }

    public class BindableProperty<T> : IBindableProperty
    {
        public event ValueChangedHandler1<T> OnValueChanged;
        public event ValueChangedHandler2<T> OnValueChangedFrom;
        public event PropertyChanged OnPropertyChanged;

        private T _value;
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
                    ValueChanged(old,_value);
                }
            }
        }

        object IBindableProperty.Value
        {
            get { return Value; }
            set { Value = (T)value; }
        }

        private void ValueChanged(T oldValue,T newValue)
        {
            if (OnValueChanged != null)
            {
                OnValueChanged(newValue);
            }
            if(OnValueChangedFrom != null)
            {
                OnValueChangedFrom(oldValue, newValue);
            }
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
