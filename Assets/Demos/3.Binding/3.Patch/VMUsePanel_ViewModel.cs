///*************************************************************************************
///* 作    者：       zouhunter
///* 创建时间：       2018-12-17 05:28:02
///* 说    明：       1.部分代码自动生成///                       2.尽量使用MVVM模式///                       3.宏定义内会读成注释
///* ************************************************************************************/
///<summary>
///[代码说明信息]
///<summary>
public abstract class VMUsePanel_ViewModel : BridgeUI.Binding.ViewModel
{
	#region 属性列表
	protected BridgeUI.Binding.PanelAction<UnityEngine.UI.Button> onClick {
		get {
			return GetValue<BridgeUI.Binding.PanelAction<UnityEngine.UI.Button>> (keyword_onClick);
		}
		set {
			SetValue<BridgeUI.Binding.PanelAction<UnityEngine.UI.Button>> (keyword_onClick, value);
		}
	}

	protected System.String title {
		get {
			return GetValue<System.String> (keyword_title);
		}
		set {
			SetValue<System.String> (keyword_title, value);
		}
	}

	protected System.Int32 fontSize {
		get {
			return GetValue<System.Int32> (keyword_fontSize);
		}
		set {
			SetValue<System.Int32> (keyword_fontSize, value);
		}
	}

	protected System.String info {
		get {
			return GetValue<System.String> (keyword_info);
		}
		set {
			SetValue<System.String> (keyword_info, value);
		}
	}

	#endregion 属性列表
	protected const byte keyword_onClick = 1;

	protected const byte keyword_title = 2;

	protected const byte keyword_fontSize = 3;

	protected const byte keyword_info = 4;
}
