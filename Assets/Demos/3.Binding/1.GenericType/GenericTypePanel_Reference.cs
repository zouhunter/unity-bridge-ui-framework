///*************************************************************************************
///* 更新时间：       2019-02-15 01:22:17
///* 说    明：       1.本代码由工具自动生成
///                   2.如需要扩展支持的类型，请在ObjectRefEditor中进行编辑
///                   3.注意手动添加的代码不一定能正常解析！
///* ************************************************************************************/

using System;
using BridgeUI;
using BridgeUI.Binding;
using UnityEngine;

namespace GView {
	public class GenericTypePanel_Reference : BindingReference
	{
		[SerializeField]
		private SubComponent m_subcomponent;

		#region Propertys
		public SubComponent subcomponent
		{
			get { return m_subcomponent; }
		}
		#endregion Propertys
	}
}
