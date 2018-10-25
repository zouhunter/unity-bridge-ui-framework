#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-04-29 10:47:28
    * 说    明：       
* ************************************************************************************/
#endregion
using System;
using UnityEngine;
using BridgeUI.Binding;
using UnityEngine.Events;

#if xLua
using XLua;
namespace BridgeUI.Common
{
    /// <summary>
    /// 类
    /// <summary>
    public class LuaScriptModel : Binding.ViewModel
    {
        [SerializeField]
        protected string luaScript;
        public string luaScriptText { get { return luaScript; } }

        protected LuaTable scriptEnv;
        internal static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
        protected bool inited;
        public const string on_binding_func_name = "on_binding";
        public const string on_unbinding_func_name = "on_unbinding";
        internal static float lastGCTime = 0;
        internal const float GCInterval = 1;//1 second 

        private void OnEnable()
        {
            inited = false;
        }

        public override void OnBinding(IBindingContext context)
        {
            base.OnBinding(context);
            TryInit();
            TriggerLuaFunc(on_binding_func_name,context);
        }
        public override void OnUnBinding(IBindingContext context)
        {
            base.OnUnBinding(context);
            TriggerLuaFunc(on_unbinding_func_name, context);
        }

        protected void TryInit()
        {
            if (inited)
            {
                return;
            }
            else
            {
                inited = true;
            }

            var text = luaScript;

            if (string.IsNullOrEmpty(text)) return;

            scriptEnv = luaEnv.NewTable();

            LuaTable meta = luaEnv.NewTable();
            meta.Set("__index", luaEnv.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            scriptEnv.Set("self", this);
            luaEnv.DoString(text, name, scriptEnv);
        }
        
        protected void Update()
        {
            if (Time.time - lastGCTime > GCInterval)
            {
                luaEnv.Tick();
                lastGCTime = Time.time;
            }
        }

        protected void TriggerLuaFunc(string sourceName,IBindingContext context)
        {
            var prop = GetBindableProperty<PanelAction>(sourceName);
            if (prop != null && prop.Value != null)
            {
                var func = prop.Value;
                func.Invoke(context);
            }
            else
            {
                if (log) Debug.LogWarningFormat("target prop of {0} not exist in {1}", sourceName, this);
            }
        }

        [LuaCallCSharp]
        public void SetValue(string key, object value)
        {
            var prop = GetBindablePropertySelfty(key, value.GetType());
            if (prop != null)
            {
                prop.ValueBoxed = value;
            }
        }

        public override BindableProperty<T> GetBindableProperty<T>(string name)
        {
            var prop = base.GetBindablePropertySelfty<T>(name);
            if (prop.ValueBoxed == null && scriptEnv !=null)
            {
                prop.Value = scriptEnv.Get<T>(name);
            }
            return prop;
        }
    }
}
#endif