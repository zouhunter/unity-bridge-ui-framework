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

using XLua;
namespace BridgeUI.Extend.XLua
{
    /// <summary>
    /// 类
    /// <summary>
    public class LuaViewModel : Binding.ViewModel
    {
        protected LuaTable scriptEnv;

        public void Init(LuaTable scriptEnv)
        {
            this.scriptEnv = scriptEnv;
        }

        public override BindableProperty<T> GetBindableProperty<T>(string name)
        {
            var prop = base.GetBindableProperty<T>(name);
            if (prop.ValueBoxed == null)
            {
                prop.Value = scriptEnv.Get<T>(name);
            }
            return prop;
        }
    }
}
