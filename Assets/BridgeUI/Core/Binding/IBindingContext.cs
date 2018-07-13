using System.Collections.Generic;
using UnityEngine;
namespace BridgeUI.Binding
{
    public interface IBindingContext
    {
        Binding.ViewModel ViewModel { get; }
        void OnViewModelChanged(Binding.ViewModel newValue);
    }

}