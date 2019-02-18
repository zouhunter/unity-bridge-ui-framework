using System;
using BridgeUI;
using UnityEngine;

///*************************************************************************************
///* 作    者：       邹杭特
///* 创建时间：       2019-02-16 09:44:16
///* 说    明：       1.此代码由工具生成
///                   2.不支持编写的代码
///                   3.请使用继承方式使用
///* ************************************************************************************/
namespace GView
{
	/// <summary>
	/// 模板方法
	/// <summary>
	public abstract class SperatePanel_Internal:BridgeUI.ViewBase
	{
		private SperatePanel_Reference binding;

		/// <summary>
		/// 绑定
		/// </summary>
		protected override void OnBinding(UnityEngine.GameObject target)
		{
			base.OnBinding(target);
			binding = target.GetComponent<SperatePanel_Reference>();
			if (binding != null)
			{
				binding.close.onClick.AddListener(On_on_close_click);
				binding.apply_btn.onClick.AddListener(On_apply_click);
				binding.apply_btn.onClick.AddListener(On_apply_btn);
				binding.switcher.onValueChanged.AddListener(On_swtch_click);
				Set_uid(binding.uid);
				Set_speed(binding.speed);
				Set_font_size(binding.font_size);
			}
		}

		/// <summary>
		/// 解绑定
		/// </summary>
		protected override void OnUnBinding(UnityEngine.GameObject target)
		{
			if (binding != null)
			{
				binding.close.onClick.RemoveListener(On_on_close_click);
				binding.apply_btn.onClick.RemoveListener(On_apply_click);
				binding.apply_btn.onClick.RemoveListener(On_apply_btn);
				binding.switcher.onValueChanged.RemoveListener(On_swtch_click);
			}
			base.OnUnBinding(target);
		}
		
		#region Set_Functions
		protected void Set_font_size(System.Int32 font_size)
		{
			if (binding)
			{
				if (binding.title != null)
				{
					binding.title.fontSize = font_size;
				}
			}
		}
		protected void Set_title(System.String title)
		{
			if (binding)
			{
				if (binding.title != null)
				{
					binding.title.text = title;
				}
			}
		}
		protected void Set_message(System.String message)
		{
			if (binding)
			{
				if (binding.title != null)
				{
					binding.title.SendMessage(message);
				}
			}
		}
		protected void Set_uid(System.UInt16 uid)
		{
			if (binding)
			{
			}
		}
		protected void Set_speed(System.Single speed)
		{
			if (binding)
			{
			}
		}
		#endregion Set_Functions
		
		#region Abstruct_Functions
		protected abstract void On_on_close_click();
		protected abstract void On_apply_click();
		protected abstract void On_apply_btn();
		protected abstract void On_swtch_click(System.Boolean arg0);
		#endregion Abstruct_Functions
	}
}
