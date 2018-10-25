using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using BridgeUI.Binding;
using System;

public class vm_Control : ViewModel
{
	public System.Boolean show {
		get {
			return GetValue<System.Boolean> ("show");
		}
		set {
			SetValue<System.Boolean> ("show", value);
		}
	}

	public BridgeUI.Binding.PanelAction<System.Boolean> on_show_changed {
		get {
			return GetValue<BridgeUI.Binding.PanelAction<System.Boolean>> ("on_show_changed");
		}
		set {
			SetValue<BridgeUI.Binding.PanelAction<System.Boolean>> ("on_show_changed", value);
		}
	}

	public System.Single scale {
		get {
			return GetValue<System.Single> ("scale");
		}
		set {
			SetValue<System.Single> ("scale", value);
		}
	}

	public System.Single min_scale {
		get {
			return GetValue<System.Single> ("min_scale");
		}
		set {
			SetValue<System.Single> ("min_scale", value);
		}
	}

	public System.Single max_scale {
		get {
			return GetValue<System.Single> ("max_scale");
		}
		set {
			SetValue<System.Single> ("max_scale", value);
		}
	}

	public BridgeUI.Binding.PanelAction<System.Single> on_scale_changed {
		get {
			return GetValue<BridgeUI.Binding.PanelAction<System.Single>> ("on_scale_changed");
		}
		set {
			SetValue<BridgeUI.Binding.PanelAction<System.Single>> ("on_scale_changed", value);
		}
	}

	public System.String current_scale {
		get {
			return GetValue<System.String> ("current_scale");
		}
		set {
			SetValue<System.String> ("current_scale", value);
		}
	}

	public System.String show_title {
		get {
			return GetValue<System.String> ("show_title");
		}
		set {
			SetValue<System.String> ("show_title", value);
		}
	}

	public BridgeUI.Binding.PanelAction turn_green {
		get {
			return GetValue<BridgeUI.Binding.PanelAction> ("turn_green");
		}
		set {
			SetValue<BridgeUI.Binding.PanelAction> ("turn_green", value);
		}
	}

	public BridgeUI.Binding.PanelAction turn_red {
		get {
			return GetValue<BridgeUI.Binding.PanelAction> ("turn_red");
		}
		set {
			SetValue<BridgeUI.Binding.PanelAction> ("turn_red", value);
		}
	}

	[SerializeField]
	private GameObject m_prefab;

	[SerializeField]
	private bool startActive;

	[SerializeField]
	private float minScale;

	[SerializeField]
	private float maxScale;

	[SerializeField]
	private float currentScale;

	private GameObject instence {
		get;
		set;
	}

	void OnEnable ()
	{
		show = startActive;
		scale = currentScale;
		min_scale = minScale;
		max_scale = maxScale;
		current_scale = "";
		UpdateShowText ();
		on_scale_changed = SetGameObjectScale;
		on_show_changed = SetGameObjectVisiable;
		turn_green = SetCubeGreen;
		turn_red = SetCubeRed;
	}

	private void SetCubeGreen (IBindingContext panel)
	{
		if (instence)
			instence.GetComponent<MeshRenderer> ().material.color = Color.green;
	}

	private void SetCubeRed (IBindingContext panel)
	{
		if (instence)
			instence.GetComponent<MeshRenderer> ().material.color = Color.red;
	}

	public override void OnBinding (IBindingContext context)
	{
		Debug.Log ("OnBinding:" + context);
		base.OnBinding (context);
		if (instence == null && m_prefab != null) {
			instence = Instantiate (m_prefab);
			instence.SetActive (startActive);
			SetScaleInteranl (scale);
		}
	}

	public override void OnUnBinding (IBindingContext context)
	{
		base.OnUnBinding (context);
		if (Contexts.Count == 0) {
			if (instence) {
				Destroy (instence);
			}
		}
	}

	private void SetGameObjectVisiable (IBindingContext panel, bool isOn)
	{
		Debug.Log ("SetGameObjectVisiable");
		show = isOn;
		UpdateShowText ();
		if (instence)
			instence.gameObject.SetActive (show);
	}

	private void UpdateShowText ()
	{
		show_title = show ? "隐藏" : "显示";
	}

	private void SetGameObjectScale (IBindingContext panel, float value)
	{
		scale = value;
		SetScaleInteranl (scale);
	}

	private void SetScaleInteranl (float scale)
	{
		current_scale = string.Format ("当前cube 尺寸：" + scale.ToString ("0.0"));
		if (instence)
			instence.gameObject.transform.localScale = Vector3.one * scale;
	}
}
