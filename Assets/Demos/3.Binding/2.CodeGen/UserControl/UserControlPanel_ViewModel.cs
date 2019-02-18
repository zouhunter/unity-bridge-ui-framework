///*************************************************************************************
///* 作    者：       zouhunter
///* 创建时间：       2018-11-19 12:54:01
///* 说    明：       1.部分代码自动生成///                       2.尽量使用MVVM模式///                       3.宏定义内会读成注释
///* ************************************************************************************/
///<summary>
///[代码说明信息]
///<summary>
public abstract class UserControlPanel_ViewModel : BridgeUI.Binding.ViewModel
{
	#region 属性列表
	protected System.String[] infos {
		get {
			return GetValue<System.String[]> (keyword_infos);
		}
		set {
			SetValue<System.String[]> (keyword_infos, value);
		}
	}

	protected BridgeUI.Binding.PanelAction<ButtonList> onSelect {
		get {
			return GetValue<BridgeUI.Binding.PanelAction<ButtonList>> (keyword_onSelect);
		}
		set {
			SetValue<BridgeUI.Binding.PanelAction<ButtonList>> (keyword_onSelect, value);
		}
	}

	#endregion 属性列表
	protected const byte keyword_infos = 1;

	protected const byte keyword_onSelect = 2;
}
