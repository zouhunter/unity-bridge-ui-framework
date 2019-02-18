///*************************************************************************************
///* 更新时间：       2019-02-15 11:36:17
///* 说    明：       1.本代码由工具自动生成
///                   2.如需要扩展支持的类型，请在ObjectRefEditor中进行编辑
///                   3.注意手动添加的代码不一定能正常解析！
///* ************************************************************************************/

using System;
using BridgeUI;
using BridgeUI.Binding;
using UnityEngine;

namespace GView {
	public class SperatePanel_Reference : BindingReference<SperatePanel_Reference.Data >
	{
		[SerializeField]
		private UnityEngine.UI.Button m_close;
		[SerializeField]
		private UnityEngine.UI.Button m_apply_btn;
		[SerializeField]
		private UnityEngine.UI.Text m_title;
		[SerializeField]
		private System.UInt16 m_uid;
        [SerializeField]
        private UnityEngine.UI.Toggle m_switcher;

        [Serializable]
		public struct Data
		{
			public System.UInt16 uid;
			public System.Single speed;
			public System.Int32 font_size;
		}
		#region Propertys
		public System.UInt16 uid
		{
			get { return m_data.uid; }
		}
		public System.Single speed
		{
			get { return m_data.speed; }
		}
		public System.Int32 font_size
		{
			get { return m_data.font_size; }
		}
		public UnityEngine.UI.Button close
		{
			get { return m_close; }
		}
		public UnityEngine.UI.Button apply_btn
		{
			get { return m_apply_btn; }
		}
		public UnityEngine.UI.Text title
		{
			get { return m_title; }
		}
		public UnityEngine.UI.Toggle switcher
		{
			get { return m_switcher; }
		}
		#endregion Propertys
	}
}
