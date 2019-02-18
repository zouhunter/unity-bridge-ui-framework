using System;
using BridgeUI.Binding;

///*************************************************************************************
///* 作    者：       邹杭特
///* 创建时间：       2019-02-15 05:30:47
///* 说    明：       1.此代码由工具生成
///                   2.不支持编写的代码
///                   3.请使用继承方式使用
///* ************************************************************************************/

namespace GView
{
	/*绑定协议
	 * 001 : btn_color
	 * 002 : btn_color1
	 * 003 : on_cull_statechanged
	 * 004 : on_cull_statechanged1
	 * 005 : on_button_clicked
	 * 006 : on_button_clicked1
	 * 007 : on_slider_switched
	 * 008 : on_slider_switched1
	 * 009 : on_inputfield_edited
	 * 010 : on_inputfield_edited1
	 * 011 : on_dropdown_switched
	 * 012 : on_toggle_switched
	 * 013 : image
	 * 014 : image_color
	 * 015 : text
	 * 016 : on_scrollview_changed
	 */
	public partial class LuaPanel01:BridgeUI.Extend.XLua.LuaPanel
	{
		public const byte keyword_btn_color = 001;
		public const byte keyword_btn_color1 = 002;
		public const byte keyword_on_cull_statechanged = 003;
		public const byte keyword_on_cull_statechanged1 = 004;
		public const byte keyword_on_button_clicked = 005;
		public const byte keyword_on_button_clicked1 = 006;
		public const byte keyword_on_slider_switched = 007;
		public const byte keyword_on_slider_switched1 = 008;
		public const byte keyword_on_inputfield_edited = 009;
		public const byte keyword_on_inputfield_edited1 = 010;
		public const byte keyword_on_dropdown_switched = 011;
		public const byte keyword_on_toggle_switched = 012;
		public const byte keyword_image = 013;
		public const byte keyword_image_color = 014;
		public const byte keyword_text = 015;
		public const byte keyword_on_scrollview_changed = 016;

		/// <summary>
		/// 代码绑定
		/// </summary>
		protected override void OnBinding(UnityEngine.GameObject target)
		{
			base.OnBinding(target);
			var binding = target.GetComponent<LuaPanel01_Reference>();
			if (binding != null)
			{
				Binder.RegistValueChange<UnityEngine.Color>(x => binding.btnPic.color = x, keyword_btn_color);
				Binder.RegistValueChange<UnityEngine.Color>(x => binding.btnPic.color = x, keyword_btn_color1);
				Binder.RegistEvent(binding.btnPic.onCullStateChanged, keyword_on_cull_statechanged,binding.btnPic);
				Binder.RegistEvent(binding.btnPic.onCullStateChanged, keyword_on_cull_statechanged1);
				Binder.RegistEvent(binding.Button.onClick, keyword_on_button_clicked);
				Binder.RegistEvent(binding.Button.onClick, keyword_on_button_clicked1,binding.Button);
				Binder.RegistEvent(binding.Slider.onValueChanged, keyword_on_slider_switched,binding.Slider);
				Binder.RegistEvent(binding.Slider.onValueChanged, keyword_on_slider_switched1);
				Binder.RegistEvent(binding.InputField.onEndEdit, keyword_on_inputfield_edited,binding.InputField);
				Binder.RegistEvent(binding.InputField.onEndEdit, keyword_on_inputfield_edited1,binding.InputField);
				Binder.RegistEvent(binding.Dropdown.onValueChanged, keyword_on_dropdown_switched,binding.Dropdown);
				Binder.RegistEvent(binding.Toggle.onValueChanged, keyword_on_toggle_switched,binding.Toggle);
				Binder.RegistValueChange<UnityEngine.Sprite>(x => binding.Image.sprite = x, keyword_image);
				Binder.RegistValueChange<UnityEngine.Color>(x => binding.Image.color = x, keyword_image_color);
				Binder.RegistValueChange<System.String>(x => binding.Text.text = x, keyword_text);
				Binder.RegistEvent(binding.ScrollView.onValueChanged, keyword_on_scrollview_changed,binding.ScrollView);
			}
		}

		/// <summary>
		/// 显示模型模板
		/// <summary>
		public class LogicBase : BridgeUI.Binding.ViewModel
		{
			#region BindablePropertys
			protected BindableProperty<UnityEngine.Color> m_btn_color;
			protected BindableProperty<UnityEngine.Color> m_btn_color1;
			protected BindableProperty<BridgeUI.Binding.PanelAction<System.Boolean,UnityEngine.UI.Image>> m_on_cull_statechanged;
			protected BindableProperty<BridgeUI.Binding.PanelAction<System.Boolean>> m_on_cull_statechanged1;
			protected BindableProperty<BridgeUI.Binding.PanelAction> m_on_button_clicked;
			protected BindableProperty<BridgeUI.Binding.PanelAction<UnityEngine.UI.Button>> m_on_button_clicked1;
			protected BindableProperty<BridgeUI.Binding.PanelAction<System.Single,UnityEngine.UI.Slider>> m_on_slider_switched;
			protected BindableProperty<BridgeUI.Binding.PanelAction<System.Single>> m_on_slider_switched1;
			protected BindableProperty<BridgeUI.Binding.PanelAction<System.String,UnityEngine.UI.InputField>> m_on_inputfield_edited;
			protected BindableProperty<BridgeUI.Binding.PanelAction<System.String,UnityEngine.UI.InputField>> m_on_inputfield_edited1;
			protected BindableProperty<BridgeUI.Binding.PanelAction<System.Int32,UnityEngine.UI.Dropdown>> m_on_dropdown_switched;
			protected BindableProperty<BridgeUI.Binding.PanelAction<System.Boolean,UnityEngine.UI.Toggle>> m_on_toggle_switched;
			protected BindableProperty<UnityEngine.Sprite> m_image;
			protected BindableProperty<UnityEngine.Color> m_image_color;
			protected BindableProperty<System.String> m_text;
			protected BindableProperty<BridgeUI.Binding.PanelAction<UnityEngine.Vector2,UnityEngine.UI.ScrollRect>> m_on_scrollview_changed;
			#endregion BindablePropertys

			#region Propertys
			public UnityEngine.Color btn_color
			{
				get
				{
					if(m_btn_color == null)
					{
						m_btn_color = GetBindableProperty<UnityEngine.Color>(keyword_btn_color);
					}
					return m_btn_color;
				}
				set
				{
					if(m_btn_color == null)
					{
						m_btn_color = GetBindableProperty<UnityEngine.Color>(keyword_btn_color);
					}
					m_btn_color.Value = value;
				}
			}
			public UnityEngine.Color btn_color1
			{
				get
				{
					if(m_btn_color1 == null)
					{
						m_btn_color1 = GetBindableProperty<UnityEngine.Color>(keyword_btn_color1);
					}
					return m_btn_color1;
				}
				set
				{
					if(m_btn_color1 == null)
					{
						m_btn_color1 = GetBindableProperty<UnityEngine.Color>(keyword_btn_color1);
					}
					m_btn_color1.Value = value;
				}
			}
			public BridgeUI.Binding.PanelAction<System.Boolean,UnityEngine.UI.Image> on_cull_statechanged
			{
				get
				{
					if(m_on_cull_statechanged == null)
					{
						m_on_cull_statechanged = GetBindableProperty<BridgeUI.Binding.PanelAction<System.Boolean,UnityEngine.UI.Image>>(keyword_on_cull_statechanged);
					}
					return m_on_cull_statechanged;
				}
				set
				{
					if(m_on_cull_statechanged == null)
					{
						m_on_cull_statechanged = GetBindableProperty<BridgeUI.Binding.PanelAction<System.Boolean,UnityEngine.UI.Image>>(keyword_on_cull_statechanged);
					}
					m_on_cull_statechanged.Value = value;
				}
			}
			public BridgeUI.Binding.PanelAction<System.Boolean> on_cull_statechanged1
			{
				get
				{
					if(m_on_cull_statechanged1 == null)
					{
						m_on_cull_statechanged1 = GetBindableProperty<BridgeUI.Binding.PanelAction<System.Boolean>>(keyword_on_cull_statechanged1);
					}
					return m_on_cull_statechanged1;
				}
				set
				{
					if(m_on_cull_statechanged1 == null)
					{
						m_on_cull_statechanged1 = GetBindableProperty<BridgeUI.Binding.PanelAction<System.Boolean>>(keyword_on_cull_statechanged1);
					}
					m_on_cull_statechanged1.Value = value;
				}
			}
			public BridgeUI.Binding.PanelAction on_button_clicked
			{
				get
				{
					if(m_on_button_clicked == null)
					{
						m_on_button_clicked = GetBindableProperty<BridgeUI.Binding.PanelAction>(keyword_on_button_clicked);
					}
					return m_on_button_clicked;
				}
				set
				{
					if(m_on_button_clicked == null)
					{
						m_on_button_clicked = GetBindableProperty<BridgeUI.Binding.PanelAction>(keyword_on_button_clicked);
					}
					m_on_button_clicked.Value = value;
				}
			}
			public BridgeUI.Binding.PanelAction<UnityEngine.UI.Button> on_button_clicked1
			{
				get
				{
					if(m_on_button_clicked1 == null)
					{
						m_on_button_clicked1 = GetBindableProperty<BridgeUI.Binding.PanelAction<UnityEngine.UI.Button>>(keyword_on_button_clicked1);
					}
					return m_on_button_clicked1;
				}
				set
				{
					if(m_on_button_clicked1 == null)
					{
						m_on_button_clicked1 = GetBindableProperty<BridgeUI.Binding.PanelAction<UnityEngine.UI.Button>>(keyword_on_button_clicked1);
					}
					m_on_button_clicked1.Value = value;
				}
			}
			public BridgeUI.Binding.PanelAction<System.Single,UnityEngine.UI.Slider> on_slider_switched
			{
				get
				{
					if(m_on_slider_switched == null)
					{
						m_on_slider_switched = GetBindableProperty<BridgeUI.Binding.PanelAction<System.Single,UnityEngine.UI.Slider>>(keyword_on_slider_switched);
					}
					return m_on_slider_switched;
				}
				set
				{
					if(m_on_slider_switched == null)
					{
						m_on_slider_switched = GetBindableProperty<BridgeUI.Binding.PanelAction<System.Single,UnityEngine.UI.Slider>>(keyword_on_slider_switched);
					}
					m_on_slider_switched.Value = value;
				}
			}
			public BridgeUI.Binding.PanelAction<System.Single> on_slider_switched1
			{
				get
				{
					if(m_on_slider_switched1 == null)
					{
						m_on_slider_switched1 = GetBindableProperty<BridgeUI.Binding.PanelAction<System.Single>>(keyword_on_slider_switched1);
					}
					return m_on_slider_switched1;
				}
				set
				{
					if(m_on_slider_switched1 == null)
					{
						m_on_slider_switched1 = GetBindableProperty<BridgeUI.Binding.PanelAction<System.Single>>(keyword_on_slider_switched1);
					}
					m_on_slider_switched1.Value = value;
				}
			}
			public BridgeUI.Binding.PanelAction<System.String,UnityEngine.UI.InputField> on_inputfield_edited
			{
				get
				{
					if(m_on_inputfield_edited == null)
					{
						m_on_inputfield_edited = GetBindableProperty<BridgeUI.Binding.PanelAction<System.String,UnityEngine.UI.InputField>>(keyword_on_inputfield_edited);
					}
					return m_on_inputfield_edited;
				}
				set
				{
					if(m_on_inputfield_edited == null)
					{
						m_on_inputfield_edited = GetBindableProperty<BridgeUI.Binding.PanelAction<System.String,UnityEngine.UI.InputField>>(keyword_on_inputfield_edited);
					}
					m_on_inputfield_edited.Value = value;
				}
			}
			public BridgeUI.Binding.PanelAction<System.String,UnityEngine.UI.InputField> on_inputfield_edited1
			{
				get
				{
					if(m_on_inputfield_edited1 == null)
					{
						m_on_inputfield_edited1 = GetBindableProperty<BridgeUI.Binding.PanelAction<System.String,UnityEngine.UI.InputField>>(keyword_on_inputfield_edited1);
					}
					return m_on_inputfield_edited1;
				}
				set
				{
					if(m_on_inputfield_edited1 == null)
					{
						m_on_inputfield_edited1 = GetBindableProperty<BridgeUI.Binding.PanelAction<System.String,UnityEngine.UI.InputField>>(keyword_on_inputfield_edited1);
					}
					m_on_inputfield_edited1.Value = value;
				}
			}
			public BridgeUI.Binding.PanelAction<System.Int32,UnityEngine.UI.Dropdown> on_dropdown_switched
			{
				get
				{
					if(m_on_dropdown_switched == null)
					{
						m_on_dropdown_switched = GetBindableProperty<BridgeUI.Binding.PanelAction<System.Int32,UnityEngine.UI.Dropdown>>(keyword_on_dropdown_switched);
					}
					return m_on_dropdown_switched;
				}
				set
				{
					if(m_on_dropdown_switched == null)
					{
						m_on_dropdown_switched = GetBindableProperty<BridgeUI.Binding.PanelAction<System.Int32,UnityEngine.UI.Dropdown>>(keyword_on_dropdown_switched);
					}
					m_on_dropdown_switched.Value = value;
				}
			}
			public BridgeUI.Binding.PanelAction<System.Boolean,UnityEngine.UI.Toggle> on_toggle_switched
			{
				get
				{
					if(m_on_toggle_switched == null)
					{
						m_on_toggle_switched = GetBindableProperty<BridgeUI.Binding.PanelAction<System.Boolean,UnityEngine.UI.Toggle>>(keyword_on_toggle_switched);
					}
					return m_on_toggle_switched;
				}
				set
				{
					if(m_on_toggle_switched == null)
					{
						m_on_toggle_switched = GetBindableProperty<BridgeUI.Binding.PanelAction<System.Boolean,UnityEngine.UI.Toggle>>(keyword_on_toggle_switched);
					}
					m_on_toggle_switched.Value = value;
				}
			}
			public UnityEngine.Sprite image
			{
				get
				{
					if(m_image == null)
					{
						m_image = GetBindableProperty<UnityEngine.Sprite>(keyword_image);
					}
					return m_image;
				}
				set
				{
					if(m_image == null)
					{
						m_image = GetBindableProperty<UnityEngine.Sprite>(keyword_image);
					}
					m_image.Value = value;
				}
			}
			public UnityEngine.Color image_color
			{
				get
				{
					if(m_image_color == null)
					{
						m_image_color = GetBindableProperty<UnityEngine.Color>(keyword_image_color);
					}
					return m_image_color;
				}
				set
				{
					if(m_image_color == null)
					{
						m_image_color = GetBindableProperty<UnityEngine.Color>(keyword_image_color);
					}
					m_image_color.Value = value;
				}
			}
			public System.String text
			{
				get
				{
					if(m_text == null)
					{
						m_text = GetBindableProperty<System.String>(keyword_text);
					}
					return m_text;
				}
				set
				{
					if(m_text == null)
					{
						m_text = GetBindableProperty<System.String>(keyword_text);
					}
					m_text.Value = value;
				}
			}
			public BridgeUI.Binding.PanelAction<UnityEngine.Vector2,UnityEngine.UI.ScrollRect> on_scrollview_changed
			{
				get
				{
					if(m_on_scrollview_changed == null)
					{
						m_on_scrollview_changed = GetBindableProperty<BridgeUI.Binding.PanelAction<UnityEngine.Vector2,UnityEngine.UI.ScrollRect>>(keyword_on_scrollview_changed);
					}
					return m_on_scrollview_changed;
				}
				set
				{
					if(m_on_scrollview_changed == null)
					{
						m_on_scrollview_changed = GetBindableProperty<BridgeUI.Binding.PanelAction<UnityEngine.Vector2,UnityEngine.UI.ScrollRect>>(keyword_on_scrollview_changed);
					}
					m_on_scrollview_changed.Value = value;
				}
			}
			#endregion Propertys
		}
	}
}
