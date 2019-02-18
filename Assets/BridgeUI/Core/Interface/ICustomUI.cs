using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeUI
{
    public interface ICustomUI
    {
        Transform Content { get;  }
        void Initialize(IUIPanel view);
        void Recover();
    }
}