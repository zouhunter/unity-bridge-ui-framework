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
        [System.Serializable]
        public class PopData
        {
            public int typeInt;
            public string typeName;
            public string title;
            [Multiline(2)]
            public string info;

            public PopData(int typeInt, string typeName)
            {
                this.typeInt = typeInt;
                this.typeName = typeName;
            }
        }

        public Text title;
        public Text info;
        private bool donthide = false;
        public InputField.OnChangeEvent onGet;
        private Queue<KeyValuePair<string, string>> valueQueue = new Queue<KeyValuePair<string, string>>();
        private bool isShowing;
        protected bool callBack;

#if UNITY_EDITOR
        [HideInInspector]
        public UnityEditor.MonoScript popEnum;
#endif
        [HideInInspector]
        public List<PopData> popDatas = new List<PopData>();

        public PopData GetPopData(int typeInt)
        {
            var item = popDatas.Find(x => x.typeInt == typeInt);
            return item;
        }

        public override void Close()
        {
            if (valueQueue.Count > 0)
            {
                ShowAnItem();
            }
            else
            {
                isShowing = false;
                if (!donthide)
                {
                    CallBack(callBack);
                    base.Close();
                }
            }

        }

        void OnMessageRecevie(string arg0, string arg1)
        {
            if (string.IsNullOrEmpty(arg0) && string.IsNullOrEmpty(arg1))
            {
                Debug.Log("Empty:" + arg0 + ":" + arg1);
            }
            else
            {
                valueQueue.Enqueue(new KeyValuePair<string, string>(arg0, arg1));

                if (!isShowing)
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
        protected override void HandleData(object obj)
        {
            base.HandleData(obj);
            if (obj is string)
            {
                OnMessageRecevie("温馨提示", (string)obj);
            }
            else if (obj is string[])
            {
                var strs = (string[])obj;
                if (strs.Length > 1)
                {
                    var title = strs[0];
                    var info = strs[1];
                    OnMessageRecevie(title, info);
                }
                else if (strs.Length > 0)
                {
                    var title = "温馨提示";
                    var info = strs[0];
                    OnMessageRecevie(title, info);
                }
            }
            else if (obj is Hashtable)
            {
                var dic = obj as Hashtable;
                if (dic["popInfo"] != null)
                {
                    int typeIndex = (int)dic["popInfo"];
                    donthide = dic["closeAble"] != null ? (bool)dic["closeAble"] : false;
                    var data = GetPopData(typeIndex);
                    if (data != null)
                    {
                        OnMessageRecevie(data.title, data.info);
                    }
                    else
                    {
                        Debug.Log("Empty::" + typeIndex);
                    }
                }
                else
                {
                    var title = dic["title"] != null ? (string)dic["title"] : "温馨提示";
                    var info = dic["info"] != null ? dic["info"] as string : "";
                    donthide = dic["closeAble"] != null ? (bool)dic["closeAble"] : false;
                    OnMessageRecevie(title, info);
                }
            }
        }
    }
}