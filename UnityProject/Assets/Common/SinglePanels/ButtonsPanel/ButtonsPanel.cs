using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
namespace BridgeUI.Common
{
    public class ButtonsPanel : SinglePanel
    {
        [SerializeField, HideInInspector]
        protected List<Button> btns;

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < btns.Count; i++)
            {
                var index = i;
                btns[index].onClick.AddListener(() =>
                {
                    this.OpenSubPanel(index);
                });
            }
        }
    }
}