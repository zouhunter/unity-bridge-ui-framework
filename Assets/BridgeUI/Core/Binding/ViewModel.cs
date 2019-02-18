using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

namespace BridgeUI.Binding
{
    public class ViewModel : IViewModel
    {
#if BridgeUI_Log
        public static bool log { get; set; }
#endif
        private List<IUIPanel> _contexts = new List<IUIPanel>();
        public List<IUIPanel> Contexts { get { return _contexts; } }
        private readonly Dictionary<byte, IBindableProperty> innerDic = new Dictionary<byte, IBindableProperty>();
        protected IBindableProperty this[byte key]
        {
            get
            {
                if (innerDic.ContainsKey(key))
                {
                    return innerDic[key];
                }
                return null;
            }
            set
            {
                innerDic[key] = value;
            }
        }
        public bool ContainsKey(byte key)
        {
            return innerDic.ContainsKey(key);
        }
        public virtual void SetBindableProperty(byte keyword, IBindableProperty value)
        {
            if (this[keyword] == null)
            {
                this[keyword] = value;
            }
            else if (this[keyword] == value)
            {
                this[keyword].ValueBoxed = value.ValueBoxed;
            }
            else
            {
                this[keyword] = value;
            }
        }
        public virtual BindableProperty<T> GetBindableProperty<T>(byte keyword)
        {
            if (!ContainsKey(keyword) || this[keyword] == null)
            {
                var prop = new BindableProperty<T>();
                this[keyword] = prop;
                return prop;
            }
            else if (this[keyword] is BindableProperty<T>)
            {
                return this[keyword] as BindableProperty<T>;
            }
            else 
            {
                throw new Exception("类型不一致,请检查！" + this[keyword].GetType());
            }
        }
        public virtual IBindableProperty GetBindableProperty(byte keyword, System.Type type)
        {
            var fullType = typeof(BindableProperty<>).MakeGenericType(type);

            if (!ContainsKey(keyword) || this[keyword] == null)
            {
                this[keyword] = System.Activator.CreateInstance(fullType) as IBindableProperty;
            }
            if (this[keyword].GetType() == fullType)
            {
                return this[keyword] as IBindableProperty;
            }
            else
            {
                throw new Exception("类型不一致,请检查！" + this[keyword].GetType());
            }
        }

        protected virtual T GetValue<T>(byte keyword)
        {
            return GetBindableProperty<T>(keyword).Value;
        }
        protected virtual void SetValue<T>(byte keyword, T value)
        {
            GetBindableProperty<T>(keyword).Value = value;
        }
        public virtual void OnAfterBinding(BridgeUI.IUIPanel panel) {
            this._contexts.Add(panel);
        }
        public virtual void OnBeforeUnBinding(BridgeUI.IUIPanel panel) {
            this._contexts.Remove(panel);
        }
        public virtual void Monitor<T>(byte sourceName, UnityAction<T> onValueChanged)
        {
            if (onValueChanged != null)
            {
                GetBindableProperty<T>(sourceName).RegistValueChanged(onValueChanged);
            }
        }
        public bool HaveDefultProperty(byte keyword)
        {
            return innerDic.ContainsKey(keyword);
        }
    }
}
