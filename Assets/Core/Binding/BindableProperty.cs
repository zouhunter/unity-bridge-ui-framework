using System;
using UnityEngine.Events;

namespace BridgeUI.Binding
{
    public interface IBindTarget
    {
        object Target { get; set; }
    }
    public class B_Property<T> : IBindTarget
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

        public object Target
        {
            get { return Value; }
            set { Value = (T)value; }
        }
        public B_Property()
        {
        }
        public B_Property(T value)
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

        public void Clear()
        {
            _value = default(T);
        }
    }
    public class B_String : B_Property<string> { }
    public class B_Int : B_Property<int> { }
    public class B_Float : B_Property<float> { }
    public class B_Bool : B_Property<bool> { }
    public class B_Byte : B_Property<byte> { }
    public class B_Long : B_Property<long> { }
    public class B_Short : B_Property<short> { }
    public class B_StringArray : B_Property<string[]> { }

    public class B_Sprite : B_Property<UnityEngine.Sprite> { }
    public class B_Texture : B_Property<UnityEngine.Texture> { }

    public class B_Action : B_Property<System.Action> { }
    public class B_Action<T> : B_Property<System.Action<T>> { }
    public class B_UnityAction : B_Property<UnityEngine.Events.UnityAction> { }
    public class B_UnityAction<T> : B_Property<UnityEngine.Events.UnityAction<T>> { }
    public class B_Func<T> : B_Property<System.Func<T>> { }

    public class C_PanelAction : IBindTarget
    {
        protected PanelAction panelAction;
        public object Target { get { return panelAction; } set { panelAction = value as PanelAction; } }
        public C_PanelAction() { }
        public C_PanelAction(PanelAction data)
        {
            this.panelAction = data;
        }
        public void RegistAction(PanelAction data)
        {
            panelAction = data;
        }
    }
    public class C_PanelAction<T> : IBindTarget
    {
        protected PanelAction<T> panelAction;
        public object Target
        {
            get
            {
                return panelAction;
            }
            set
            {
                if(value is PanelAction<T>)
                {
                    this.panelAction = value as PanelAction<T>;
                }
                else
                {
                    UnityEngine.Debug.Log(value +"not equal" + typeof(PanelAction<T>));
                }
            }
        }
        public C_PanelAction() { }
        public C_PanelAction(PanelAction<T> data) {
            this.panelAction = data;
        }
        public void RegistAction(PanelAction<T> data)
        {
            this.panelAction = data;
        }
    }
    public class C_Button : C_PanelAction<UnityEngine.UI.Button> { }
    public class C_Toggle : C_PanelAction<UnityEngine.UI.Toggle> { }
    public class C_Slider : C_PanelAction<UnityEngine.UI.Slider> { }
    public class C_Scrollbar : C_PanelAction<UnityEngine.UI.Scrollbar> { }
    public class C_Dropdown : C_PanelAction<UnityEngine.UI.Dropdown> { }
    public class C_ScrollRect : C_PanelAction<UnityEngine.UI.ScrollRect> { }
    public class C_InputField : C_PanelAction<UnityEngine.UI.InputField> { }
}
