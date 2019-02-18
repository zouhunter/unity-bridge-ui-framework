using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeUI.Extend.XLua
{
    public abstract class LuaPanel_Reference : MonoBehaviour, IUIPanelReference
    {
        [HideInInspector]
        public ResourceType resourceType;
        [HideInInspector]
        public TextAsset luaScript;
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
        [HideInInspector]
        public Model.BundleLoader bundleLoader;

        public Action onUpdate { get; internal set; }

        protected virtual void Awake()
        {
            if (bundleLoader)
                bundleLoader.InitEnviroment();
        }
        protected virtual void Update()
        {
            if (onUpdate != null) onUpdate();
        }
        public abstract Type CetPanelScriptType();
    }
}