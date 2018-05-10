using System.Collections.Generic;
using UnityEngine;
namespace BridgeUI.Binding
{
    public interface IBindingContext
    {
        Binding.ViewModelBase ViewModel { get; }
        void OnViewModelChanged(Binding.ViewModelBase newValue);
    }

}