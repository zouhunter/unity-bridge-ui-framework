using System.Collections.Generic;
using UnityEngine;
namespace BridgeUI.Binding
{
    public delegate void PropertyChangedHand(string propertyName);

    public interface IBindingContext
    {
        event PropertyChangedHand onPropertyChanged;
    }

}