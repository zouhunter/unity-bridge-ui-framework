//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using LuaView;
//using UnityEditor;
//using UnityEditor.Experimental.AssetImporters;
//using BridgeUI;
//using BridgeUI.Model;
//using BridgeUI.Binding;

//namespace BridgeUIEditor {

//    [ScriptedImporter(1, "vm")]
//    public class ViewModelImporter : ScriptedImporter
//    {
//        [SerializeField] private string lua_pic_guid = "7992a79125a570d488a0092d05c991b1";

//        public override void OnImportAsset(AssetImportContext ctx)
//        {
//            var path = ctx.assetPath;
//            var json = System.IO.File.ReadAllText(path);
//            if(!string.IsNullOrEmpty(json))
//            {
//                var info = JsonUtility.FromJson<ViewModelInfo>(json);
//                if(info != null)
//                {
//                    var viewModel = ScriptableObject.CreateInstance(info.type);
//                    if(viewModel != null && viewModel is ViewModel)
//                    {
//                        JsonUtility.FromJsonOverwrite(json, viewModel);
//                        LoadViewModel(ctx,viewModel as ViewModel);
//                    }
//                }
//            }
//        }

//        private void LoadViewModel(AssetImportContext ctx, ViewModel vm)
//        {
//            ctx.AddObjectToAsset("viewmodel", vm, GetThumbnail());
//            ctx.SetMainObject(vm);
//        }

//        private Texture2D GetThumbnail()
//        {
//            var path = AssetDatabase.GUIDToAssetPath(lua_pic_guid);
//            if (!string.IsNullOrEmpty(path))
//            {
//                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
//                return texture;
//            }
//            else
//            {
//                return null;
//            }
//        }
//    } }