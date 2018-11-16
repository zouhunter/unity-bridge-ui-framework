using System.Collections.Generic;
using UnityEngine;
namespace BridgeUI.Binding
{
    public interface IViewModel
    {
        bool ContainsKey(string key);
        void OnBinding(IBindingContext context);
        void OnUnBinding(IBindingContext context);
        BindableProperty<T> GetBindableProperty<T>(string keyward);
        IBindableProperty GetBindableProperty(string keyward, System.Type type);
        void SetBindableProperty(string keyward, IBindableProperty value);
    }

}