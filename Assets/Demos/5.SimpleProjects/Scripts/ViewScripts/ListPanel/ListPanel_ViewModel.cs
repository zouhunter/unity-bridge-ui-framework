///*************************************************************************************
///* 作    者：       zouhunter
///* 创建时间：       2018-11-19 09:36:29
///* 说    明：       1.部分代码自动生成///                       2.尽量使用MVVM模式///                       3.宏定义内会读成注释
///* ************************************************************************************/
///<summary>
///[代码说明信息]
///<summary>
public abstract class ListPanel_ViewModel : BridgeUI.Binding.ViewModel
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

	protected System.String title {
		get {
			return GetValue<System.String> (keyword_title);
		}
		set {
			SetValue<System.String> (keyword_title, value);
		}
	}

	#endregion 属性列表
	protected const byte keyword_options = 1;

	protected const byte keyword_on_selectid = 2;

	protected const byte keyword_title = 3;
}
