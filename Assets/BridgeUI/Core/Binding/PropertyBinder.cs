using UnityEngine;
using UnityEngine.Events;

namespace BridgeUI.Binding
{
    public class PropertyBinder
    {
#if BridgeUI_Log
        public static bool log = false;
#endif

        public IBindingContext Context { get; private set; }
        public IViewModel viewModel { get; private set; }
        protected event UnityAction<IViewModel> binders;
        protected event UnityAction<IViewModel> unbinders;
        public PropertyBinder(IBindingContext context)
        {
            this.Context = context;
        }

        #region Bind&UnBind
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
        #endregion

        #region InvokeEvents
        /// <summary>
        /// 触发方法
        /// </summary>
        /// <param name="sourceName"></param>
        public virtual void InvokeEvent(string sourceName)
        {
            if (viewModel == null) return;

            var prop = viewModel.GetBindableProperty<PanelAction>(sourceName);
            if (prop != null && prop.Value != null)
            {
                var func = prop.Value;
                func.Invoke(Context);
            }
            else
            {
#if BridgeUI_Log
                if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
#endif
            }
        }
        /// <summary>
        /// 触发方法
        /// </summary>
        /// <param name="sourceName"></param>
        public virtual void InvokeEvent<T>(string sourceName, T arg0)
        {
            if (viewModel == null) return;

            var prop = viewModel.GetBindableProperty<PanelAction<T>>(sourceName);
            if (prop != null && prop.Value != null)
            {
                var func = prop.Value;
                func.Invoke(Context, arg0);
            }
            else
            {
#if BridgeUI_Log
                if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
#endif
            }
        }
        /// <summary>
        /// 触发方法
        /// </summary>
        /// <param name="sourceName"></param>
        public virtual void InvokeEvent<S, T>(string sourceName, S arg0, T arg1)
        {
            if (viewModel == null) return;

            var prop = viewModel.GetBindableProperty<PanelAction<S, T>>(sourceName);
            if (prop != null && prop.Value != null)
            {
                var func = prop.Value;
                func.Invoke(Context, arg0, arg1);
            }
            else
            {
#if BridgeUI_Log
                if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
#endif
            }
        }
        /// <summary>
        /// 触发方法
        /// </summary>
        /// <param name="sourceName"></param>
        public virtual void InvokeEvent<R, S, T>(string sourceName, R arg0, S arg1, T arg2)
        {
            if (viewModel == null) return;

            var prop = viewModel.GetBindableProperty<PanelAction<R, S, T>>(sourceName);
            if (prop != null && prop.Value != null)
            {
                var func = prop.Value;
                func.Invoke(Context, arg0, arg1, arg2);
            }
            else
            {
#if BridgeUI_Log
                if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
#endif
            }
        }
        /// <summary>
        /// 触发方法
        /// </summary>
        /// <param name="sourceName"></param>
        public virtual void InvokeEvent<Q, R, S, T>(string sourceName, Q arg0, R arg1, S arg2, T arg3)
        {
            if (viewModel == null) return;

            var prop = viewModel.GetBindableProperty<PanelAction<Q, R, S, T>>(sourceName);
            if (prop != null && prop.Value != null)
            {
                var func = prop.Value;
                func.Invoke(Context, arg0, arg1, arg2, arg3);
            }
            else
            {
#if BridgeUI_Log
                if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
#endif
            }
        }
        /// <summary>
        /// 触发方法
        /// </summary>
        /// <param name="sourceName"></param>
        public virtual void InvokeEvent<P, Q, R, S, T>(string sourceName, P arg0, Q arg1, R arg2, S arg3, T arg4)
        {
            if (viewModel == null) return;

            var prop = viewModel.GetBindableProperty<PanelAction<P, Q, R, S, T>>(sourceName);
            if (prop != null && prop.Value != null)
            {
                var func = prop.Value;
                func.Invoke(Context, arg0, arg1, arg2, arg3, arg4);
            }
            else
            {
#if BridgeUI_Log
                if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, viewModel);
#endif
            }
        }
        #endregion

        #region UnityEvent
        /// <summary>
        /// 注册通用事件
        /// </summary>
        /// <param name="uEvent"></param>
        /// <param name="sourceName"></param>
        public virtual void RegistEvent(UnityEvent uEvent, string sourceName)
        {
            UnityAction action = () =>
            {
                InvokeEvent(sourceName);
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
                InvokeEvent(sourceName, target);
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
        #endregion

        #region UnityEvent<T>

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
                InvokeEvent<T>(sourceName, x);
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
            UnityAction<T> action = (arg0) =>
            {
                InvokeEvent(sourceName, arg0, target);
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

        #endregion

        #region UnityEvent<T, S>
        /// <summary>
        /// 注册状态改变事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uEvent"></param>
        /// <param name="sourceName"></param>
        public virtual void RegistEvent<T, S>(UnityEvent<T, S> uEvent, string sourceName)
        {
            UnityAction<T, S> action = (arg0, arg1) =>
            {
                InvokeEvent(sourceName, arg0, arg1);
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
            UnityAction<T, X> action = (arg0, arg1) =>
            {
                InvokeEvent(sourceName, arg0, arg1, target);
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

        #endregion

        #region UnityEvent<T, X, Y>

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
                InvokeEvent(sourceName, x, y, z);
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
                 InvokeEvent(sourceName, x, y, z, target);
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

        #endregion

        #region UnityEvent<T, X, Y, Z>

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
                InvokeEvent(sourceName, x, y, z, w);
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
                 InvokeEvent(sourceName, x, y, z, w, target);
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
        #endregion

        #region RegistValueEvent
        /// <summary>
        /// 注册状态改变事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uEvent"></param>
        /// <param name="sourceName"></param>
        public virtual void RegistValueEvent<T>(UnityEvent<T> uEvent, string sourceName)
        {
            UnityAction<T> action = (x) =>
            {
                SetValue<T>(x, sourceName);
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
        #endregion

        #region ValueRegist&Set
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

                onViewModelChanged.Invoke(prop.Value);

                prop.RegistValueChanged(onViewModelChanged);
            };

            unbinders += (viewModel) =>
            {
                var prop = viewModel.GetBindableProperty<T>(sourceName);

                prop.RemoveValueChanged(onViewModelChanged);
            };
        }

        /// <summary>
        /// 设置viewModel中变量的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="sourceName"></param>
        public virtual void SetValue<T>(T value, string sourceName)
        {
            if (viewModel != null)
            {
                var prop = viewModel.GetBindableProperty<T>(sourceName);

                prop.Value = value;
            }
        }
        /// <summary>
        /// 设置viewModel中变量的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="sourceName"></param>
        public virtual void SetBoxValue(object value, string sourceName)
        {
            if (viewModel != null)
            {
                var prop = viewModel.GetBindableProperty(sourceName, value.GetType());
                if (prop != null)
                {
                    prop.ValueBoxed = value;
                }
            }
        }
        #endregion
    }
}
