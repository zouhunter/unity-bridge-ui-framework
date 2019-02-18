using System;
using UnityEngine;
using UnityEngine.Events;

namespace BridgeUI.Binding
{
    public class UIPanelBinder
    {
#if BridgeUI_Log
        public static bool log = true;
#endif
        public IUIPanel Context { get; private set; }
        public IViewModel viewModel { get; private set; }
        private event UnityAction<IViewModel> binders;
        private event UnityAction<IViewModel> unbinders;
        public UIPanelBinder(IUIPanel panel)
        {
            this.Context = panel;
        }

        #region UnityEvent
        /// <summary>
        /// 注册通用事件
        /// </summary>
        /// <param name="uEvent"></param>
        /// <param name="sourceName"></param>
        public virtual void RegistEvent(UnityEvent uEvent, byte sourceName)
        {
            UnityAction action = () =>
            {
                InvokeEvent(sourceName);
            };

            if (viewModel != null)
            {
                uEvent.AddListener(action);
            }
            else
            {
                binders += viewModel =>
                {
                    uEvent.AddListener(action);
                };
            }

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
        public virtual void RegistEvent<T>(UnityEvent uEvent, byte sourceName, T target)
        {
            UnityAction action = () =>
            {
                InvokeEvent(sourceName, target);
            };

            if (viewModel != null)
            {
                uEvent.AddListener(action);
            }
            else
            {
                binders += viewModel =>
                {
                    uEvent.AddListener(action);
                };
            }

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
        public virtual void RegistEvent<T>(UnityEvent<T> uEvent, byte sourceName)
        {
            UnityAction<T> action = (x) =>
            {
                InvokeEvent<T>(sourceName, x);
            };

            if (viewModel != null)
            {
                uEvent.AddListener(action);
            }
            else
            {
                binders += viewModel =>
                {
                    uEvent.AddListener(action);
                };
            }

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
        public virtual void RegistEvent<T, P>(UnityEvent<T> uEvent, byte sourceName, P target)
        {
            UnityAction<T> action = (arg0) =>
            {
                InvokeEvent(sourceName, arg0, target);
            };

            if (viewModel != null)
            {
                uEvent.AddListener(action);
            }
            else
            {
                binders += viewModel =>
                {
                    uEvent.AddListener(action);
                };
            }

            unbinders += viewModel =>
            {
                uEvent.RemoveListener(action);
            };
        }

        #endregion

        #region  触发事件

        #region Bind&UnBind
        public virtual void Bind(IViewModel viewModel)
        {
            //Debug.Log("Bind:" + viewModel);
            this.viewModel = viewModel;

            if (viewModel != null)
            {
                if (binders != null)
                {
                    binders.Invoke(viewModel);
                }

                viewModel.OnAfterBinding(Context);
            }
        }
        public virtual void Unbind()
        {
            //Debug.Log("UnBind:" + viewModel);
            if (viewModel != null)
            {
                viewModel.OnBeforeUnBinding(Context);

                if (unbinders != null)
                {
                    unbinders.Invoke(viewModel);
                }
            }
            this.viewModel = null;
        }
        #endregion

        #region InvokeEvents
        /// <summary>
        /// 触发方法
        /// </summary>
        /// <param name="sourceName"></param>
        public virtual void InvokeEvent(byte sourceName)
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
        public virtual void InvokeEvent<T>(byte sourceName, T arg0)
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
        public virtual void InvokeEvent<S, T>(byte sourceName, S arg0, T arg1)
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
        public virtual void InvokeEvent<R, S, T>(byte sourceName, R arg0, S arg1, T arg2)
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
        public virtual void InvokeEvent<Q, R, S, T>(byte sourceName, Q arg0, R arg1, S arg2, T arg3)
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
        public virtual void InvokeEvent<P, Q, R, S, T>(byte sourceName, P arg0, Q arg1, R arg2, S arg3, T arg4)
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
        #endregion

        #region UnityEvent<T, S>
        /// <summary>
        /// 注册状态改变事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uEvent"></param>
        /// <param name="sourceName"></param>
        public virtual void RegistEvent<T, S>(UnityEvent<T, S> uEvent, byte sourceName)
        {
            UnityAction<T, S> action = (arg0, arg1) =>
            {
                InvokeEvent(sourceName, arg0, arg1);
            };

            if (viewModel != null)
            {
                uEvent.AddListener(action);
            }
            else
            {
                binders += viewModel =>
                {
                    uEvent.AddListener(action);
                };
            }

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
        public virtual void RegistEvent<T, X, P>(UnityEvent<T, X> uEvent, byte sourceName, P target)
        {
            UnityAction<T, X> action = (arg0, arg1) =>
            {
                InvokeEvent(sourceName, arg0, arg1, target);
            };

            if (viewModel != null)
            {
                uEvent.AddListener(action);
            }
            else
            {
                binders += viewModel =>
                {
                    uEvent.AddListener(action);
                };
            }

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
        public virtual void RegistEvent<T, S, R>(UnityEvent<T, S, R> uEvent, byte sourceName)
        {
            UnityAction<T, S, R> action = (x, y, z) =>
            {
                InvokeEvent(sourceName, x, y, z);
            };

            if (viewModel != null)
            {
                uEvent.AddListener(action);
            }
            else
            {
                binders += viewModel =>
                {
                    uEvent.AddListener(action);
                };
            }

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
        public virtual void RegistEvent<T, X, Y, P>(UnityEvent<T, X, Y> uEvent, byte sourceName, P target)
        {
            UnityAction<T, X, Y> action = (x, y, z) =>
            {
                InvokeEvent(sourceName, x, y, z, target);
            };

            if (viewModel != null)
            {
                uEvent.AddListener(action);
            }
            else
            {
                binders += viewModel =>
                {
                    uEvent.AddListener(action);
                };
            }

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
        public virtual void RegistEvent<T, S, R, Q>(UnityEvent<T, S, R, Q> uEvent, byte sourceName)
        {
            UnityAction<T, S, R, Q> action = (x, y, z, w) =>
            {
                InvokeEvent(sourceName, x, y, z, w);
            };

            if (viewModel != null)
            {
                uEvent.AddListener(action);
            }
            else
            {
                binders += viewModel =>
                {
                    uEvent.AddListener(action);
                };
            }

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
        public virtual void RegistEvent<T, X, Y, Z, P>(UnityEvent<T, X, Y, Z> uEvent, byte sourceName, P target)
        {
            UnityAction<T, X, Y, Z> action = (x, y, z, w) =>
            {
                InvokeEvent(sourceName, x, y, z, w, target);
            };

            if (viewModel != null)
            {
                uEvent.AddListener(action);
            }
            else
            {
                binders += viewModel =>
                {
                    uEvent.AddListener(action);
                };
            }

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
        public virtual void RegistValueEvent<T>(UnityEvent<T> uEvent, byte sourceName)
        {
            UnityAction<T> action = (x) =>
            {
                SetValue<T>(x, sourceName);
            };

            if (viewModel != null)
            {
                uEvent.AddListener(action);
            }
            else
            {
                binders += viewModel =>
                {
                    uEvent.AddListener(action);
                };
            }

            unbinders += viewModel =>
            {
                uEvent.RemoveListener(action);
            };
        }
        #endregion

        #region RegistValueChange 
        /// <summary>
        /// 手动指定绑定事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceName"></param>
        /// <param name="onViewModelChanged"></param>
        public virtual void RegistValueChange<T>(UnityAction<T> onViewModelChanged, byte sourceName)
        {
            if (onViewModelChanged == null) return;

            if (viewModel != null)
            {
                var prop = viewModel.GetBindableProperty<T>(sourceName);

                var haveDefult = viewModel.HaveDefultProperty(sourceName);

                if (haveDefult && prop.Value != null)
                {
                    onViewModelChanged.Invoke(prop.Value);
                }

                Debug.Log(sourceName + " porp:" + prop.Value + ":" + prop.GetHashCode());

                prop.RegistValueChanged(onViewModelChanged);
            }
            else
            {
                binders += (viewModel) =>
                {
                    var haveDefult = viewModel.HaveDefultProperty(sourceName);

                    var prop = viewModel.GetBindableProperty<T>(sourceName);

                    if (haveDefult && prop.Value != null)
                    {
                        onViewModelChanged.Invoke(prop.Value);
                    }

                    Debug.Log(sourceName + " porp:" + prop.Value + ":" + prop.GetHashCode());

                    prop.RegistValueChanged(onViewModelChanged);
                };
            }

            unbinders += (viewModel) =>
            {
                var prop = viewModel.GetBindableProperty<T>(sourceName);

                prop.RemoveValueChanged(onViewModelChanged);
            };
        }
        #endregion

        #region SetValue
        /// <summary>
        /// 设置viewModel中变量的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="sourceName"></param>
        public virtual void SetValue<T>(T value, byte sourceName)
        {
            if (viewModel != null)
            {
                var prop = viewModel.GetBindableProperty<T>(sourceName);
                prop.Value = value;
            }
            else
            {
                binders += (viewModel) =>
                {
                    var prop = viewModel.GetBindableProperty<T>(sourceName);
                    prop.Value = value;
                };
            }
        }
        /// <summary>
        /// 设置viewModel中变量的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="sourceName"></param>
        public virtual void SetBoxValue(object value, byte sourceName)
        {
            if (viewModel != null)
            {
                var prop = viewModel.GetBindableProperty(sourceName, value.GetType());
                if (prop != null)
                {
                    prop.ValueBoxed = value;
                }
            }
            else
            {
                binders += (viewModel) =>
                {
                    var prop = viewModel.GetBindableProperty(sourceName, value.GetType());
                    if (prop != null)
                    {
                        prop.ValueBoxed = value;
                    }
                };
            }
        }
        #endregion
    }
}
