#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-04-24 02:40:55
    * 说    明：       
* ************************************************************************************/
#endregion
using UnityEngine;

namespace BridgeUI
{
    /// <summary>
    /// 类
    /// <summary>
    public class LuaScript:ScriptableObject
    {
        public string luaScriptText { get { return System.Text.Encoding.UTF8.GetString(bytes); } }
        public byte[] bytes;
    }

}