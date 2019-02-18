using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BridgeUI.Binding
{
    public class BindingReference : UIBehaviour, IUIPanelReference
    {
        public Type CetPanelScriptType()
        {
            var typeName = string.Format("{0}.{1}", Setting.defultNameSpace, name);
            var viewScriptType = typeof(IUIPanelReference).Assembly.GetType(typeName);
            if (viewScriptType != null)
            {
                return viewScriptType;
            }
            Debug.LogError("未编写脚本：" + typeName);
            return null;
        }
    }
    public class BindingReference<T> : BindingReference where T : struct
    {
        [SerializeField]
        protected T m_data;
    }
}