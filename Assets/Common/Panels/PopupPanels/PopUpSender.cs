using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;
using BridgeUI;
namespace BridgeUI.Common
{
    public class PopUpSender : MonoBehaviour
    {
        public string enumType;
#if UNITY_EDITOR
        public UnityEditor.MonoScript popEnum;
#endif
        public string selected;
        private Hashtable node = new Hashtable();
        private bool isCallBack;
        public void SendPopInfo()
        {
            Debug.Log("Send!" + GetCurrentEnumValue().GetType());
            UIFacade.Instence.Open("PopUpPanel", GetCurrentEnumValue());
            isCallBack = false;
        }
        public void SendFunctionPopInfo(UnityAction<bool> callBack)
        {
            UIFacade.Instence.Open("FunctionPopupPanel", new UnityAction<object>((x) => { if (callBack != null) callBack.Invoke((bool)x); }), GetCurrentEnumValue());
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