using UnityEngine;
using UnityEditor;
namespace BridgeUI
{
    [CustomEditor(typeof(LuaScript))]
    public class LuaScriptDrawer : Editor
    {
        [MenuItem("Assets/Create/LuaScript",priority = 81,validate =false)]
        static void CreateAnScript()
        {
            var script = ScriptableObject.CreateInstance<LuaScript>();
            ProjectWindowUtil.CreateAsset(script, "new_lua.asset");
        }


    }

}