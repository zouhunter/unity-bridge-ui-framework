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
using System.Linq;
using System;
using System.Reflection;


namespace BridgeUI.Binding
{
    public class ViewModelContainer : ScriptableObject, IViewModel
    {
        [SerializeField,HideInInspector]
        private List<ViewModel> instences = new List<ViewModel>();

        public ViewModel instence
        {
            get
            {
                if(instences.Count == 0)
                {
                    instences.Add(new ViewModel());
                }
                return instences[0];
            }
        }

        public bool ContainsKey(string key)
        {
            return instence.ContainsKey(key);
        }

        public BindableProperty<T> GetBindableProperty<T>(string keyward)
        {
            return instence.GetBindableProperty<T>(keyward);
        }

        public IBindableProperty GetBindablePropertySelfty(string keyward, Type type)
        {
            return instence.GetBindablePropertySelfty(keyward, type);
        }

        public BindableProperty<T> GetBindablePropertySelfty<T>(string keyward)
        {
            return instence.GetBindablePropertySelfty<T>(keyward);
        }

        public void OnBinding(IBindingContext context)
        {
            instence.OnBinding(context);
        }

        public void OnUnBinding(IBindingContext context)
        {
            instence.OnUnBinding(context);
        }

        public void SetBindableProperty(string keyward, IBindableProperty value)
        {
            instence.SetBindableProperty(keyward,value);
        }
    }
}