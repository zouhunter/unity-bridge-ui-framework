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
        /// 利用反射自动完成绑定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberPath"></param>
        /// <param name="sourceName"></param>
        public void RegistMember<T>(string memberPath, string sourceName)
        {
            object root = Context;
            var member = GetDeepMember(ref root, memberPath);
            if (root == null)
            {
                if (log) Debug.LogWarning("ignore:" + memberPath + "(because some component is null!)");
            }
            else
            {
                UnityAction<T> onViewModelChanged = (value) =>
                {
                    SetMemberValue<T>(root, member, value);
                };
                RegistMember(onViewModelChanged, sourceName);
            }

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
        public void RegistMember<T>(UnityAction<T> onViewModelChanged, string sourceName)
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

        /// <summary>
        ///  Get Member Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Instance"></param>
        /// <param name="temp"></param>
        /// <returns></returns>
        protected static T GetMemberValue<T>(object Instance, MemberInfo temp)
        {
            if (temp == null)
            {
                return default(T);
            }

            if (temp is FieldInfo)
            {
                return (T)(temp as FieldInfo).GetValue(Instance);
            }
            else if (temp is PropertyInfo)
            {
                return (T)(temp as PropertyInfo).GetValue(Instance, null);
            }
            else
            {
                return (T)(temp as MethodInfo).Invoke(Instance, null);
            }
        }

        /// <summary>
        /// Set Member Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Instance"></param>
        /// <param name="temp"></param>
        /// <param name="value"></param>
        protected static void SetMemberValue<T>(object Instance, MemberInfo temp, T value)
        {
            if (temp == null)
            {
                return;
            }

            if (temp is FieldInfo)
            {
                (temp as FieldInfo).SetValue(Instance, value);
            }
            else if (temp is PropertyInfo)
            {
                (temp as PropertyInfo).SetValue(Instance, value, null);
            }
            else
            {
                (temp as MethodInfo).Invoke(Instance, new object[] { value });
            }
        }

        /// <summary>
        /// Invoke Method Value
        /// </summary>
        /// <param name="memberName"></param>
        protected static void InvokeMethod(object Instance, string memberName, params object[] value)
        {
            if (log) Debug.Log(Instance + ":" + memberName);
            var temps = Instance.GetType().GetMember(memberName);
            if (temps.Length > 0)
            {
                var temp = temps[0];
                if (temp is MethodInfo)
                {
                    (temp as MethodInfo).Invoke(Instance, value);
                }
            }
        }

        /// <summary>
        /// 深度获取对象
        /// </summary>
        /// <param name="Instance"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        protected static MemberInfo GetDeepMember(ref object Instance, string memberName)
        {
            var names = memberName.Split(new char[] { '.' });
            Type type = Instance.GetType();
            MemberInfo member = null;
            for (int i = 0; i < names.Length; i++)
            {
                var members = type.GetMember(names[i], BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (members == null || members.Length == 0)
                {
                    return null;
                }
                else
                {
                    member = members[0];

                    if (member is FieldInfo)
                    {
                        var fieldInfo = (member as FieldInfo);
                        type = fieldInfo.FieldType;

                        if (i < names.Length - 1)
                        {
                            Instance = fieldInfo.GetValue(Instance);
                        }
                    }
                    else if (member is PropertyInfo)
                    {
                        var propertyInfo = (member as PropertyInfo);
                        type = propertyInfo.PropertyType;

                        if (i < names.Length - 1)
                        {
                            Instance = propertyInfo.GetValue(Instance, null);
                        }
                    }
                }
            }
            return member;
        }
    }
}
