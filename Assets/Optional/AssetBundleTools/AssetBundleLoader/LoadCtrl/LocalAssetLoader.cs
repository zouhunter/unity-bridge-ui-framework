using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace AssetBundles
{
    public interface ILocalAssetLoader
    {
        void UpdateDownLand();
        T LoadAsset<T>(string filePath, string assetName) where T : UnityEngine.Object;
        void LoadAssetAsync<T>(string filePath, string assetName, UnityAction<T> OnAssetLoad) where T : UnityEngine.Object;
        AsyncOperation LoadLevelAsync(string filePath, string assetName, UnityEngine.SceneManagement.LoadSceneMode loadModle);
        void UnLoadAssetBundle(string filePath);
    }
    public class LocalAssetLoader : ILocalAssetLoader
    {
        private Dictionary<string, AssetBundleCreateRequest> requests = new Dictionary<string, AssetBundleCreateRequest>();
        private Dictionary<AssetBundleCreateRequest, UnityAction<AssetBundle>> m_InProgressOperations = new Dictionary<AssetBundleCreateRequest, UnityAction<AssetBundle>>();
        private List<AssetBundleCreateRequest> handled = new List<AssetBundleCreateRequest>();
        private Dictionary<string, AssetBundle> loadedBundle = new Dictionary<string, AssetBundle>();

        public void UpdateDownLand()
        {
            handled.Clear();
            foreach (var item in m_InProgressOperations)
            {
                if (item.Key.isDone)
                {
                    if (item.Value != null)
                    {
                        item.Value.Invoke(item.Key.assetBundle);
                        handled.Add(item.Key);
                    }
                }
            }
            foreach (var item in handled)
            {
                m_InProgressOperations.Remove(item);
            }
        }
        /// <summary>
        /// 异步加载Asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="assetName"></param>
        /// <param name="OnAssetLoad"></param>
        public void LoadAssetAsync<T>(string filePath, string assetName, UnityAction<T> OnAssetLoad) where T : UnityEngine.Object
        {
            if (!requests.ContainsKey(filePath)){
                requests.Add(filePath, AssetBundle.LoadFromFileAsync(filePath, 0));
            }

            if (loadedBundle.ContainsKey(filePath))
            {
                AssetBundle bundle = loadedBundle[filePath];
                T asset = bundle.LoadAsset<T>(assetName);
                if (OnAssetLoad != null) OnAssetLoad(asset);
            }
            else
            {
                UnityAction<AssetBundle> onrequestLoad = (x) =>
                {
                    loadedBundle[filePath] = x;
                    T asset = x.LoadAsset<T>(assetName);
                    if (OnAssetLoad != null) OnAssetLoad(asset);
                };
                if (m_InProgressOperations.ContainsKey(requests[filePath]))
                {
                    m_InProgressOperations[requests[filePath]] += onrequestLoad;
                }
                else
                {
                    m_InProgressOperations[requests[filePath]] = onrequestLoad;
                }
            }

        }

        /// <summary>
        /// 同步加载Asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public T LoadAsset<T>(string filePath, string assetName) where T : UnityEngine.Object
        {
            AssetBundle bundle = LoadAssetBundleFromFile(filePath);
            T asset = bundle.LoadAsset<T>(assetName);
            bundle.Unload(false);
            return asset;
        }
        AssetBundle LoadAssetBundleFromFile(string filePath)
        {
            return AssetBundle.LoadFromFile(filePath, 0);
        }

        /// <summary>
        /// 释放加载的Assetbundle
        /// </summary>
        /// <param name="filePath"></param>
        public void UnLoadAssetBundle(string filePath)
        {
            if (loadedBundle.ContainsKey(filePath))
            {
                loadedBundle[filePath].Unload(false);;
                loadedBundle.Remove(filePath);
                requests.Remove(filePath);
            }
        }

        public AsyncOperation LoadLevelAsync(string filePath, string assetName, UnityEngine.SceneManagement.LoadSceneMode loadModle)
        {
            AssetBundle bundle = LoadAssetBundleFromFile(filePath);
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(assetName, loadModle);
            bundle.Unload(false);
            return operation;
        }
    }
}