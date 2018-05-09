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

namespace BridgeUI.Binding
{
    public class ViewModelBase
    {
        private List<IBindingContext> _contexts = new List<IBindingContext>();
        protected List<IBindingContext> Contexts { get { return _contexts; } }
        protected readonly Dictionary<string, IBindTarget> bindingPropertyDic = new Dictionary<string, BridgeUI.Binding.IBindTarget>();
        public IBindTarget this[string name]
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
            protected set
            {
                bindingPropertyDic[name] = value;
            }
        }

        public ViewModelBase()
        {
            RegistPropertys();
        }
        private void RegistPropertys()
        {
            var fields = this.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field != null && typeof(IBindTarget).IsAssignableFrom(field.FieldType))
                {
                    var customAttributes = field.GetCustomAttributes(false);
                    var bindingProp = customAttributes.Where(x => x.GetType() == typeof(BindingAttribute)).FirstOrDefault();
                    string bindingKey = field.Name;
                    if (bindingProp != null)
                    {
                        var customKey = (bindingProp as BindingAttribute).keyword;
                        if (!string.IsNullOrEmpty(customKey))
                        {
                            bindingKey = customKey;
                        }
                    }
                    var value = field.GetValue(this) as IBindTarget;
                    if (value == null)
                    {
                        value = System.Activator.CreateInstance(field.FieldType) as IBindTarget;
                        field.SetValue(this, value);
                    }

                    bindingPropertyDic[bindingKey] = value;
                }
            }
        }

        public virtual T GetUsefulBindTarget<T>(string name) where T : IBindTarget, new()
        {
            if (this[name] == null || !(this[name] is T))
            {
                this[name] = new T();
            }
            return (T)this[name];
        }

        public virtual IBindTarget GetUsefulBindTarget(string name, System.Type type)
        {
            if (this[name] == null)
            {
                this[name] = System.Activator.CreateInstance(type) as IBindTarget;
            }
            return this[name] as IBindTarget;
        }

        public virtual void OnBinding(IBindingContext context) { this._contexts.Add(context); }
        public virtual void OnUnBinding(IBindingContext context) { this._contexts.Remove(context); }
    }

}
