///*************************************************************************************
///* 更新时间：       2019-02-15 03:34:36
///* 说    明：       1.本代码由工具自动生成
///                   2.如需要扩展支持的类型，请在ObjectRefEditor中进行编辑
///                   3.注意手动添加的代码不一定能正常解析！
///* ************************************************************************************/

using System;
using BridgeUI;
using BridgeUI.Binding;
using UnityEngine;
using BridgeUI.Extend.XLua;

namespace GView {
	public class LuaPanel01_Reference : LuaPanel_Reference
	{
		[SerializeField]
		private UnityEngine.UI.RawImage m_RawImage;
		[SerializeField]
		private UnityEngine.UI.Image m_btnPic;
		[SerializeField]
		private UnityEngine.UI.Slider m_Slider;
		[SerializeField]
		private UnityEngine.UI.Button m_Button;
		[SerializeField]
		private UnityEngine.UI.Toggle m_Toggle;
		[SerializeField]
		private UnityEngine.UI.Dropdown m_Dropdown;
		[SerializeField]
		private UnityEngine.UI.InputField m_InputField;
		[SerializeField]
		private UnityEngine.UI.Image m_Image;
		[SerializeField]
		private UnityEngine.UI.Text m_Text;
		[SerializeField]
		private UnityEngine.UI.ScrollRect m_ScrollView;

		#region Propertys
		public UnityEngine.UI.RawImage RawImage
		{
			get { return m_RawImage; }
		}
		public UnityEngine.UI.Image btnPic
		{
			get { return m_btnPic; }
		}
		public UnityEngine.UI.Slider Slider
		{
			get { return m_Slider; }
		}
		public UnityEngine.UI.Button Button
		{
			get { return m_Button; }
		}
		public UnityEngine.UI.Toggle Toggle
		{
			get { return m_Toggle; }
		}
		public UnityEngine.UI.Dropdown Dropdown
		{
			get { return m_Dropdown; }
		}
		public UnityEngine.UI.InputField InputField
		{
			get { return m_InputField; }
		}
		public UnityEngine.UI.Image Image
		{
			get { return m_Image; }
		}
		public UnityEngine.UI.Text Text
		{
			get { return m_Text; }
		}
		public UnityEngine.UI.ScrollRect ScrollView
		{
			get { return m_ScrollView; }
		}

        public override Type CetPanelScriptType()
        {
            return typeof(LuaPanel01);
        }
        #endregion Propertys
    }
}
