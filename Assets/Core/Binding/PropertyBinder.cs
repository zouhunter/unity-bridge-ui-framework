using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace BridgeUI.Binding
{
    public enum Direction
    {
        ViewToModel,
        ModelToView,
        Bidirection
    }

    public class PropertyBinder
    {
        public IPropertyChanged Context { get; private set; }
        public ViewModelBase viewModel { get; private set; }

        protected event UnityAction<ViewModelBase> binders;
        protected event UnityAction<ViewModelBase> unbinders;
        protected readonly Dictionary<string, IBindableProperty> bindingPropertyDic = new Dictionary<string, BridgeUI.Binding.IBindableProperty>();
        public IBindableProperty this[string name]
        {
            get
            {
                if (bindingPropertyDic.ContainsKey(name))
                {
                    return bindingPropertyDic[name];
                }
                else
                {
                    return null;
                }
            }
            private set
            {
                bindingPropertyDic[name] = value;
            }
        }
        public BindableProperty<T> GetBindableProperty<T>(string name)
        {
            if (this[name] == null || !(this[name] is BindableProperty<T>))
            {
                this[name] = new BindableProperty<T>();
            }
            return this[name] as BindableProperty<T>;
        }

        public PropertyBinder(IPropertyChanged context)
        {
            this.Context = context;
            context.onPropertyChanged += OnContextPropertyChanged;
        }

        private void OnContextPropertyChanged(string propertyName)
        {
            var prop = this[propertyName];
            if (prop != null)
            {
                prop.Notify();
            }
        }

        public void Bind(ViewModelBase viewModel)
        {
            Debug.Log("Bind:" + viewModel);
            this.viewModel = viewModel;
            
            if (viewModel != null && binders != null)
                binders.Invoke(viewModel);
        }

        public void Unbind()
        {
            Debug.Log("UnBind:" + viewModel);
            if (viewModel != null && unbinders != null){
                unbinders.Invoke(viewModel);
            }
            this.viewModel = null;
        }
        //public void AddValue(string target, string sourceName, Direction direction = Direction.Bidirection)
        //{
        //    AddValue<object>(sourceName, target, direction);
        //}
        public void AddValue<T>(string target, string sourceName, Direction direction = Direction.Bidirection)
        {
            if (direction == Direction.ModelToView || direction == Direction.Bidirection)
            {
                object root = Context;
                var member = GetDeepMember(ref root, target);
                UnityAction<T> onViewModelChanged = (value) =>
                {
                    Debug.Log(value);
                    Set<T>(root, member, value);
                };
                AddToModel(sourceName, onViewModelChanged);
            }

            if (direction == Direction.ViewToModel || direction == Direction.Bidirection)
            {
                UnityAction<T> onViewChanged = (value) =>
                {
                    if (viewModel != null)
                    {
                        viewModel.GetBindableProperty<T>(sourceName).Value = value;
                        UnityEngine.Debug.Log(viewModel);
                    }
                };
                var prop = GetBindableProperty<T>(target);
                prop.RegistValueChanged(onViewChanged);
            }
        }


        public void AddToModel<T>(string sourceName, UnityAction<T> onViewModelChanged)
        {
            binders += (viewModel) =>
            {
                var prop = viewModel.GetBindableProperty<T>(sourceName);
                if (prop != null)
                {
                    onViewModelChanged.Invoke(prop.Value);
                    prop.RegistValueChanged(onViewModelChanged);
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
        /// Get Member Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberName"></param>
        /// <returns></returns>
        protected static T Get<T>(object Instance, MemberInfo temp)
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
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        protected static void Set<T>(object Instance, MemberInfo temp, T value)
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
                UnityEngine.Debug.Log(value);

                (temp as MethodInfo).Invoke(Instance, new object[] { value });
            }
        }

        /// <summary>
        /// Invoke Method Value
        /// </summary>
        /// <param name="memberName"></param>
        protected static void Invoke(object Instance, string memberName, params object[] value)
        {
            Debug.Log(Instance + ":" + memberName);
            var temps = Instance.GetType().GetMember(memberName);
            if(temps.Length > 0)
            {
                var temp = temps[0];
                if (temp is MethodInfo)
                {
                    (temp as MethodInfo).Invoke(Instance, value);
                }
            }
        }


        private static MemberInfo GetDeepMember(ref object Instance, string memberName)
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
