using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using AssetBundleReference.Tuples;
using Object = UnityEngine.Object;
namespace AssetBundles
{

#if UNITY_EDITOR
    public interface ISimulationLoader
    {
        T LoadAsset<T>(string bundleName, string assetName) where T : UnityEngine.Object;
        void LoadAssetAsync(string bundleName, string assetName, UnityAction<Object> onLoad);// where T : UnityEngine.Object;
        T[] LoadAssets<T>(string bundleName, params string[] assetName) where T : UnityEngine.Object;
        T[] LoadAssets<T>(string bundleName) where T : UnityEngine.Object;
        void LoadSceneAsync(string bundleName, string sceneName, bool isAddictive, UnityAction<float> onProgressChanged);
    }
    public class SimulationLoader : ISimulationLoader
    {
        MonoBehaviour holder;
        public SimulationLoader(MonoBehaviour holder)
        {
            this.holder = holder;
        }
        Queue<Tuple<string, string, UnityAction<Object>>> tupes = new Queue<Tuple<string, string, UnityAction<Object>>>();// Tuple<string, string, UnityAction<Object>>();
        public T LoadAsset<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {
            string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetName);
            if (assetPaths.Length == 0)
            {
                return null;
            }
            // @TODO: Now we only get the main object from the first asset. Should consider type also.
            T target = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPaths[0]);
            return target;
        }

        public T[] LoadAssets<T>(string bundleName, params string[] assetNames) where T : UnityEngine.Object
        {
            T[] objectPool = new T[assetNames.Length];
            for (int i = 0; i < assetNames.Length; i++)
            {
                objectPool[i] = LoadAsset<T>(bundleName, assetNames[i]);
            }
            return objectPool;
        }

        public T[] LoadAssets<T>(string bundleName) where T : UnityEngine.Object
        {
            string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
            if (assetPaths.Length == 0)
            {
                Debug.LogError("There is no asset with type \"" + typeof(T).ToString() + "\" in " + bundleName);
            }
            List<T> items = new List<T>();
            for (int i = 0; i < assetPaths.Length; i++)
            {
                T item = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPaths[i]);
                if (item != null)
                {
                    items.Add(item);
                }
            }
            return items.ToArray();
        }
        public void LoadSceneAsync(string bundleName, string sceneName, bool isAddictive, UnityAction<float> onProgressChanged)
        {
            string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, sceneName);
            AsyncOperation m_Operation;
            if (isAddictive)
                m_Operation = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode(levelPaths[0]);
            else
                m_Operation = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode(levelPaths[0]);
            holder.StartCoroutine(SimulationWaitLoadLevel(m_Operation, onProgressChanged));
        }
        IEnumerator SimulationWaitLoadLevel(AsyncOperation operation, UnityAction<float> onProgressChanged)
        {
            while (!operation.isDone)
            {
                operation.allowSceneActivation = false;
                onProgressChanged(operation.progress);
                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }
                yield return null;
            }
        }

        public void LoadAssetAsync(string bundleName, string assetName, UnityAction<Object> onLoad)// where T : UnityEngine.Object
        {
            var tupe = new Tuple<string, string, UnityAction<Object>>(bundleName, assetName, onLoad);
            tupes.Enqueue(tupe);
            if (tupes.Count == 1){
                holder.StartCoroutine(SimulationWaitLoadObject());
            }
        }

        IEnumerator SimulationWaitLoadObject()
        {
            while (tupes.Count > 0)
            {
                var tupe = tupes.Dequeue();
                tupe.Element3.Invoke(LoadAsset<Object>(tupe.Element1, tupe.Element2));
                yield return new WaitForEndOfFrame();
            }
        }
    }
#endif
}