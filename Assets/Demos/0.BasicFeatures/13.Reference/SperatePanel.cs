using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BridgeUI;
using BridgeUI.Binding;
using System;

namespace GView
{
    public class SperatePanel: SperatePanel_Internal
    {
        protected override void OnInitialize()
        {
            base.OnInitialize();
            Debug.Log("OnInitialize" + this);
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            Debug.Log("OnRecover" + this);
        }

        protected override void On_apply_btn()
        {
            Debug.Log("On_apply_btn");
        }

        protected override void On_apply_click()
        {
            Debug.Log("On_apply_click");

            this.Open(0);
        }

        protected override void On_on_close_click()
        {
            Debug.Log("On_on_close_click");

            Close();
        }

        protected override void On_swtch_click(bool arg0)
        {
            Debug.Log("On_swtch_click"+ arg0);
        }
    }
}