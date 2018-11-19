///*************************************************************************************
///* 作    者：       zouhunter
///* 创建时间：       2018-11-19 08:46:28
///* 说    明：       1.部分代码自动生成///                       2.尽量使用MVVM模式///                       3.宏定义内会读成注释
///* ************************************************************************************/
///<summary>
///[代码说明信息]
///<summary>
public abstract class ListPanel_ViewModel : BridgeUI.Binding.ViewModelObject
{
	#region 属性列表
	protected System.String[] options {
		get {
			return GetValue<System.String[]> (keyword_options);
		}
		set {
			SetValue<System.String[]> (keyword_options, value);
		}
	}

	protected BridgeUI.Binding.PanelAction<System.Int32> on_selectid {
		get {
			return GetValue<BridgeUI.Binding.PanelAction<System.Int32>> (keyword_on_selectid);
		}
		set {
			SetValue<BridgeUI.Binding.PanelAction<System.Int32>> (keyword_on_selectid, value);
		}
	}

	#endregion 属性列表
	protected const string keyword_options = "options";

	protected const string keyword_on_selectid = "on_selectid";
}
