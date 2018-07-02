using System;
using UnityEngine.Events;

namespace BridgeUI.Binding
{
    public interface IBindableProperty
    {
        object ValueBoxed { get; set; }

        void Trigger();
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
                    _value = value;
                    ValueChanged(_value);
                }
            }
        }

        public object ValueBoxed
        {
            get { return Value; }
            set { Value = (T)value; }
        }
        public BindableProperty()
        {
        }
        public BindableProperty(T value)
        {
            Value = value;
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

        public override string ToString()
        {
            return (Value != null ? Value.ToString() : "null");
        }

        public void SetValueNoTrigger(T value)
        {
            _value = value;
        }

        public void Trigger()
        {
            ValueChanged(Value);
        }
        public void Clear()
        {
            _value = default(T);
        }

        public static implicit operator T(BindableProperty<T> target){
            return target.Value;
        }
    }
}
