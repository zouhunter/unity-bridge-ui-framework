using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using BridgeUI;
using System;
namespace BridgeUI.Common
{
    public class WaitPopPanel : SinglePanel
    {
        [SerializeField]
        private Button m_cansale;
        [SerializeField]
        private Text title;
        protected override void Awake()
        {
            base.Awake();
            m_cansale.gameObject.SetActive(false);//默认不可取消
            if (m_cansale != null) m_cansale.onClick.AddListener(() => { Destroy(gameObject); });
        }
        protected override void HandleData(object data)
        {
            if (data != null)
            {
                ChangeState(data);
            }
        }

        void ChangeState(object uidata)
        {
            var dic = uidata as System.Collections.Hashtable;
            var calsale = dic["cansaleAble"] == null ? false : dic["cansaleAble"] as bool?;
            if (m_cansale && (bool)dic["cansaleAble"])
            {
                m_cansale.gameObject.SetActive(calsale == null ? false : (bool)calsale);
            }

            if (title && dic["title"] != null)
            {
                title.text = (string)dic["title"];
            }
        }
    }
}