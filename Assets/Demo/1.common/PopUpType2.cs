using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
namespace BridgeUI.Common
{
    public enum PopUpType2 
    {
        OnNetError = 1,//网络出错
        OnNetExit = 2,//网络断开
        FileNotExit = 3,//文件?
        NaviComplete = 4,//引导结束
    }
}