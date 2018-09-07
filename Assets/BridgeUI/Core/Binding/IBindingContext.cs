using System.Collections.Generic;
using UnityEngine;
namespace BridgeUI.Binding
{
    public interface IBindingContext
    {
        IViewModel ViewModel { get; set; }
        void OnViewModelChanged(IViewModel newValue);
    }

}