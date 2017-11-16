using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace AssetBundleBuilder {
    public class GlobalBuilderWindow : EditorWindow
    {
        [MenuItem(ABBUtility.Menu_GlobalBuildWindow)]
        static void BuildGlobalAssetBundles()
        {
            EditorWindow.GetWindow<GlobalBuilderWindow>("全局AssetBundle", true);
        }
        
        public string assetBundleName;
        public string localPath = "";
        public string targetPath = "";
        public BuildAssetBundleOptions buildOption = BuildAssetBundleOptions.None;
        public BuildTarget buildTarget = BuildTarget.StandaloneWindows;
        private SerializedProperty script;

        private const string Perfer_buildPath = "globalbuildPath";
        private const string Perfer_localPath = "localglobalbuildPath";

        void OnEnable()
        {
            script = new SerializedObject(this).FindProperty("m_Script");
            if (EditorPrefs.HasKey(Perfer_buildPath))
            {
                localPath = EditorPrefs.GetString(Perfer_buildPath);
            }
        }
        void OnGUI()
        {
            EditorGUILayout.PropertyField(script);
            EditorGUILayout.BeginHorizontal();
            localPath = EditorGUILayout.TextField("ExportTo", localPath);
            if (GUILayout.Button("选择路径"))
            {
                var path = EditorUtility.SaveFolderPanel("选择保存路径", localPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    localPath = path;
                    EditorPrefs.SetString(Perfer_buildPath, localPath);
                    this.Repaint();
                }
            }
            EditorGUILayout.EndHorizontal();
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                targetPath = EditorGUILayout.TextField("CopyTo", targetPath);
                if (GUILayout.Button("选择路径"))
                {
                    var path = EditorUtility.SaveFolderPanel("选择本地路径", targetPath, "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        targetPath = path;
                        EditorPrefs.SetString(Perfer_buildPath, targetPath);
                        this.Repaint();
                    }
                }
                if (GUILayout.Button("Copy"))
                {
                    FileUtil.DeleteFileOrDirectory(targetPath);
                    FileUtil.CopyFileOrDirectory(localPath, targetPath);
                }
            }
            buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("BuildTarget", buildTarget);

         
                #region 全局打包
                buildOption = (BuildAssetBundleOptions)EditorGUILayout.EnumMaskField("Options", buildOption);
                if (GUILayout.Button("GlobleBulid"))
                {
                    ABBUtility.BuildGlobalAssetBundle(localPath, buildOption, buildTarget);
                }
                #endregion
        }
    }
}
