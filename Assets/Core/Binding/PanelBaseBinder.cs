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
        internal virtual void RegistTextView(Text text, string sourceName)
        {
            RegistValueCharge<string>((value) => { text.text = value; }, sourceName);
        }

        /// <summary>
        /// 接收图片显示
        /// </summary>
        /// <param name="image"></param>
        /// <param name="sourceName"></param>
        internal virtual void RegistImageView(Image image, string sourceName)
        {
            RegistValueCharge<Sprite>((value) => { image.sprite = value; }, sourceName);
        }

        /// <summary>
        /// 接收图片显示
        /// </summary>
        /// <param name="image"></param>
        /// <param name="sourceName"></param>
        internal virtual void RegistRawImageView(RawImage image, string sourceName)
        {
            RegistValueCharge<Texture>((value) => { image.texture = value; }, sourceName);
        }
        /// <summary>
        /// 接收按扭点击事件
        /// </summary>
        /// <param name="button"></param>
        /// <param name="methodName"></param>
        internal virtual void RegistButtonEvent(Button button, string methodName, params object[] args)
        {
            UnityAction action = () =>
            {
                var prop = viewModel.GetBindableProperty<ButtonEvent>(methodName);
                if (prop.Value != null)
                {
                    var func = prop.Value;
                    func.Invoke((PanelBase)Context, button, args);
                }
            };

            binders += viewModel =>
            {
                button.onClick.AddListener(action);
            };

            unbinders += viewModel =>
            {
                button.onClick.RemoveListener(action);
            };
        }

        internal virtual void RegistNormalEvent(UnityEvent uEvent, string methodName)
        {
            UnityAction action = () =>
            {
                var prop = viewModel.GetBindableProperty<UnityAction>(methodName);
                if (prop.Value != null)
                {
                    var func = prop.Value;
                    func.Invoke();
                }
            };

            binders += viewModel =>
            {
                uEvent.AddListener(action);
            };

            unbinders += viewModel =>
            {
                uEvent.RemoveListener(action);
            };
        }

        /// <summary>
        /// 注册toggle事件
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="methodName"></param>
        internal virtual void RegistToggleEvent(Toggle toggle, string methodName, params object[] args)
        {
            UnityAction<bool> action = (isOn) =>
            {
                var prop = viewModel.GetBindableProperty<ToggleEvent>(methodName);
                if (prop.Value != null)
                {
                    var func = prop.Value;
                    func.Invoke((PanelBase)Context, toggle, args);
                }
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

        /// <summary>
        /// 注册slider事件
        /// </summary>
        /// <param name="slider"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        internal virtual void RegistSliderEvent(Slider slider, string methodName, params object[] args)
        {
            UnityAction<float> action = (isOn) =>
            {
                var prop = viewModel.GetBindableProperty<SliderEvent>(methodName);
                if (prop.Value != null)
                {
                    var func = prop.Value;
                    func.Invoke((PanelBase)Context, slider, args);
                }
            };

            binders += viewModel =>
            {
                slider.onValueChanged.AddListener(action);
            };

            unbinders += viewModel =>
            {
                slider.onValueChanged.RemoveListener(action);
            };
        }
    }

}