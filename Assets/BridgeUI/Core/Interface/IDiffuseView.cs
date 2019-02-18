using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeUI
{
    public interface IDiffuseView : IChildPanelOpenClose
    {
        void Close();
        void CallBack(object arg0);
    }

}
