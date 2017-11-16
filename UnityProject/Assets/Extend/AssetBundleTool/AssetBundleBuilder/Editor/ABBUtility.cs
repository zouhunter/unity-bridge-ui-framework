using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;

namespace AssetBundleBuilder
{
    public static class ABBUtility
    {
        public const string Menu_GlobalBuildWindow = "AssetBundle/AssetBundleBuilder/Builder/GlobalBuild";
        public const string Menu_ConfigBuildWindow = "AssetBundle/AssetBundleBuilder/Builder/ConfigBuild";
        public const string Menu_SelectBuildWindow = "AssetBundle/AssetBundleBuilder/Builder/SelectBuild";
        public const string Menu_BundleName = "AssetBundle/AssetBundleBuilder/Setter/BundleView";
        public const string Menu_QuickSetter = "AssetBundle/AssetBundleBuilder/Setter/QuickSetter";
        public static void BuildGlobalAssetBundle(string path, BuildAssetBundleOptions option, BuildTarget target)
        {
            BuildAllAssetBundles(path, option, target);
        }
        public static void BuildSelectAssets(string abName, string path, UnityEngine.Object[] obj, BuildTarget target)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            AssetBundleBuild[] builds = new AssetBundleBuild[1];
            builds[0].assetBundleName = abName;
            builds[0].assetNames = new string[obj.Length];
            
            for (int i = 0; i < obj.Length; i++)
            {
                builds[0].assetNames[i] = AssetDatabase.GetAssetPath(obj[i]);
            }
            BuildPipeline.BuildAssetBundles(path, builds, BuildAssetBundleOptions.DeterministicAssetBundle, target);
            AssetDatabase.Refresh();
        }
        public static void BuildGroupBundles(string path,AssetBundleBuild[] builds, BuildAssetBundleOptions option, BuildTarget target)
        {
            if (!System.IO.Directory.Exists(path)) {
                System.IO.Directory.CreateDirectory(path);
            }
            BuildPipeline.BuildAssetBundles(path, builds, BuildAssetBundleOptions.DeterministicAssetBundle, target);
            AssetDatabase.Refresh();
        }
        public static void BuildAllAssetBundles(string path, BuildAssetBundleOptions option, BuildTarget target)
        {
            if (!System.IO.Directory.Exists(path)) {
                System.IO.Directory.CreateDirectory(path);
            }
            BuildPipeline.BuildAssetBundles(path, option, target);
        }

        /// <summary>
        /// 遍历目录及其子目录
        /// </summary>
        public static void RecursiveSub(string path, string ignoreFileExt = ".meta", string ignorFolderEnd = "_files", Action<string> action = null)
        {
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);
            foreach (string filename in names)
            {
                string ext = Path.GetExtension(filename);
                if (ext.Equals(ignoreFileExt)) continue;
                action(filename.Replace('\\', '/'));
            }
            foreach (string dir in dirs)
            {
                if (dir.EndsWith(ignorFolderEnd)) continue;
                RecursiveSub(dir, ignoreFileExt, ignorFolderEnd, action);
            }
        }
        /// <summary>
        /// 遍历目录及其子目录
        /// </summary>
        public static void Recursive(string path, string fileExt, bool deep = true, Action<string> action = null)
        {
            string[] names = Directory.GetFiles(path);
            foreach (string filename in names)
            {
                string ext = Path.GetExtension(filename);
                if (ext.ToLower().Contains(fileExt.ToLower()))
                    action(filename.Replace('\\', '/'));
            }
            if (deep)
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    Recursive(dir, fileExt, deep, action);
                }
            }

        }
    }
}