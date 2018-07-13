#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 01:13:36
    * 说    明：       1.支持string,string[],popInfo等参数
                       2.支持继承
* ************************************************************************************/
#endregion
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;

namespace BridgeUI.Common
{
    /// <summary>
    /// 消息显示面板
    /// [popInfo][closeAble][title][info]
    /// </summary>
    public class PopupPanel : SingleCloseAblePanel
    {
        public Text title;
        public Text info;
        public string defultTitle = "温馨提示";
        public bool queue;
        public InputField.OnChangeEvent onGet;
        public List<PopDataObj> popDatas = new List<PopDataObj>();

        protected bool callBack;
        private bool donthide = false;
        private Queue<KeyValuePair<string, string>> valueQueue = new Queue<KeyValuePair<string, string>>();
        private bool isShowing;

        public override void Close()
        {
            if (valueQueue.Count > 0)
            {
                ShowAnItem();
            }
            else
            {
                isShowing = false;
                CallBack(callBack);
                if (!donthide){
                    base.Close();
                }
            }

        }

        protected override void HandleData(object data)
        {
            base.HandleData(data);
            if(data is Enum)
            {
                HandleEnum(data as Enum);
            }
            else if (data is string)
            {
                HandleString(data as string);
            }
            else if (data is string[])
            {
                HandleArray(data as string[]);
            }
            else if (data is IDictionary)
            {
                HandleIDictionary(data as IDictionary);
            }
        }

        #region 支持多种数据类型
        private void HandleEnum(Enum data)
        {
            var typeName = data.GetType().FullName;
            var dataSource = popDatas.Find(x => x.enumType == typeName);
            if (dataSource != null)
            {
                int typeInt = (int)System.Convert.ChangeType(data, typeof(int));
                var popData = dataSource.GetPopData(typeInt);
                donthide = popData.donthide;
                OnMessageRecevie(popData.title, popData.info);
            }
        }
        private void HandleString(string value)
        {
            OnMessageRecevie(defultTitle, value);
        }
        private void HandleArray(string[] array)
        {
            if (array.Length > 1)
            {
                OnMessageRecevie(array[0], array[1]);
            }
            else if (array.Length > 0)
            {
                OnMessageRecevie(defultTitle, array[0]);
            }
        }
        private void HandleIDictionary(IDictionary dic)
        {
            var titleText = dic["title"] != null ? (string)dic["title"] : defultTitle;
            var infoText = dic["info"] != null ? dic["info"] as string : "";
            queue = dic["queue"] != null ? (bool)dic["queue"] : queue;
            donthide = dic["donthide"] != null ? (bool)dic["donthide"] : false;
            OnMessageRecevie(titleText, infoText);
        }
        #endregion


        private void OnMessageRecevie(string arg0, string arg1)
        {
            if (string.IsNullOrEmpty(arg0) && string.IsNullOrEmpty(arg1))
            {
                Debug.Log("Empty:" + arg0 + ":" + arg1);
            }
            else
            {
                valueQueue.Enqueue(new KeyValuePair<string, string>(arg0, arg1));

                if (queue)
                {
                    if (!isShowing)
                    {
                        ShowAnItem();
                    }
                }
                else
                {
                    ShowAnItem();
                }
            }
        }

        private void ShowAnItem()
        {
            isShowing = true;
            var item = valueQueue.Dequeue();
            title.text = item.Key;
            info.text = item.Value;
            onGet.Invoke(item.Key + item.Value);
        }
    }
}