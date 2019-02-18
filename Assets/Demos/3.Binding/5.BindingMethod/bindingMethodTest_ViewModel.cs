///*************************************************************************************
///* 作    者：       未指定用户，请在Preference中填写！
///* 创建时间：       2019-01-27 12:51:38
///* 说    明：       1.部分代码自动生成
///                   2.可在其中编写代码
///                   3.宏定义暂无法正常解析
///* ************************************************************************************/
///<summary>
///[代码说明信息]
///<summary>
public abstract class bindingMethodTest_ViewModel : BridgeUI.Binding.ViewModel
{
	#region 属性列表
	private BridgeUI.Binding.BindableProperty<BridgeUI.Binding.PanelAction> _ButtonClicked;

	protected BridgeUI.Binding.PanelAction ButtonClicked {
		get {
			if (_ButtonClicked==null) {
				_ButtonClicked=GetBindableProperty<BridgeUI.Binding.PanelAction>(keyword_ButtonClicked);
			}
			return _ButtonClicked.Value;
		}
		set {
			if (_ButtonClicked==null) {
				_ButtonClicked=GetBindableProperty<BridgeUI.Binding.PanelAction>(keyword_ButtonClicked);
			}
			_ButtonClicked.Value = value;
		}
	}

	private BridgeUI.Binding.BindableProperty<System.String[]> _value;

	protected System.String[] value {
		get {
			if (_value==null) {
				_value=GetBindableProperty<System.String[]>(keyword_value);
			}
			return _value.Value;
		}
		set {
			if (_value==null) {
				_value=GetBindableProperty<System.String[]>(keyword_value);
			}
			_value.Value = value;
		}
	}

	private BridgeUI.Binding.BindableProperty<UnityEngine.UI.ColorBlock> _color;

	protected UnityEngine.UI.ColorBlock color {
		get {
			if (_color==null) {
				_color=GetBindableProperty<UnityEngine.UI.ColorBlock>(keyword_color);
			}
			return _color.Value;
		}
		set {
			if (_color==null) {
				_color=GetBindableProperty<UnityEngine.UI.ColorBlock>(keyword_color);
			}
			_color.Value = value;
		}
	}

	#endregion 属性列表
	protected byte keyword_value = 1;

	protected byte keyword_color = 2;

	protected byte keyword_ButtonClicked = 3;
}
