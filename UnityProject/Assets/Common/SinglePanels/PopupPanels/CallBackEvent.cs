using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
namespace BridgeUI.Common
{
    [System.Serializable]
    public class CallBackEvent : UnityEvent<UnityAction<bool>>
    {

    }
}