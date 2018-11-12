using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace BridgeUI.Binding
{
    public class PropertyBinder
    {
        public IBindingContext Context { get; private set; }
        public IViewModel viewModel { get; private set; }

        protected event UnityAction<IViewModel> binders;
        protected event UnityAction<IViewModel> unbinders;
        public static bool log = false;
        public PropertyBinder(IBindingContext context)
        {
            this.Context = context;
        }

        public void Bind(IViewModel viewModel)
        {
            //Debug.Log("Bind:" + viewModel);
            this.viewModel = viewModel;

            if (viewModel != null)
            {
                if (binders != null)
                {
                    binders.Invoke(viewModel);
                }
                viewModel.OnBinding(Context);
            }
        }

        public void Unbind()
        {
            //Debug.Log("UnBind:" + viewModel);
            if (viewModel != null)
            {
                if (unbinders != null)
                {
                    unbinders.Invoke(viewModel);
                }
                viewModel.OnUnBinding(Context);
            }
            this.viewModel = null;
        }
       
        /// <summary>
        /// 注册通用事件
        /// </summary>
        /// <param name="uEvent"></param>
        /// <param name="sourceName"></param>
        public virtual void RegistEvent(UnityEvent uEvent, string sourceName)
        {
            UnityAction action = () =>
            {
                var prop = viewModel.GetBindableProperty<PanelAction>(sourceName);
                if (prop != null && prop.Value != null)
                {
                    var func = prop.Value;
                    func.Invoke(Context);
                }
                else
                {
                    if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
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
        /// 注册通用事件
        /// </summary>
        /// <param name="uEvent"></param>
        /// <param name="sourceName"></param>
        public virtual void RegistEvent<T>(UnityEvent uEvent, string sourceName, T target)
        {
            UnityAction action = () =>
            {
                var prop = viewModel.GetBindableProperty<PanelAction<T>>(sourceName);

                if (prop != null && prop.Value != null)
                {
                    var func = prop.Value;
                    func.Invoke(Context, target);
                }
                else
                {
                    if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
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
        /// 注册事件并传递指定参数
        /// (其中arguments中的参数只能是引用类型,否则无法正常显示)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uEvent"></param>
        /// <param name="sourceName"></param>
        public virtual void RegistEvent<T, P>(UnityEvent<T> uEvent, string sourceName, P target)
        {
            UnityAction<T> action = (x) =>
            {
                var prop = viewModel.GetBindableProperty<PanelAction<P>>(sourceName);

                if (prop != null && prop.Value != null)
                {
                    var func = prop.Value;
                    func.Invoke(Context, target);
                }
                else
                {
                    if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
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
        /// 注册事件并传递指定参数
        /// (其中arguments中的参数只能是引用类型,否则无法正常显示)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uEvent"></param>
        /// <param name="sourceName"></param>
        public virtual void RegistEvent<T, X, P>(UnityEvent<T, X> uEvent, string sourceName, P target)
        {
            UnityAction<T, X> action = (x, y) =>
             {
                 var prop = viewModel.GetBindableProperty<PanelAction<P>>(sourceName);

                 if (prop != null && prop.Value != null)
                 {
                     var func = prop.Value;
                     func.Invoke(Context, target);
                 }
                 else
                 {
                     if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
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
        /// 注册事件并传递指定参数
        /// (其中arguments中的参数只能是引用类型,否则无法正常显示)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uEvent"></param>
        /// <param name="sourceName"></param>
        public virtual void RegistEvent<T, X, Y, P>(UnityEvent<T, X, Y> uEvent, string sourceName, P target)
        {
            UnityAction<T, X, Y> action = (x, y, z) =>
             {
                 var prop = viewModel.GetBindableProperty<PanelAction<P>>(sourceName);

                 if (prop != null && prop.Value != null)
                 {
                     var func = prop.Value;
                     func.Invoke(Context, target);
                 }
                 else
                 {
                     if (log)
                         Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
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
        /// 注册事件并传递指定参数
        /// (其中arguments中的参数只能是引用类型,否则无法正常显示)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uEvent"></param>
        /// <param name="sourceName"></param>
        public virtual void RegistEvent<T, X, Y, Z, P>(UnityEvent<T, X, Y, Z> uEvent, string sourceName, P target)
        {
            UnityAction<T, X, Y, Z> action = (x, y, z, w) =>
             {
                 var prop = viewModel.GetBindableProperty<PanelAction<P>>(sourceName);

                 if (prop != null && prop.Value != null)
                 {
                     var func = prop.Value;
                     func.Invoke(Context, target);
                 }
                 else
                 {
                     if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
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
        /// 注册状态改变事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uEvent"></param>
        /// <param name="sourceName"></param>
        public virtual void RegistEvent<T>(UnityEvent<T> uEvent, string sourceName)
        {
            UnityAction<T> action = (x) =>
            {
                var prop = viewModel.GetBindableProperty<PanelAction<T>>(sourceName);

                if (prop != null && prop.Value != null)
                {
                    var func = prop.Value;
                    func.Invoke(Context, x);
                }
                else
                {
                    if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
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
        /// 注册状态改变事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uEvent"></param>
        /// <param name="sourceName"></param>
        public virtual void RegistEvent<T, S>(UnityEvent<T, S> uEvent, string sourceName)
        {
            UnityAction<T, S> action = (x, y) =>
             {
                 var prop = viewModel.GetBindableProperty<PanelAction<T, S>>(sourceName);

                 if (prop != null && prop.Value != null)
                 {
                     var func = prop.Value;
                     func.Invoke(Context, x, y);
                 }
                 else
                 {
                     if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
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
        /// 注册状态改变事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uEvent"></param>
        /// <param name="sourceName"></param>
        public virtual void RegistEvent<T, S, R>(UnityEvent<T, S, R> uEvent, string sourceName)
        {
            UnityAction<T, S, R> action = (x, y, z) =>
             {
                 var prop = viewModel.GetBindableProperty<PanelAction<T, S, R>>(sourceName);

                 if (prop != null && prop.Value != null)
                 {
                     var func = prop.Value;
                     func.Invoke(Context, x, y, z);
                 }
                 else
                 {
                     if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
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
        /// 注册状态改变事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uEvent"></param>
        /// <param name="sourceName"></param>
        public virtual void RegistEvent<T, S, R, Q>(UnityEvent<T, S, R, Q> uEvent, string sourceName)
        {
            UnityAction<T, S, R, Q> action = (x, y, z, w) =>
             {
                 var prop = viewModel.GetBindableProperty<PanelAction<T, S, R, Q>>(sourceName);

                 if (prop != null && prop.Value != null)
                 {
                     var func = prop.Value;
                     func.Invoke(Context, x, y, z, w);
                 }
                 else
                 {
                     if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
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
        /// 手动指定绑定事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceName"></param>
        /// <param name="onViewModelChanged"></param>
        public void RegistValueChange<T>(UnityAction<T> onViewModelChanged, string sourceName)
        {
            if (onViewModelChanged == null) return;

            binders += (viewModel) =>
            {
                var prop = viewModel.GetBindableProperty<T>(sourceName);

                if (prop != null)
                {
                    onViewModelChanged.Invoke(prop.Value);
                    prop.RegistValueChanged(onViewModelChanged);
                }
                else
                {
                    if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
                }
            };

            unbinders += (viewModel) =>
            {
                var prop = viewModel.GetBindableProperty<T>(sourceName);

                if (prop != null)
                {
                    prop.RemoveValueChanged(onViewModelChanged);
                }
            };
        }
    }
}
