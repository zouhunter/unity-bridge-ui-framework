/*************************************************************************************   
    * 作    者：       DefaultCompany
    * 时    间：       2018-04-15 06:04:45
    * 说    明：       1.本脚本由电脑自动生成
                       2.尽量使用MVVM模式
* ************************************************************************************/using BridgeUI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

///<summary>
///[代码说明信息]
///<summary>
public class MainPanel : BridgeUI.PanelBase
{
	[SerializeField]
	private UnityEngine.UI.Button m_close;

	[SerializeField]
	private UnityEngine.UI.Button m_openPanel01;

	[SerializeField]
	private UnityEngine.UI.Button m_openPanel02;

	[SerializeField]
	private UnityEngine.UI.Button m_openPanel03;

	[SerializeField]
	private UnityEngine.UI.Text m_title;

	[SerializeField]
	private UnityEngine.UI.Text m_info;

	[SerializeField]
	private UnityEngine.UI.Toggle m_switch;

	[SerializeField]
	private UnityEngine.UI.Slider m_slider;

	private string m_keyword;

	protected override void Awake ()
	{
		base.Awake ();
		BindingContext = new MainPanelViewModel ();
	}

	protected override void InitComponent ()
	{
		base.InitComponent ();
		this.m_close.onClick.AddListener (Close);
	}

	protected override void Binding ()
	{
		base.Binding ();
		Binder.AddValue<string> ("m_keyword", "keyword");
		Binder.AddText (this.m_title, "title");
		Binder.AddText (this.m_info, "info");
		Binder.AddButton (this.m_openPanel01, "OpenPanel01");
		Binder.AddButton (this.m_openPanel02, "OpenPanel02");
		Binder.AddButton (this.m_openPanel03, "OpenPanel03");
		Binder.AddToggle (this.m_switch, "Switch");
		Binder.AddSlider (this.m_slider, "progress");
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.A)) {
			Binder ["m_switch.isOn"].Value = (this.m_switch.isOn == false);
		}
		if (Input.GetKeyDown (KeyCode.V)) {
			Debug.Log (("m_keyword:" + this.m_keyword));
		}
	}
}
