using UnityEngine;
using System;


namespace BridgeUI.Binding
{
    public class ViewModelContainer : ScriptableObject, IViewModel
    {
        [SerializeField]
        public ViewModelObject instence;

        public bool ContainsKey(string key)
        {
            return instence.ContainsKey(key);
        }

        public BindableProperty<T> GetBindableProperty<T>(string keyward)
        {
            return instence.GetBindableProperty<T>(keyward);
        }

        public IBindableProperty GetBindableProperty(string keyward, Type type)
        {
            return instence.GetBindableProperty(keyward, type);
        }

        public void OnBinding(IBindingContext context)
        {
            instence.OnBinding(context);
        }

        public void OnUnBinding(IBindingContext context)
        {
            instence.OnUnBinding(context);
        }

        public void SetBindableProperty(string keyward, IBindableProperty value)
        {
            instence.SetBindableProperty(keyward,value);
        }
    }
}