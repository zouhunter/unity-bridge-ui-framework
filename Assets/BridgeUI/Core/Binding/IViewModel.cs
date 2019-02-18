using System.Collections.Generic;
using UnityEngine;
namespace BridgeUI.Binding
{
    public interface IViewModel
    {
        bool ContainsKey(byte keyword);
        void OnAfterBinding(BridgeUI.IUIPanel panel);
        void OnBeforeUnBinding(BridgeUI.IUIPanel panel);
        bool HaveDefultProperty(byte keyword);
        BindableProperty<T> GetBindableProperty<T>(byte keyword);
        IBindableProperty GetBindableProperty(byte keyword, System.Type type);
        void SetBindableProperty(byte keyword, IBindableProperty value);
    }

}