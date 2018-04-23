/*************************************************************************************   
    * 作    者：       zouhunter
    * 创建时间：       2018-04-23 01:42:32
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
public class LuaPanel01 : BridgeUI.LuaPanel
{
	[SerializeField]
	private UnityEngine.UI.Button m_Button;

	protected override void InitComponents ()
	{
	}

	protected override void PropBindings ()
	{
		Binder.RegistButtonEvent (m_Button, "on_button_clicked");
	}

    protected override void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown(0))
        {
            HandleData("你好");
        }
    }
}
