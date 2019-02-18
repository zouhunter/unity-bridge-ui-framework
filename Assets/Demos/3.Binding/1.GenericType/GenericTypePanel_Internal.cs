using System;
using BridgeUI.Binding;

///*************************************************************************************
///* 作    者：       邹杭特
///* 创建时间：       2019-02-15 01:23:42
///* 说    明：       1.此代码由工具生成
///                   2.不支持编写的代码
///                   3.请使用继承方式使用
///* ************************************************************************************/

namespace GView
{
	/*绑定协议
	 * 001 : array
	 * 002 : list
	 * 003 : dic
	 * 004 : complex
	 */
	public partial class GenericTypePanel:BindingViewBase
	{
		public const byte keyword_array = 001;
		public const byte keyword_list = 002;
		public const byte keyword_dic = 003;
		public const byte keyword_complex = 004;

		/// <summary>
		/// 代码绑定
		/// </summary>
		protected override void OnBinding(UnityEngine.GameObject target)
		{
			base.OnBinding(target);
			var binding = target.GetComponent<GenericTypePanel_Reference>();
			if (binding != null)
			{
				Binder.RegistValueChange<System.String[]>(x => binding.subcomponent.stringTest = x, keyword_array);
				Binder.RegistValueChange<System.Collections.Generic.List<System.String>>(x => binding.subcomponent.listTest = x, keyword_list);
				Binder.RegistValueChange<System.Collections.Generic.Dictionary<System.String,System.String>>(x => binding.subcomponent.dicTest = x, keyword_dic);
				Binder.RegistValueChange<System.Collections.Generic.List<System.Collections.Generic.List<System.String[]>>>(x => binding.subcomponent.complexList = x, keyword_complex);
				RegistUIControl(binding.subcomponent);
			}
		}

		/// <summary>
		/// 显示模型模板
		/// <summary>
		public class LogicBase : BridgeUI.Binding.ViewModel
		{
			#region BindablePropertys
			protected BindableProperty<System.String[]> m_array;
			protected BindableProperty<System.Collections.Generic.List<System.String>> m_list;
			protected BindableProperty<System.Collections.Generic.Dictionary<System.String,System.String>> m_dic;
			protected BindableProperty<System.Collections.Generic.List<System.Collections.Generic.List<System.String[]>>> m_complex;
			#endregion BindablePropertys

			#region Propertys
			public System.String[] array
			{
				get
				{
					if(m_array == null)
					{
						m_array = GetBindableProperty<System.String[]>(keyword_array);
					}
					return m_array;
				}
				set
				{
					if(m_array == null)
					{
						m_array = GetBindableProperty<System.String[]>(keyword_array);
					}
					m_array.Value = value;
				}
			}
			public System.Collections.Generic.List<System.String> list
			{
				get
				{
					if(m_list == null)
					{
						m_list = GetBindableProperty<System.Collections.Generic.List<System.String>>(keyword_list);
					}
					return m_list;
				}
				set
				{
					if(m_list == null)
					{
						m_list = GetBindableProperty<System.Collections.Generic.List<System.String>>(keyword_list);
					}
					m_list.Value = value;
				}
			}
			public System.Collections.Generic.Dictionary<System.String,System.String> dic
			{
				get
				{
					if(m_dic == null)
					{
						m_dic = GetBindableProperty<System.Collections.Generic.Dictionary<System.String,System.String>>(keyword_dic);
					}
					return m_dic;
				}
				set
				{
					if(m_dic == null)
					{
						m_dic = GetBindableProperty<System.Collections.Generic.Dictionary<System.String,System.String>>(keyword_dic);
					}
					m_dic.Value = value;
				}
			}
			public System.Collections.Generic.List<System.Collections.Generic.List<System.String[]>> complex
			{
				get
				{
					if(m_complex == null)
					{
						m_complex = GetBindableProperty<System.Collections.Generic.List<System.Collections.Generic.List<System.String[]>>>(keyword_complex);
					}
					return m_complex;
				}
				set
				{
					if(m_complex == null)
					{
						m_complex = GetBindableProperty<System.Collections.Generic.List<System.Collections.Generic.List<System.String[]>>>(keyword_complex);
					}
					m_complex.Value = value;
				}
			}
			#endregion Propertys
		}
	}
}
