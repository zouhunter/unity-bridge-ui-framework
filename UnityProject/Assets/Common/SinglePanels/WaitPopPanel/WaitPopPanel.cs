#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 01:13:36
    * 说    明：       1.选择性传入标题和可关闭性
* ************************************************************************************/
#endregion
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using BridgeUI;
using System;
namespace BridgeUI.Common
{
    /// <summary>
    /// 等待面板
    /// </summary>
    public class WaitPopPanel : SingleCloseAblePanel
    {
        [SerializeField]
        private Text m_title;

        [Charge]
        private bool cansaleAble { set { m_close.interactable = value; } }

        [Charge]
        private string title;

        protected override void Awake()
        {
            base.Awake();
            m_close.interactable = false;
        }
    }
}