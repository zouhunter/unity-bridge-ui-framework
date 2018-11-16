using System.Collections.Generic;
using UnityEngine;
namespace BridgeUI.Binding
{
    public interface IBindingContext
    {
        Binding.PropertyBinder Binder { get; }
        IViewModel ViewModel { get; set; }
    }

}