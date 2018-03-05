using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using AssetBundleReference.Tuples;

public class AssetBundleLoader :MonoBehaviour
{
    private const string defultMenu = "AssetBundle";
#if UNITY_EDITOR
    //private static int m_SimulateAssetBundleInEditor;
    private static string kSimulateAssetBundles = "simulateinEditor";
    private ISimulationLoader simuationLoader;
    // Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
    public static bool SimulateAssetBundleInEditor
    {
        get
        {
            return UnityEditor.EditorPrefs.GetBool(kSimulateAssetBundles, true);
        }
        set
        {
            UnityEditor.EditorPrefs.SetBool(kSimulateAssetBundles, value);
        }
    }
#endif
    #region 单例
    private static object lockHelper = new object();
    private static bool isQuit = false;
    private static AssetBundleLoader defult;
    public static AssetBundleLoader Instence
    {
       get
        {
            if (defult == null)
            {
                lock (lockHelper)
                {
                    if (defult == null && !isQuit)
                    {
                        GameObject go = new GameObject(defultMenu);
                        defult = go.AddComponent<AssetBundleLoader>();
                        var url =
#if UNITY_STANDALONE || UNITY_EDITOR
                           "file://" + Application.streamingAssetsPath + "/" + defultMenu;
#else
                            Application.streamingAssetsPath + "/" + defultMenu;
#endif
                        loaderDic.Add(url, defult);
                        defult.Init(url, defultMenu);
                    }
                }
            }
            return defult;
        }
    }
    public static AssetBundleLoader GetInstance(string url,string menu)
    {
        AssetBundleLoader instance = null;
        if (!loaderDic.TryGetValue(url, out instance))
        {
            lock (lockHelper)
            {
                if (instance == null && !isQuit)
                {
                    GameObject go = new GameObject(url);
                    instance = go.AddComponent<AssetBundleLoader>();
                    instance.Init(url,menu);
                    loaderDic.Add(url, instance);
                }
            }
        }
        return instance;
    }
#if UNITY_EDITOR
    private bool canSimulation = true;
#endif
    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void OnApplicationQuit()
    {
        isQuit = true;
    }
    #endregion 

    private IUrlAssetBundleLoadCtrl activeLoader;
    private bool isDownLanding;
    private bool menuLoaded;
    private Queue<Tuple<string, string, UnityAction<UnityEngine.Object>>> m_LoadObjectQueue =
      new Queue<Tuple<string, string, UnityAction<UnityEngine.Object>>>();
    private static Dictionary<string, AssetBundleLoader> loaderDic = new Dictionary<string, AssetBundleLoader>();

    protected void Init(string url,string menu)
    {
        //资源加载
        UrlAssetBundleLoadCtrl.logMode = AssetBundles.UrlAssetBundleLoadCtrl.LogMode.JustErrors;
        activeLoader = new UrlAssetBundleLoadCtrl(url, menu);

#if UNITY_EDITOR
        canSimulation = url.Contains(Application.streamingAssetsPath + "/" + defultMenu);
        if(canSimulation) simuationLoader = new SimulationLoader(this);
#endif
    }
    void Update()
    {
        if (activeLoader != null)
        {
            activeLoader.UpdateDownLand();
            if (!isDownLanding)
            {
                if (m_LoadObjectQueue.Count > 0)
                {
                    Tuple<string, string, UnityAction<UnityEngine.Object>> data = m_LoadObjectQueue.Dequeue();
                    LoadAssetFromUrlAsync(data.Element1, data.Element2, data.Element3);
                }
            }
        }
    }
    /// <summary>
    /// 加载依赖关系
    /// </summary>
    /// <param name="onMenuLoad"></param>
    private void LoadMenu(UnityAction onMenuLoad)
    {
        if (menuLoaded)
        {
            onMenuLoad();
        }
        else
        {
            UnityAction newOnMenuLoad = () =>
            {
                menuLoaded = true;
                if(onMenuLoad != null) onMenuLoad.Invoke();
            };
            AssetBundleLoadOperation initopera = activeLoader.Initialize();
            StartCoroutine(WaitInalize(initopera, newOnMenuLoad));
        }
    }
    /// <summary>
    /// 从url异步加载一个资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetBundleName"></param>
    /// <param name="assetName"></param>
    /// <param name="onAssetLoad"></param>
    public void LoadAssetFromUrlAsync<T>(string assetBundleName, string assetName, UnityAction<T> onAssetLoad) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (canSimulation && SimulateAssetBundleInEditor)
        {
            simuationLoader.LoadAssetAsync(assetBundleName, assetName, (x) =>
            {
                onAssetLoad((T)x);
            });
            return;
        }
#endif
        LoadMenu(() =>
            {
                if (isDownLanding)
                {
                    m_LoadObjectQueue.Enqueue(new Tuple<string, string, UnityAction<UnityEngine.Object>>(assetBundleName, assetName, (x) => onAssetLoad((T)x)));
                    return;
                }
                else
                {
                    isDownLanding = true;
                    onAssetLoad += (x) => {
                        activeLoader.UnloadAssetBundle(assetBundleName);
                        isDownLanding = false;
                    };
                    AssetBundleLoadAssetOperation operation = activeLoader.LoadAssetAsync(assetBundleName, assetName, typeof(T));
                    StartCoroutine(WaitLoadObject(operation, onAssetLoad));
                }
            });

    }
    /// <summary>
    /// 异步加载一组资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetBundleName"></param>
    /// <param name="onAssetsLoad"></param>
    public void LoadAssetsFromUrlAsync<T>(string assetBundleName, UnityAction<T[]> onAssetsLoad) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (canSimulation && SimulateAssetBundleInEditor)
        {
            T[] asset = simuationLoader.LoadAssets<T>(assetBundleName);
            onAssetsLoad(asset);
            return;
        }
#endif
        if (activeLoader != null)
        {
            LoadMenu(() =>
            {
                onAssetsLoad += (x) => { activeLoader.UnloadAssetBundle(assetBundleName); };
                AssetBundleLoadAssetsOperation operation = activeLoader.LoadAssetsAsync(assetBundleName, typeof(T));
                StartCoroutine(WaitLoadObjects(operation, onAssetsLoad));
            });
        }
        else
        {
            Debug.Log("Please Set Menu");
        }
    }
    /// <summary>
    /// 从url异步从bundle中加载一组资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetBundleName"></param>
    /// <param name="assetNames"></param>
    /// <param name="allAssetLoad"></param>
    public void LoadAssetsFromUrlAsync<T>(string assetBundleName, string[] assetNames, UnityAction<T[]> allAssetLoad) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (canSimulation && SimulateAssetBundleInEditor)
        {
            T[] asset = simuationLoader.LoadAssets<T>(assetBundleName, assetNames);
            allAssetLoad(asset);
            return;
        }
#endif
        if (activeLoader != null)
        {
            LoadMenu(() =>
            {
                T[] objectPool = new T[assetNames.Length];
                int j = 0;


                for (int i = 0; i < assetNames.Length; i++)
                {
                    int index = i;
                    UnityAction<T> loadOnce = (x) =>
                    {
                        objectPool[index] = x;
                        j++;
                        if (j == assetNames.Length)
                        {
                            allAssetLoad(objectPool);
                            activeLoader.UnloadAssetBundle(assetBundleName);
                        }
                    };
                    AssetBundleLoadAssetOperation operation = activeLoader.LoadAssetAsync(assetBundleName, assetNames[index], typeof(T));
                    StartCoroutine(WaitLoadObject(operation, loadOnce));
                }
            });
        }
        else
        {
            Debug.Log("Please Set Menu");
        }
    }
    /// <summary>
    /// 从url加载出场景
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="assetName"></param>
    /// <param name="isAddictive"></param>
    /// <param name="onLevelLoad"></param>
    public void LoadLevelFromUrlAsync(string assetBundleName, string assetName, bool isAddictive, UnityAction<float> onProgressChange)
    {
#if UNITY_EDITOR
        if (canSimulation && SimulateAssetBundleInEditor)
        {
            simuationLoader.LoadSceneAsync(assetBundleName, assetName, isAddictive, onProgressChange);
            return;
        }
#endif
        if (activeLoader != null)
        {
            LoadMenu(() =>
            {
                
                AssetBundleLoadLevelOperation operation = activeLoader.LoadLevelAsync(assetBundleName, assetName, isAddictive);
                StartCoroutine(WaitLoadLevel(operation, onProgressChange));
            });
        }
        else
        {
            Debug.Log("Please Set Menu");
        }
    }


    IEnumerator WaitInalize(AssetBundleLoadOperation operation, UnityAction onActive)
    {
        yield return operation;
        if (onActive != null) onActive.Invoke();
    }
    IEnumerator WaitLoadObject<T>(AssetBundleLoadAssetOperation operation, UnityAction<T> onLoad) where T : UnityEngine.Object
    {
        yield return operation;
        if (onLoad != null)
        {
            T asset = operation.GetAsset<T>();
            if (asset == null){
                Debug.Log(operation.IsDone());
            }
            onLoad.Invoke(asset);
        }
    }
    IEnumerator WaitLoadObjects<T>(AssetBundleLoadAssetsOperation operation, UnityAction<T[]> onLoad) where T : UnityEngine.Object
    {
        yield return operation;
        if (onLoad != null)
        {
            T[] asset = operation.GetAssets<T>();
            onLoad.Invoke(asset);
        }
    }
    IEnumerator WaitLoadLevel(AssetBundleLoadLevelOperation operation, UnityAction<float> onProgressChanged)
    {
        while (!operation.IsDone())
        {
            if (operation.m_Request != null)
            {
                operation.m_Request.allowSceneActivation = false;
                if (operation.m_Request.progress >= 0.9f)
                {
                    operation.m_Request.allowSceneActivation = true;
                    if (onProgressChanged != null)  onProgressChanged(1);
                    break;
                }
                else
                {
                    if (onProgressChanged != null) onProgressChanged(operation.m_Request.progress);
                }

            }
            yield return null;
        }
    }
    IEnumerator WaitLoadLevel(AsyncOperation operation, UnityAction<float> onProgressChanged)
    {
        while (!operation.isDone)
        {
            operation.allowSceneActivation = false;
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
                if (onProgressChanged != null) onProgressChanged(1);
                break;

            }
            else
            {
                if (onProgressChanged != null) onProgressChanged(operation.progress);
            }
            yield return null;
        }
    }
}
