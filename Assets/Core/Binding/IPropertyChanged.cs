using System.Collections.Generic;
using UnityEngine;
namespace BridgeUI.Binding
{
    public delegate void PropertyChangedHand(string propertyName);

    public interface BindingContext
    {
        event PropertyChangedHand onPropertyChanged;
    }

}