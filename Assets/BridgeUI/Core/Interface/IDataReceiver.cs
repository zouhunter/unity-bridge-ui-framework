using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeUI
{
    public interface IDataReceiver
    {
        void HandleData(object data);
    }
}
