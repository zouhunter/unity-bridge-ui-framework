using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace BridgeUI.Binding
{
    public static class UGUIBindingHelper
    {
        /// <summary>
        /// 输入框
        /// </summary>
        /// <param name="Binder"></param>
        /// <param name="inputField"></param>
        /// <param name="sourceName"></param>
        public static void BindingInputField(this PropertyBinder Binder, InputField inputField, string sourceName)
        {
            Binder.RegistValueChange<string>(x => inputField.text = x, sourceName);
            Binder.RegistValueEvent(inputField.onValueChanged, sourceName);
        }
        /// <summary>
        /// 文本框
        /// </summary>
        /// <param name="Binder"></param>
        /// <param name="inputField"></param>
        /// <param name="sourceName"></param>
        public static void BindingText(this PropertyBinder Binder, Text textComponent, string sourceName)
        {
            Binder.RegistValueChange<string>(x => textComponent.text = x, sourceName);
        }
        /// <summary>
        /// 滑动条
        /// </summary>
        /// <param name="Binder"></param>
        /// <param name="sliderComponent"></param>
        /// <param name="sourceName"></param>
        public static void BindingSlider(this PropertyBinder Binder, Slider sliderComponent, string sourceName)
        {
            Binder.RegistValueChange<float>(x => sliderComponent.value = x, sourceName);
            Binder.RegistValueEvent(sliderComponent.onValueChanged, sourceName);
        }
        /// <summary>
        /// 下拉框
        /// </summary>
        /// <param name="Binder"></param>
        /// <param name="dropdown"></param>
        /// <param name="sourceName"></param>
        public static void BindingDropdown(this PropertyBinder Binder, Dropdown dropdown, string sourceName)
        {
            Binder.RegistValueChange<int>(x => dropdown.value = x, sourceName);
            Binder.RegistValueEvent(dropdown.onValueChanged, sourceName);
        }

        /// <summary>
        /// 下拉框
        /// </summary>
        /// <param name="Binder"></param>
        /// <param name="button"></param>
        /// <param name="sourceName"></param>
        public static void BindingButton(this PropertyBinder Binder, Button button, string sourceName)
        {
            Binder.RegistEvent(button.onClick, sourceName);
        }

        /// <summary>
        /// 双选
        /// </summary>
        /// <param name="Binder"></param>
        /// <param name="toggle"></param>
        /// <param name="sourceName"></param>
        public static void BindingToggle(this PropertyBinder Binder, Toggle toggle, string sourceName)
        {
            Binder.RegistValueChange<bool>(x=> toggle.isOn = x, sourceName);
            Binder.RegistValueEvent(toggle.onValueChanged, sourceName);
        }

        /// <summary>
        /// RawImage
        /// </summary>
        /// <param name="Binder"></param>
        /// <param name="rawImage"></param>
        /// <param name="sourceName"></param>
        public static void BindingRawImage(this PropertyBinder Binder,RawImage rawImage,string sourceName)
        {
            Binder.RegistValueChange<Texture>(x => rawImage.texture = x, sourceName);
        }

        /// <summary>
        /// Image
        /// </summary>
        /// <param name="Binder"></param>
        /// <param name="image"></param>
        /// <param name="sourceName"></param>
        public static void BindingImage(this PropertyBinder Binder, Image image, string sourceName)
        {
            Binder.RegistValueChange<Sprite>(x => image.sprite = x, sourceName);
        }
    }
}