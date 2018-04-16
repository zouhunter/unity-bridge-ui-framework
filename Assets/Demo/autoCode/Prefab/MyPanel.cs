/*************************************************************************************   
    * 作    者：       DefaultCompany
    * 时    间：       2018-03-24 04:12:39
    * 说    明：       1.本脚本由电脑自动生成
                       2.请尽量不要在其中写代码
                       3.更无法使用协程及高版本特性
* ************************************************************************************/


using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;
public sealed class MyPanel : BridgeUI.SingleCloseAblePanel {

    [UnityEngine.SerializeField()]
    private UnityEngine.UI.Image m_head;

    [UnityEngine.SerializeField()]
    private UnityEngine.UI.Text m_title;

    [UnityEngine.SerializeField()]
    private UnityEngine.UI.Text m_info;

    [UnityEngine.SerializeField()]
    private UnityEngine.UI.Slider m_slider;

    public BridgeUI.Binding.ViewModelBase model = new MyPanelModel();

    private void Set_Head_Image(Sprite sprite)
    {
        m_head.sprite = sprite;
    }
}
