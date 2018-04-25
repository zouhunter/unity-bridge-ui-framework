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
using System.Reflection;

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
        /// 注册事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberPath"></param>
        /// <param name="sourceName"></param>
        /// <param name="arguments"></param>
        internal virtual void RegistEvent<T>(string memberPath, string sourceName, params object[] arguments)
        {
            object root = Context;
            MemberInfo member = GetDeepMember(ref root, memberPath);
            Debug.Assert(member != null && (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property));
            Type eventType = null;
            object eventValue = null;
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    eventType = (member as FieldInfo).FieldType;
                    eventValue = (member as FieldInfo).GetValue(root);
                    break;
                case MemberTypes.Property:
                    eventType = (member as PropertyInfo).PropertyType;
                    eventValue = (member as PropertyInfo).GetValue(root, null);
                    break;
            }

            MethodInfo addListener = eventType.GetMethod("AddListener");
            MethodInfo removeListener = eventType.GetMethod("RemoveListener");

            ParameterInfo argType = addListener.GetParameters()[0];
            ComponentData<T> data = new ComponentData<T>()
            {
                sourceName = sourceName,
                args = arguments,
                sender = root,
                panelBinder = this,
            };
            var argumentLength = argType.ParameterType.GetGenericArguments().Length;
            MethodInfo method = this.GetType().GetMethod("InvokeAction" + argumentLength);

            Delegate dele = Delegate.CreateDelegate(argType.ParameterType, data, method.MakeGenericMethod(typeof(T)));

            binders += viewModel =>
            {
                addListener.Invoke(eventValue, new object[] { dele });
            };

            unbinders += viewModel =>
            {
                removeListener.Invoke(eventValue, new object[] { dele });
            };
        }

        public static void InvokeAction0<T>(ComponentData<T> data)
        {
            var viewModel = data.panelBinder.viewModel;
            var prop = viewModel.GetBindableProperty<T>(data.sourceName);

            if (prop.ValueBoxed != null)
            {
                typeof(T).GetMethod("Invoke").Invoke(prop.ValueBoxed, new object[] { data.panelBinder.Context, data.sender, data.args });
            }
        }

        public static void InvokeAction1<T>(ComponentData<T> data,object arg1)
        {
            var viewModel = data.panelBinder.viewModel;
            var prop = viewModel.GetBindableProperty<T>(data.sourceName);
            if (prop.ValueBoxed != null)
            {
                typeof(T).GetMethod("Invoke").Invoke(prop.ValueBoxed, new object[] { data.panelBinder.Context, data.sender, data.args });
            }
        }

        /// <summary>
        /// 下拉框事件
        /// </summary>
        /// <param name="dropdown"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        internal void RegistDropdownEvent(Dropdown dropdown, string methodName, params object[] args)
        {
            UnityAction<int> action = (idex) =>
            {
                var prop = viewModel.GetBindableProperty<DropdownEvent>(methodName);
                if (prop.Value != null)
                {
                    var func = prop.Value;
                    func.Invoke((PanelBase)Context, dropdown, args);
                }
            };

            binders += viewModel =>
            {
                dropdown.onValueChanged.AddListener(action);
            };

            unbinders += viewModel =>
            {
                dropdown.onValueChanged.RemoveListener(action);
            };
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

        /// <summary>
        /// 注册inputfield事件
        /// </summary>
        /// <param name="inputfield"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        internal virtual void RegistInputFieldEvent(InputField inputfield, string methodName, params object[] args)
        {
            UnityAction<string> action = (isOn) =>
            {
                var prop = viewModel.GetBindableProperty<InputFieldEvent>(methodName);
                if (prop.Value != null)
                {
                    var func = prop.Value;
                    func.Invoke((PanelBase)Context, inputfield, args);
                }
            };

            binders += viewModel =>
            {
                inputfield.onValueChanged.AddListener(action);
            };

            unbinders += viewModel =>
            {
                inputfield.onValueChanged.RemoveListener(action);
            };
        }
    }

}