/*************************************************************************************   
    * 作    者：       DefaultCompany
    * 时    间：       2018-04-15 04:09:46
    * 说    明：       1.本脚本由电脑自动生成
* ************************************************************************************/
using BridgeUI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

class MainPanel : BridgeUI.PanelBase
{
	[SerializeField]
	UnityEngine.UI.Button m_close;

	[SerializeField]
	UnityEngine.UI.Button m_openPanel01;

	[SerializeField]
	UnityEngine.UI.Button m_openPanel02;

	[SerializeField]
	UnityEngine.UI.Button m_openPanel03;

	[SerializeField]
	UnityEngine.UI.Text m_title;

	string a;

	void Awake ()
	{
		base.Awake ();
	}
}
