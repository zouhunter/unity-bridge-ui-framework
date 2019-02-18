using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;
using BridgeUI;
using System;

namespace BridgeUI.Common
{
    public class PopupSender : MonoBehaviour
    {
        public string enumType;
#if UNITY_EDITOR
        public UnityEditor.MonoScript popEnum;
#endif
        public string selected;
        private bool isCallBack;

       
        public void SendPopInfo()
        {
            Debug.Log("Send!" + GetCurrentEnumValue().GetType());
 
            UIFacade.Instence.Open("PopupPanel", GetCurrentEnumValue());
            isCallBack = false;
        }
        public void SendFunctionPopInfo(UnityAction<bool> callBack)
        {
            UIFacade.Instence.Open("FunctionPopupPanel", GetCurrentEnumValue());
            //.RegistCallBack(new UnityAction<IUIPanel,object>((z,x) => { if (callBack != null) callBack.Invoke((bool)x); }));
            isCallBack = true;
        }
        public void ClosePopUpPanel()
        {
            if (isCallBack)
            {
                UIFacade.Instence.Close("FunctionPopupPanel");
            }
            else
            {
                UIFacade.Instence.Close("PopupPanel");
            }
        }


        private object GetCurrentEnumValue()
        {
            return System.Enum.Parse(System.Type.GetType(enumType), selected);
        }
    }
}