///*************************************************************************************
///* 作    者：       zouhunter
///* 创建时间：       2018-10-24 03:49:59
///* 说    明：       1.部分代码自动生成///                       2.尽量使用MVVM模式///                       3.宏定义内会读成注释
///* ************************************************************************************/
///<summary>
///[代码说明信息]
///<summary>
public abstract class VMUsePanel_ViewModel : BridgeUI.Binding.ViewModel
{
	#region 属性列表

	public BridgeUI.Binding.PanelAction<UnityEngine.UI.Button> onClick {
		get {
			return GetValue<BridgeUI.Binding.PanelAction<UnityEngine.UI.Button>> ("onClick");
		}
		set {
			SetValue<BridgeUI.Binding.PanelAction<UnityEngine.UI.Button>> ("onClick", value);
		}
	}

    public System.String title {
		get {
			return GetValue<System.String> ("title");
		}
		set {
			SetValue<System.String> ("title", value);
		}
	}


	public System.Int32 fontSize {
		get {
			return GetValue<System.Int32> ("fontSize");
		}
		set {
			SetValue<System.Int32> ("fontSize", value);
		}
	}


	public System.String info {
		get {
			return GetValue<System.String> ("info");
		}
		set {
			SetValue<System.String> ("info", value);
		}
	}
#endregion 属性列表
}
