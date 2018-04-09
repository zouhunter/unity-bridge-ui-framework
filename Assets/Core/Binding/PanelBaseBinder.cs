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
using System;

namespace BridgeUI.Binding
{

    public class PanelBaseBinder : PropertyBinder
    {
        public PanelBaseBinder(PanelBase panel) : base(panel) { }
        /// <summary>
        /// 接收文字显示
        /// </summary>
        /// <param name="text"></param>
        /// <param name="name"></param>
        internal void AddText(Text text, string name)
        {
            AddToModel<string>(name, (value) => { text.text = value; });
        }
        /// <summary>
        /// 接收按扭点击事件
        /// </summary>
        /// <param name="button"></param>
        /// <param name="methodName"></param>
        internal void AddButton(Button button, string methodName)
        {
            UnityAction action = () => { Invoke(Context,methodName, button, new RoutedEventArgs(Context)); };

            binders += viewModel =>
            {
                button.onClick.AddListener(action);
            };

            unbinders += viewModel =>
            {
                button.onClick.RemoveListener(action);
            };
        }

        internal void AddToggle(Toggle toggle, string methodName)
        {
            UnityAction<bool> action = (isOn) =>
            {
                Invoke(Context, methodName, toggle, new RoutedEventArgs(Context));
            };

            binders += viewModel =>
            {
                toggle.onValueChanged.AddListener(action);
            };

            unbinders += viewModel =>
            {
                toggle.onValueChanged.RemoveListener(action);
            };
        }

        internal void AddSlider(Slider m_slider, string v)
        {

        }
    }

}