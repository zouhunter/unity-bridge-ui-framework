using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
namespace BridgeUI.Common
{
    public enum PopUpType : int
    {
        OnNetError = 1,//网络出错
        OnNetExit = 2,//网络断开
        FileNotExit = 3,//文件?
        NaviComplete = 4,//引导结束
        NoExamination = 5,//没有试卷
        NoAnswerSheetInfo = 6,//没有答卷
        PushAnswerConfer = 7,//提交确认
        LoginTwince = 8,//重复登录
        SaveOrNot = 9,//保存或取消
        NetSelect = 10//未选择
    }
}