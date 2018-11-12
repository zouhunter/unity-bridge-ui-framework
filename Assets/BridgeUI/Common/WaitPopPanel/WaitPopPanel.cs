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

        protected override void PropBindings()
        {
            base.PropBindings();
            Binder.RegistValueChange<bool>(x=>m_close.interactable = x, "cansaleAble");
            Binder.RegistValueChange<string>(x=>m_title.text = x, "title");
        }
    }
}