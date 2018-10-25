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
#if xLua
using XLua;
using BridgeUI.Binding;

namespace BridgeUI.Common
{
    [LuaCallCSharp]
    [Attributes.PanelParent]
    public class LuaPanel : PanelBase
    {
        /// <summary>
        /// 
        /// <summary>
        public enum ResourceType
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
        [HideInInspector]
        public ResourceType resourceType;
        [HideInInspector]
        public TextAsset luaScript;
        [HideInInspector]
        public LuaScriptModel scriptObj;
        [HideInInspector]
        public string streamingPath;
        [HideInInspector]
        public string webUrl;
        [HideInInspector]
        public string assetBundleName;
        [HideInInspector]
        public string assetName;
        [HideInInspector]
        public string menu;
        [HideInInspector]
        public string scriptName;

        internal static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
        internal static float lastGCTime = 0;
        internal const float GCInterval = 1;//1 second 
        protected UnityEvent luaOnInit = new UnityEvent();
        protected UnityEvent luaUpdate = new UnityEvent();
        protected UnityEvent luaOnDestroy = new UnityEvent();

        protected LuaViewModel luaViewModel { get { return ViewModel as LuaViewModel; }  }
        private LuaTable tableCreated;

        protected override void Awake()
        {
            base.Awake();
            RegistBaseAction();
        }

        protected override void Start()
        {
            base.Start();
            LoadLuaScriptOnAwake();
        }

        protected virtual void Update()
        {
            luaUpdate.Invoke();
            if (Time.time - lastGCTime > GCInterval)
            {
                luaEnv.Tick();
                lastGCTime = Time.time;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            luaOnDestroy.Invoke();
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
                case ResourceType.StreamingFile:
                    url =
#if UNITY_EDITOR || UNITY_STANDALONE
                    "file:///" +
#endif
                    Application.streamingAssetsPath + "/" + streamingPath;
                    StartCoroutine(LoadScriptFromUrl(url));
                    break;
                case ResourceType.WebFile:
                    StartCoroutine(LoadScriptFromUrl(webUrl));
                    break;
#if AssetBundleTools
                case ResourceType.AssetBundle:
                    LoadScriptFromBundle();
                    break;
#endif
                case ResourceType.Resource:
                    LoadScriptFromResource();
                    break;
                case ResourceType.ScriptObject:
                    InitScritEnv(scriptObj.luaScriptText);
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
#if AssetBundleTools
        /// <summary>
        /// 从assetbundle中加载脚本
        /// </summary>
        private void LoadScriptFromBundle()
        {
            AssetBundleLoader loader = null;
            if (string.IsNullOrEmpty(menu))
            {
                loader = AssetBundleLoader.Instence;
            }
            else
            {
                string url =
#if UNITY_EDITOR || UNITY_STANDALONE
                    "file:///" +
#endif
                    Application.streamingAssetsPath + "/" + menu;

                loader = AssetBundleLoader.GetInstance(url, menu);
            }
            loader.LoadAssetFromUrlAsync<TextAsset>(assetBundleName, assetName, (asset) =>
            {
                if (asset != null)
                {
                    InitScritEnv(asset.text);
                }
            });
        }
#endif
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

        [LuaCallCSharp]
        public void SetValue(string key, object value)
        {
            if (luaViewModel != null)
            {
                var prop = luaViewModel.GetBindablePropertySelfty(key, value.GetType());
                if (prop != null)
                {
                    prop.ValueBoxed = value;
                }
            }
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
            luaEnv.DoString(text, name, tableCreated);

            var model = LuaViewModel.CreateInstance<LuaViewModel>();
            model.Init(this.tableCreated);
            ViewModel = model;
            luaOnInit.Invoke();
        }

        private void RegistBaseAction()
        {
            Binder.RegistEvent(luaOnInit, "oninit");
            Binder.RegistEvent(luaUpdate, "update");
            Binder.RegistEvent(luaOnDestroy, "ondestroy");
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
                var action = luaViewModel.GetBindablePropertySelfty<UnityAction<object>>("handle_data");
                if (action.Value != null)
                {
                    action.Value.Invoke(data);
                }
            }
        }
    }

}
#endif
