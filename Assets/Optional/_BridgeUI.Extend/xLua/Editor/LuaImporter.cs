using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
namespace LuaView
{
    [ScriptedImporter(1, "lua")]
    public class LuaImporter : ScriptedImporter
    {
        [SerializeField] private string textValue;
        [SerializeField] private string lua_pic_guid = "7992a79125a570d488a0092d05c991b1";

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var path = ctx.assetPath;
            textValue = System.IO.File.ReadAllText(path);
            var lua = new LuaScript(textValue);
            var txt = new TextAsset(textValue);
            ctx.AddObjectToAsset("lua", lua, GetThumbnail());
            ctx.AddObjectToAsset("txt", txt);
            ctx.SetMainObject(lua);
        }

        private Texture2D GetThumbnail()
        {
            var path = AssetDatabase.GUIDToAssetPath(lua_pic_guid);
            if(!string.IsNullOrEmpty(path)){
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                return texture;
            }
            else
            {
                return null;
            }
        }
    }
}
