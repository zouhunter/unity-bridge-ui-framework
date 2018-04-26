/*************************************************************************************   
    * 作    者：       zouhunter
    * 创建时间：       2018-04-23 09:17:19
    * 说    明：       1.部分代码自动生成
                       2.尽量使用MVVM模式
* ************************************************************************************/using BridgeUI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

///<summary>
///[代码说明信息]
///<summary>
public class MyPanel : BridgeUI.SingleCloseAblePanel
{
	[SerializeField]
	private UnityEngine.UI.Image m_head;

	[SerializeField]
	private UnityEngine.UI.Text m_title;

	[SerializeField]
	private UnityEngine.UI.Text m_info;

	[SerializeField]
	private UnityEngine.UI.Slider m_slider;

	protected override void InitComponents ()
	{
		m_slider.onValueChanged.AddListener (OnSlider);
	}

	protected override void PropBindings ()
	{
		Binder.RegistMember<Sprite> ("m_head.sprite", "head");
		Binder.RegistMember<Text>("m_title.text", "title");
		Binder.RegistMember<Text>("m_info.text", "info");
	}

	protected void OnSlider (float value)
	{
	}
}
