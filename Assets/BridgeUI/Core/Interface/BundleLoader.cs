using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using BridgeUI.Model;

namespace BridgeUI.Model
{
    public class BundleLoaderAttribute : PropertyAttribute {
        public string title;
        public BundleLoaderAttribute(string title)
        {
            this.title = title;
        }
    }

    public abstract class BundleLoader:ScriptableObject
    {
        public abstract void InitEnviroment();
        public abstract void LoadAssetAsync<T>(string assetBundleName,string assetName, UnityAction<T> onLoad) where T : UnityEngine.Object;
    }
}