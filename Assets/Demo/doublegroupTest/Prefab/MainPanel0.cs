///*************************************************************************************   
//    * 作    者：       DefaultCompany
//    * 时    间：       2018-04-10 12:30:32
//    * 说    明：       1.本脚本由电脑自动生成
//                       2.请尽量不要在其中写代码
//                       3.更无法使用协程及高版本特性
//* ************************************************************************************/

//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Audio;
//using UnityEngine.Events;
//using UnityEngine.Sprites;
//using UnityEngine.Scripting;
//using UnityEngine.Assertions;
//using UnityEngine.EventSystems;
//using UnityEngine.Assertions.Must;
//using UnityEngine.Assertions.Comparers;
//using System.Collections;
//using System.Collections.Generic;
//using BridgeUI;
//using BridgeUI.Binding;
//using System;
//using System.Reflection;


//public sealed class MainPanel : BridgeUI.PanelBase {
//    public List<string> strs = new List<string>();
//    [UnityEngine.SerializeField()]
//    private UnityEngine.UI.Button m_close;
    
//    [UnityEngine.SerializeField()]
//    private UnityEngine.UI.Button m_openPanel01;
    
//    [UnityEngine.SerializeField()]
//    private UnityEngine.UI.Button m_openPanel02;
    
//    [UnityEngine.SerializeField()]
//    private UnityEngine.UI.Button m_openPanel03;
    
//    [UnityEngine.SerializeField()]
//    private UnityEngine.UI.Text m_title;
    
//    private UnityEngine.UI.Text m_info;
    
//    private UnityEngine.UI.Toggle m_switch;
    
//    private UnityEngine.UI.Slider m_slider;
    
//    private object m_keyword;
    
//    protected override void Awake() {
//        base.Awake();
//        BindingContext = new MainPanelViewModel();
//    }
    
//    protected override void InitComponent() {
//        base.InitComponent();
//        this.m_close.onClick.AddListener(Close);
//    }
    
//    protected override void Binding() {
//        base.Binding();
//        Binder.AddValue("m_keyword", "keyword");
//        Binder.AddText(this.m_title, "title");
//        Binder.AddText(this.m_info, "info");
//        Binder.AddButton(this.m_openPanel01, "OpenPanel01");
//        Binder.AddButton(this.m_openPanel02, "OpenPanel02");
//        Binder.AddButton(this.m_openPanel03, "OpenPanel03");
//        Binder.AddToggle(this.m_switch, "Switch");
//        Binder.AddSlider(this.m_slider, "progress");
//    }
    
//    void Update() {
//        if (Input.GetKeyDown(KeyCode.A)) {
//            Binder["m_switch.isOn"].Value = (this.m_switch.isOn == false);
//        }
//        if (Input.GetKeyDown(KeyCode.V)) {
//            Debug.Log(("m_keyword:" + this.m_keyword));
//        }
//    }
//}
