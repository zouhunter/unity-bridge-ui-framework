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

namespace BridgeUI.Binding
{

    public class ViewModelBase
    {
        protected readonly Dictionary<string, IBindableProperty> bindingPropertyDic = new Dictionary<string, BridgeUI.Binding.IBindableProperty>();
        public IBindableProperty this[string name]
        {
            get
            {
                if (bindingPropertyDic.ContainsKey(name))
                {
                    return bindingPropertyDic[name];
                }
                else
                {
                    return null;
                }
            }
            private set
            {
                bindingPropertyDic[name] = value;
            }
        }

        public virtual BindableProperty<T> GetBindableProperty<T>(string name)
        {
            if (this[name] == null || !(this[name] is BindableProperty<T>))
            {
                this[name] = GetDefultChildProperty(name);
                if (this[name] == null)
                {
                    this[name] = new BindableProperty<T>();
                }
            }
            return this[name] as BindableProperty<T>;
        }
        private IBindableProperty GetDefultChildProperty(string name)
        {
            var field = this.GetType().GetField(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance);
            if (field != null && typeof(IBindableProperty).IsAssignableFrom(field.FieldType))
            {
                return field.GetValue(this) as IBindableProperty;
            }
            return null;
        }
    }

}
