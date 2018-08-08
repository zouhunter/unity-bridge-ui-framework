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
using System.Reflection;


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

        public ViewModel()
        {
            AutoInitPropery(this.GetType());
        }

        protected void AutoInitPropery(Type type)
        {
            var propertys = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty|BindingFlags.DeclaredOnly);
            foreach (var item in propertys)
            {
                var att = item.GetCustomAttributes(true).Where(x => x is BridgeUI.Attributes.DefultValueAttribute).FirstOrDefault();
                if (att != null)
                {
                    var fullType = typeof(BindableProperty<>).MakeGenericType(item.PropertyType);
                    this[item.Name] = System.Activator.CreateInstance(fullType) as IBindableProperty;
                    //Debug.Log("注册:" + item.Name);
                }
            }

            var baseType = type.BaseType;
            if(baseType != typeof(ViewModel) && baseType != null)
            {
                AutoInitPropery(baseType);
            }
        }

        public bool ContainsKey(string key)
        {
            return innerDic.ContainsKey(key);
        }

        public virtual void SetBindableProperty(string keyward, IBindableProperty value)
        {
            if (this[keyward] == null)
            {
                this[keyward] = value;
            }
            else if (this[keyward] == value)
            {
                this[keyward].ValueBoxed = value.ValueBoxed;
            }
            else
            {
                this[keyward] = value;
            }
        }

        public virtual BindableProperty<T> GetBindableProperty<T>(string keyward)
        {
            if (ContainsKey(keyward) && this[keyward] is BindableProperty<T>)
            {
                return this[keyward] as BindableProperty<T>;
            }
            return null;
        }

        public virtual BindableProperty<T> GetBindablePropertySelfty<T>(string keyward)
        {
            if (!ContainsKey(keyward) || !(this[keyward] is BindableProperty<T>))
            {
                this[keyward] = new BindableProperty<T>();
            }
            return this[keyward] as BindableProperty<T>;
        }

        public virtual IBindableProperty GetBindablePropertySelfty(string keyward, System.Type type)
        {
            var fullType = typeof(BindableProperty<>).MakeGenericType(type);
            if (!ContainsKey(keyward) || this[keyward].GetType() != fullType)
            {
                this[keyward] = System.Activator.CreateInstance(fullType) as IBindableProperty;
            }
            return this[keyward] as IBindableProperty;
        }

        protected virtual T GetValue<T>(string keyward)
        {
            return GetBindablePropertySelfty<T>(keyward).Value;
        }
        protected virtual void SetValue<T>(string keyward, T value)
        {
            GetBindablePropertySelfty<T>(keyward).Value = value;
        }
        public virtual void OnBinding(IBindingContext context) { this._contexts.Add(context); }
        public virtual void OnUnBinding(IBindingContext context) { this._contexts.Remove(context); }

    }

}
