#region statement
/*************************************************************************************   
    * 作    者：       请填写作者
    * 时    间：       2018-03-23 06:20:06
    * 说    明：       
* ************************************************************************************/
#endregion

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;


public partial class MyPanel : PanelBase {
    
    [SerializeField()]
    private UnityEngine.UI.Button m_close;
    
    private UnityEngine.GameObject m_btn;
    
    [SerializeField()]
    private UnityEngine.UI.Image m_head;
    
    [SerializeField()]
    private UnityEngine.UI.Text m_title;
    
    [SerializeField()]
    private UnityEngine.UI.Text m_info;
}
