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
using System.Collections.Generic;

using XLua;
namespace BridgeUI.Extend.XLua
{
    /// <summary>
    /// 类
    /// <summary>
    public class LuaViewModel : Binding.ViewModel
    {
        protected LuaTable luaTable;
        protected Dictionary<byte, string> propDic;

        public void Init(LuaTable table,Dictionary<byte,string> propDic)
        {
            this.luaTable = table;
            this.propDic = propDic;
        }

        public override BindableProperty<T> GetBindableProperty<T>(byte id)
        {
            var prop = base.GetBindableProperty<T>(id);
            if (prop.ValueBoxed == null)
            {
                if(propDic != null && propDic.ContainsKey(id))
                {
                    var propertyName = propDic[id];
                    prop.Value = luaTable.Get<string, T>(propertyName);///未实现
                    Debug.Log(propertyName+ ": "+typeof(T).FullName + ":" + prop.Value);
                }
            }
            return prop;
        }
    }
}
