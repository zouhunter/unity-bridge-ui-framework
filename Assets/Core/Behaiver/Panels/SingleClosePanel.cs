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
namespace BridgeUI
{
    public class SingleCloseAblePanel : SinglePanel
    {
        [SerializeField]
        protected Button m_close;
        protected override void Awake()
        {
            base.Awake();
            if (m_close != null) m_close.onClick.AddListener(Close);
        }
    }
}