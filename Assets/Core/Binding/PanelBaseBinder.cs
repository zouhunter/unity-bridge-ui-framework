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
        /// <param name="sourceName"></param>
        internal void RegistTextView(Text text, string sourceName)
        {
            RegistValueCharge<string>((value) => { text.text = value; }, sourceName);
        }

        /// <summary>
        /// 接收图片显示
        /// </summary>
        /// <param name="image"></param>
        /// <param name="sourceName"></param>
        internal void RegistImageView(Image image, string sourceName)
        {
            RegistValueCharge<Sprite>((value) => { image.sprite = value; }, sourceName);
        }

        /// <summary>
        /// 接收图片显示
        /// </summary>
        /// <param name="image"></param>
        /// <param name="sourceName"></param>
        internal void RegistRawImageView(RawImage image, string sourceName)
        {
            RegistValueCharge<Texture>((value) => { image.texture = value; }, sourceName);
        }
        /// <summary>
        /// 接收按扭点击事件
        /// </summary>
        /// <param name="button"></param>
        /// <param name="methodName"></param>
        internal void RegistButtonEvent(Button button, string methodName, params object[] args)
        {
            UnityAction action = () =>
            {
                if (viewModel[methodName] != null && viewModel[methodName].Value is ButtonEvent)
                {
                    var func = (ButtonEvent)viewModel[methodName].Value;
                    func.Invoke((PanelBase)Context, button, args);
                }
            };

            binders += viewModel =>
            {
                viewModel.GetBindableProperty<ButtonEvent>(methodName);
                button.onClick.AddListener(action);
            };

            unbinders += viewModel =>
            {
                viewModel.GetBindableProperty<ButtonEvent>(methodName);
                button.onClick.RemoveListener(action);
            };
        }
        /// <summary>
        /// 注册toggle事件
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="methodName"></param>
        internal void RegistToggleEvent(Toggle toggle, string methodName, params object[] args)
        {
            UnityAction<bool> action = (isOn) =>
            {
                if (viewModel[methodName] != null && viewModel[methodName].Value is ToggleEvent)
                {
                    var func = (ToggleEvent)viewModel[methodName].Value;
                    func.Invoke((PanelBase)Context, toggle, args);
                }
            };

            binders += viewModel =>
            {
                viewModel.GetBindableProperty<ToggleEvent>(methodName);
                toggle.onValueChanged.AddListener(action);
            };

            unbinders += viewModel =>
            {
                viewModel.GetBindableProperty<ToggleEvent>(methodName);
                toggle.onValueChanged.RemoveListener(action);
            };
        }
        /// <summary>
        /// 注册slider事件
        /// </summary>
        /// <param name="m_slider"></param>
        /// <param name="v"></param>
        internal void RegistSliderEvent(Slider m_slider, string methodName)
        {

        }
    }

}