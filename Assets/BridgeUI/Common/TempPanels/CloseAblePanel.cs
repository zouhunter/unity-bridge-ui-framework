#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 11:27:06
    * 说    明：       
* ************************************************************************************/
#endregion
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace BridgeUI
{
    public class CloseAblePanel : ViewBaseComponent
    {
        [SerializeField]
        protected Button m_close;

        protected override void OnInitialize()
        {
            if (m_close != null)
                m_close.onClick.AddListener(Close);
        }

        protected override void OnRecover()
        {
            if (m_close != null)
                m_close.onClick.RemoveListener(Close);
        }
    }
}