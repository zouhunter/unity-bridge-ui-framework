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
using System.Collections.Generic;
using System.Linq;
using System;

namespace BridgeUI.Binding
{
    public class ViewModel : ScriptableObject
    {
        private List<IBindingContext> _contexts = new List<IBindingContext>();
        protected List<IBindingContext> Contexts { get { return _contexts; } }
        private readonly Dictionary<string, IBindableProperty> innerDic = new Dictionary<string, IBindableProperty>();
        protected IBindableProperty this[string key]
        {
            get
            {
                if(innerDic.ContainsKey(key))
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


        public bool ContainsKey(string key)
        {
            return innerDic.ContainsKey(key);
        }


        public virtual void SetBindableProperty(string key, IBindableProperty value)
        {
            if(this[name] == null)
            {
                this[name] = value;
            }
            else if(this[name] == value)
            {
                this[name].ValueBoxed = value.ValueBoxed;
            }
            else
            {
                this[name] = value;
            }
        }

        public virtual BindableProperty<T> GetBindableProperty<T>(string name)
        {
            if (ContainsKey(name) && this[name] is BindableProperty<T>)
            {
                return this[name] as BindableProperty<T>;
            }
            return null;
        }

        public virtual BindableProperty<T> GetBindablePropertySelfty<T>(string name)
        {
            if (!ContainsKey(name) || !(this[name] is BindableProperty<T>))
            {
                this[name] = new BindableProperty<T>();
            }
            return this[name] as BindableProperty<T>;
        }

        public virtual IBindableProperty GetBindablePropertySelfty(string name, System.Type type)
        {
            var fullType = typeof(BindableProperty<>).MakeGenericType(type);

            if (!ContainsKey(name) || this[name].GetType() != fullType)
            {
                this[name] = System.Activator.CreateInstance(fullType) as IBindableProperty;
            }
            return this[name] as IBindableProperty;
        }

        protected virtual T GetValue<T>(string key)
        {
            return GetBindablePropertySelfty<T>(key).Value;
        }
        protected virtual void SetValue<T>(string key,T value)
        {
            GetBindablePropertySelfty<T>(key).Value = value;
        }
        public virtual void OnBinding(IBindingContext context) { this._contexts.Add(context); }
        public virtual void OnUnBinding(IBindingContext context) { this._contexts.Remove(context); }
   
    }

}
