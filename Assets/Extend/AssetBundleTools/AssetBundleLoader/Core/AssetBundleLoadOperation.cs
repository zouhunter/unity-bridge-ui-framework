using UnityEngine;
using System.Collections;
using System;
using Object = UnityEngine.Object;
namespace AssetBundles
{
    public abstract class AssetBundleLoadOperation : IEnumerator
    {
        public object Current
        {
            get
            {
                return null;
            }
        }
        public bool MoveNext()
        {
            return !IsDone();
        }

        public void Reset()
        {
        }

        abstract public bool Update(UrlAssetBundleLoadCtrl loadCtrl);

        abstract public bool IsDone();
    }


    public class AssetBundleLoadLevelOperation : AssetBundleLoadOperation
    {
        protected string m_AssetBundleName;
        protected string m_LevelName;
        protected bool m_IsAdditive;
        protected string m_DownloadingError;
        public AsyncOperation m_Request;

        public AssetBundleLoadLevelOperation(string assetbundleName, string levelName, bool isAdditive)
        {
            m_AssetBundleName = assetbundleName;
            m_LevelName = levelName;
            m_IsAdditive = isAdditive;
        }

        public override bool Update(UrlAssetBundleLoadCtrl loadCtrl)
        {
            if (m_Request != null)
                return false;

            LoadedAssetBundle bundle = loadCtrl.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
            if (bundle != null)
            {
                if (m_IsAdditive)
                    m_Request = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_LevelName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                else
                    m_Request = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_LevelName, UnityEngine.SceneManagement.LoadSceneMode.Single);
                return false;
            }
            else
                return true;
        }

        public override bool IsDone()
        {
            // Return if meeting downloading error.
            // m_DownloadingError might come from the dependency downloading.
            if (m_Request == null && m_DownloadingError != null)
            {
                Debug.LogError(m_DownloadingError);
                return true;
            }

            return m_Request != null && m_Request.isDone;
        }
    }
    public abstract class AssetBundleLoadAssetsOperation : AssetBundleLoadOperation
    {
        public abstract T[] GetAssets<T>() where T : Object;
    }
    public class AssetBundleLoadAssetsOperationFull : AssetBundleLoadAssetsOperation
    {
        protected string m_AssetBundleName;
        protected string m_DownloadingError;
        protected System.Type m_Type;
        protected AssetBundleRequest m_Request = null;

        public AssetBundleLoadAssetsOperationFull(string bundleName, System.Type type)
        {
            m_AssetBundleName = bundleName;
            m_Type = type;
        }
        // Returns true if more Update calls are required.
        public override bool Update(UrlAssetBundleLoadCtrl loadCtrl)
        {
            if (m_Request != null)
                return false;

            LoadedAssetBundle bundle = loadCtrl.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
            if (bundle != null)
            {
                ///@TODO: When asset bundle download fails this throws an exception...
                m_Request = bundle.m_AssetBundle.LoadAllAssetsAsync(m_Type);
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool IsDone()
        {
            // Return if meeting downloading error.
            // m_DownloadingError might come from the dependency downloading.
            if (m_Request == null && m_DownloadingError != null)
            {
                Debug.LogError(m_DownloadingError);
                return true;
            }

            return m_Request != null && m_Request.isDone;
        }

        public override T[] GetAssets<T>()
        {
            if (m_Request != null && m_Request.isDone)
            {
                System.Collections.Generic.List<T> items = new System.Collections.Generic.List<T>();
                foreach (var item in m_Request.allAssets)
                {
                    if (item is T)
                    {
                        items.Add((T)item);
                    }
                }
                return items.ToArray();
            }
            else
                return null;
        }
    }

    public abstract class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
    {
        public abstract T GetAsset<T>() where T : UnityEngine.Object;
    }

    public class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation
    {
        protected string m_AssetBundleName;
        protected string m_AssetName;
        protected string m_DownloadingError;
        protected System.Type m_Type;
        protected AssetBundleRequest m_Request = null;

        public AssetBundleLoadAssetOperationFull(string bundleName, string assetName, System.Type type)
        {
            m_AssetBundleName = bundleName;
            m_AssetName = assetName;
            m_Type = type;
        }

        public override T GetAsset<T>()
        {
            if (m_Request != null && m_Request.isDone)
            {
                return m_Request.asset as T;
            }
            else
                return null;
        }

        // Returns true if more Update calls are required.
        public override bool Update(UrlAssetBundleLoadCtrl loadCtrl)
        {
            if (m_Request != null)
                return false;

            LoadedAssetBundle bundle = loadCtrl.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
            if (bundle != null)
            {
                ///@TODO: When asset bundle download fails this throws an exception...
                m_Request = bundle.m_AssetBundle.LoadAssetAsync(m_AssetName, m_Type);
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool IsDone()
        {
            // Return if meeting downloading error.
            // m_DownloadingError might come from the dependency downloading.
            if (m_Request == null && m_DownloadingError != null)
            {
                return true;
            }

            return m_Request != null && m_Request.isDone;
        }
    }

    public class AssetBundleLoadManifestOperation : AssetBundleLoadAssetOperationFull
    {
        public AssetBundleLoadManifestOperation(string bundleName, string assetName, System.Type type)
            : base(bundleName, assetName, type)
        {
        }

        public override bool Update(UrlAssetBundleLoadCtrl loadCtrl)
        {
            base.Update(loadCtrl);

            if (m_Request != null && m_Request.isDone)
            {
                loadCtrl.AssetBundleManifestObject = GetAsset<AssetBundleManifest>();
                return false;
            }
            else
                return true;
        }
    }


}
