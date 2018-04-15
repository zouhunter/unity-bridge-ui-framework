/*************************************************************************************   
    * 作    者：       DefaultCompany
    * 时    间：       2018-04-15 03:55:28
    * 说    明：       1.本脚本由电脑自动生成
                       2.请尽量不要在其中写代码
                       3.更无法使用协程及高版本特性
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

	void Awake ()
	{
		base.Awake ();
		StartCoroutine (TestCoroutine ());
	}

	IEnumerator TestCoroutine ()
	{
		yield return null;
	}
}
