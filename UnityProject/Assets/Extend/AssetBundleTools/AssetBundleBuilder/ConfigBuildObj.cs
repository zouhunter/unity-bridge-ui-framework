using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
namespace AssetBundleBuilder
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ConfigBuildObj")]
    public class ConfigBuildObj : ScriptableObject
    {
        [System.Serializable]
        public class ObjectItem
        {
            public string assetPath;
            public string assetBundleName;//需要刷新时可刷新
            public Object obj;
            public ObjectItem(Object obj)
            {
                this.obj = obj;
            }
            public bool ReFelsh()
            {
                if (obj == null)
                {
                    Debug.LogError("assetPath :" + assetPath + "关联丢失");
                    return false;
                }
                else
                {
                    assetPath = AssetDatabase.GetAssetPath(obj);
                    AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                    assetBundleName = importer.assetBundleName;
                    return true;
                }
            }
        }
        public string LocalPath { get { return localPath + "/" + menuName; } }
        public string TargetPath { get { return targetPath + "/" + menuName; } }
        public string localPath;
        public string targetPath;
        public string menuName;
        public BuildAssetBundleOptions buildOption = BuildAssetBundleOptions.None;
        public BuildTarget buildTarget = BuildTarget.StandaloneWindows;
        public bool clearOld;
        public List<ObjectItem> needBuilds = new List<ObjectItem>();
    }
}
#endif