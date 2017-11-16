using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace AssetBundleBuilder
{
    public class SelectBuildWindow : EditorWindow
    {

        [MenuItem(ABBUtility.Menu_SelectBuildWindow)]
        static void BuildSingleAssetBundle()
        {
            EditorWindow.GetWindow<SelectBuildWindow>("局部AssetBundle", true);
        }

        public string assetBundleName;
        public string buildpath = "";
        public BuildAssetBundleOptions buildOption = BuildAssetBundleOptions.None;
        public BuildTarget buildTarget = BuildTarget.StandaloneWindows;
        private SerializedProperty script;

        private Object[] allObject
        {
            get
            {
                var objs = new List<Object>();
                objs.AddRange(selectionScene);
                objs.AddRange(selectionGameObject);
                objs.AddRange(selectionMaterial);
                objs.AddRange(selectionTexture);
                return objs.ToArray();
            }
        }
        private Object[] selectionScene = new Object[0];
        private Object[] selectionGameObject = new Object[0];
        private Object[] selectionMaterial = new Object[0];
        private Object[] selectionTexture = new Object[0];
        private const string Perfer_buildPath = "selectbuildPath";

        void OnEnable()
        {
            script = new SerializedObject(this).FindProperty("m_Script");
            SelectionChanged();
            Selection.selectionChanged += SelectionChanged;
            if (EditorPrefs.HasKey(Perfer_buildPath))
            {
                buildpath = EditorPrefs.GetString(Perfer_buildPath);
            }
        }


        void OnGUI()
        {
            EditorGUILayout.PropertyField(script);
            EditorGUILayout.BeginHorizontal();
            buildpath = EditorGUILayout.TextField("ExportTo", buildpath);
            if (GUILayout.Button("选择路径"))
            {
                var path = EditorUtility.SaveFolderPanel("选择保存路径", buildpath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    buildpath = path;
                    EditorPrefs.SetString(Perfer_buildPath, buildpath);
                    this.Repaint();
                }
            }
            EditorGUILayout.EndHorizontal();

            buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("BuildTarget", buildTarget);

            assetBundleName = EditorGUILayout.TextField("AbName", assetBundleName);
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("BuildAllSelected"))
                {
                    ABBUtility.BuildSelectAssets(assetBundleName, buildpath, allObject, buildTarget);
                }
                if (GUILayout.Button("BuildSelectedScenes"))
                {
                    ABBUtility.BuildSelectAssets(assetBundleName, buildpath, selectionScene, buildTarget);
                }
                if (GUILayout.Button("BuildSelectedGameObjects"))
                {
                    ABBUtility.BuildSelectAssets(assetBundleName, buildpath, selectionGameObject, buildTarget);
                }
                else if (GUILayout.Button("BuildSelectedMaterials"))
                {
                    ABBUtility.BuildSelectAssets(assetBundleName, buildpath, selectionMaterial, buildTarget);
                }
                else if (GUILayout.Button("BuildSelectedTextures"))
                {
                    ABBUtility.BuildSelectAssets(assetBundleName, buildpath, selectionTexture, buildTarget);
                }
            }
        }
        void SelectionChanged()
        {
            selectionScene = Selection.GetFiltered(typeof(SceneAsset), SelectionMode.DeepAssets);
            selectionGameObject = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);
            selectionMaterial = Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);
            selectionTexture = Selection.GetFiltered(typeof(Texture), SelectionMode.DeepAssets);
        }
    }
}
