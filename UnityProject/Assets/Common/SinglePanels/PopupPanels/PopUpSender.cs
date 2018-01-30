using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using BridgeUI;
namespace BridgeUI.Common
{
    public class PopUpSender : MonoBehaviour
    {
        public PopUpType popType;
        private Hashtable node = new Hashtable();
        private bool isCallBack;
        public void SendPopInfo(bool donHide)
        {
            Debug.Log("SendPopInfo");
            node["popInfo"] = (int)popType;
            node["closeAble"] = !donHide;
            UIFacade.Instence.Open("PopupPanel", node);
            isCallBack = false;
        }
        public void SendFunctionPopInfo(UnityAction<bool> callBack)
        {
            Debug.Log("SendFunctionPopInfo");
            node["popInfo"] = (int)popType;
            UIFacade.Instence.Open("FunctionPopupPanel", new UnityAction<object>((x) => { if (callBack != null) callBack.Invoke((bool)x); }), node);
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
    }
}