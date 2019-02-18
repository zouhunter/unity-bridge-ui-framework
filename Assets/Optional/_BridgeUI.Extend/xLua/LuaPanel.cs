#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-04-29 11:02:49
    * 说    明：       
* ************************************************************************************/
#endregion
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using XLua;
using BridgeUI.Binding;
using BridgeUI;

namespace BridgeUI.Extend.XLua
{
    [LuaCallCSharp]
    [BridgeUI.Attributes.PanelParent]
    public abstract class LuaPanel : BindingViewBase
    {
        internal static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
        internal static float lastGCTime = 0;
        internal const float GCInterval = 1;//1 second 

        protected const byte luaOnInit = 255;
        protected const byte luaUpdate = 254;
        protected const byte luaOnRecover = 253;
        protected const byte luaHandleData = 252;

        protected LuaViewModel luaViewModel { get { return VM as LuaViewModel; } }
        private LuaTable tableCreated;
        public override UIPanelBinder Binder
        {
            get
            {
                if (_binder == null)
                {
                    _binder = new LuaPropertyBinder(this);
                }
                return _binder;
            }
        }
        private Dictionary<byte, string> keywordDic;

        private LuaPanel_Reference luaPanelReference;

        private ResourceType resourceType
        {
            get
            {
                return luaPanelReference.resourceType;
            }
        }
        private string scriptName
        {
            get { return luaPanelReference.scriptName; }
        }
        private TextAsset luaScript
        {
            get
            {
                return luaPanelReference.luaScript;
            }
        }
        private Model.BundleLoader bundleLoader
        {
            get
            {
                return luaPanelReference.bundleLoader;
            }
        }
        private string assetBundleName
        {
            get
            {
                return luaPanelReference.assetBundleName;
            }
        }
        private string assetName
        {
            get
            {
                return luaPanelReference.assetName;
            }
        }


        protected override void OnInitialize()
        {
            base.OnInitialize();
            keywordDic = CreatePropertyDic();
            LoadLuaScriptOnAwake();
        }

        protected abstract Dictionary<byte,string> CreatePropertyDic();

        protected override void OnRelease()
        {
            base.OnRelease();
            Binder.InvokeEvent(luaOnRecover);
        }

        protected override void OnBinding(GameObject target)
        {
            base.OnBinding(target);
            luaPanelReference = target.GetComponent<LuaPanel_Reference>();
            luaPanelReference.onUpdate = Update;
        }

        protected virtual void Update()
        {
            Binder.InvokeEvent(luaUpdate);
            if (Time.time - lastGCTime > GCInterval)
            {
                luaEnv.Tick();
                lastGCTime = Time.time;
            }
        }

        private void LoadLuaScriptOnAwake()
        {
            ClearLoadedTable();
            string url = "";
            switch (resourceType)
            {
                case ResourceType.OriginLink:
                    InitScritEnv(luaScript.text);
                    break;
                //                case ResourceType.StreamingFile:
                //                    url =
                //#if UNITY_EDITOR || UNITY_STANDALONE
                //                    "file:///" +
                //#endif
                //                    Application.streamingAssetsPath + "/" + streamingPath;
                //                    StartCoroutine(LoadScriptFromUrl(url));
                //                    break;
                //                case ResourceType.WebFile:
                //                    StartCoroutine(LoadScriptFromUrl(webUrl));
                //                    break;
                case ResourceType.AssetBundle:
                    LoadScriptFromBundle();
                    break;
                case ResourceType.Resource:
                    LoadScriptFromResource();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 从路径下载script
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private IEnumerator LoadScriptFromUrl(string url)
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.error == null)
            {
                var scriptText = WorpScriptString(www.text);
                InitScritEnv(scriptText);
            }
        }
        /// <summary>
        /// 从assetbundle中加载脚本
        /// </summary>
        private void LoadScriptFromBundle()
        {
            if (bundleLoader)
                bundleLoader.LoadAssetAsync<TextAsset>(assetBundleName, assetName, (asset) =>
                {
                    if (asset != null)
                    {
                        InitScritEnv(asset.text);
                    }
                });
        }
        /// <summary>
        /// 从Resource路径加载脚本
        /// </summary>
        private void LoadScriptFromResource()
        {
            var asset = Resources.Load<TextAsset>(scriptName);
            if (asset != null)
            {
                InitScritEnv(asset.text);
            }
        }
        private void ClearLoadedTable()
        {
            if (tableCreated != null)
            {
                tableCreated.Dispose();
                tableCreated = null;
            }
        }

        [LuaCallCSharp]//注意使用Lua中的id对应！！！
        public void SetValue(byte byteID, object value)
        {
            Binder.SetBoxValue(value, byteID);
        }

        private void InitScritEnv(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            tableCreated = luaEnv.NewTable();

            LuaTable meta = luaEnv.NewTable();
            meta.Set("__index", luaEnv.Global);
            tableCreated.SetMetaTable(meta);
            meta.Dispose();

            tableCreated.Set("self", this);
            luaEnv.DoString(text, Name, tableCreated);

            var model = new LuaViewModel();
            model.Init(this.tableCreated, keywordDic);
            VM = model;
            Binder.InvokeEvent(luaOnInit);
        }

        /// <summary>
        /// 直接加载脚本文件不太安全，
        /// 可以进行解密处理
        /// </summary>
        /// <param name="orignal"></param>
        /// <returns></returns>
        protected virtual string WorpScriptString(string orignal)
        {
            return orignal;
        }

        protected override void HandleData(object data)
        {
            base.HandleData(data);

            if (resourceType == ResourceType.RuntimeString && data is string)
            {
                InitScritEnv(data as string);
            }

            if (luaViewModel != null)
            {
                var action = luaViewModel.GetBindableProperty<UnityAction<object>>(luaHandleData);
                if (action.Value != null)
                {
                    action.Value.Invoke(data);
                }
            }
        }
    }

}
