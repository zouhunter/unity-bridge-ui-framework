#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-04-29 10:56:29
    * 说    明：       
* ************************************************************************************/
#endregion
namespace BridgeUI.Common
{
    /// <summary>
    /// 
    /// <summary>
    public enum LuaResourceType
    {
        OriginLink,
        StreamingFile,
        WebFile,
#if AssetBundleTools
        AssetBundle,
#endif
        Resource,
        ScriptObject,
        RuntimeString,
    }
}
