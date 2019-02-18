using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeUI
{
    public interface IUIControl
    {
        bool Initialized { get; }
        void Initialize(object context = null);//初始化
        void Recover();//回收内存
    }
}
