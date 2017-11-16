using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace AssetBundleBuilder
{
    public class BundleViewWindow : EditorWindow
    {
        [MenuItem(ABBUtility.Menu_BundleName)]
        static void OpenAssetBundleNameConfigWidow()
        {
            BundleViewWindow window = EditorWindow.GetWindow<BundleViewWindow>("assetBundle配制", true);
            window.position = new Rect(400, 300, 700, 500);
            window.Show();
        }

        List<AssetImporter> importerList = new List<AssetImporter>();
        Dictionary<AssetImporter, Object> objectList = new Dictionary<AssetImporter, Object>();
        Object waitAdd = null;
        AssetImporter waitDelete;
        Vector2 scrollPos;
        string newBundleName = "";

        void OnEnable()
        {
            if (Selection.activeObject != null)
            {
                waitAdd = Selection.activeObject;
            }
        }

        void LoadAllAssetBundle()
        {
            string[] assets;
            string[] allAssetBundle = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < allAssetBundle.Length; i++)
            {
                assets = AssetDatabase.GetAssetPathsFromAssetBundle(allAssetBundle[i]);
                for (int j = 0; j < assets.Length; j++)
                {
                    AssetImporter importer = AssetImporter.GetAtPath(assets[j]);
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(assets[j]);
                    importer.name = obj.name;
                    objectList.Add(importer, obj);
                    importerList.Add(importer);
                }
            }
        }

        void LoadFolderAssetBundle(string path)
        {
            List<string> bundleObjs = new List<string>();
            ABBUtility.RecursiveSub(path, action: (x) => { bundleObjs.Add(x); });
            for (int i = 0; i < bundleObjs.Count; i++)
            {
                AssetImporter importer = AssetImporter.GetAtPath(FileUtil.GetProjectRelativePath(bundleObjs[i]));
                if (importer != null)
                {
                    if (!string.IsNullOrEmpty(importer.assetBundleName))
                    {
                        Object obj = AssetDatabase.LoadAssetAtPath<Object>(importer.assetPath);
                        importer.name = obj.name;
                        objectList.Add(importer, obj);
                        importerList.Add(importer);
                    }
                }

            }
        }

        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < importerList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                ShowObjectItemInfo(importerList[i]);

                EditorGUILayout.EndHorizontal();
            }

            RemoveIfNeed();

            GUILayout.Label("---");
            EditorGUILayout.BeginHorizontal();

            ShowControllButtons();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();

            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        void ShowObjectItemInfo(AssetImporter impoter)
        {
            if (!objectList.ContainsKey(impoter)) return;

            objectList[impoter] = EditorGUILayout.ObjectField(objectList[impoter], typeof(Object), false);

            impoter.name = EditorGUILayout.TextField(impoter.name);

            impoter.assetBundleName = EditorGUILayout.TextField(impoter.assetBundleName);

            if (GUILayout.Button("-"))
            {
                waitDelete = impoter;
            }

        }

        void ShowControllButtons()
        {
            waitAdd = EditorGUILayout.ObjectField(waitAdd, typeof(Object), false);
            string name = "";
            if (waitAdd != null)
            {
                name = EditorGUILayout.TextField(waitAdd.name);
            }
            newBundleName = EditorGUILayout.TextField(newBundleName);

            if (GUILayout.Button("+") && waitAdd != null)
            {
                if (!importerList.Find((x) => x.name == name && x.assetBundleName == newBundleName))
                {
                    string path = AssetDatabase.GetAssetPath(waitAdd);

                    AssetImporter impoter = AssetImporter.GetAtPath(path);
                    impoter.assetBundleName = newBundleName;
                    impoter.name = name;
                    importerList.Add(impoter);
                    objectList.Add(impoter, waitAdd);
                }
            }
            if (GUILayout.Button("ReloadFolder"))
            {
                string path = Application.dataPath;
                if (Selection.activeObject != null)
                {
                    path = AssetDatabase.GetAssetPath(Selection.activeObject);
                    path = System.IO.Path.GetDirectoryName(path);
                }
                string folder = EditorUtility.OpenFolderPanel("选择文件夹", path, "");
                if (!string.IsNullOrEmpty(folder))
                {
                    importerList.Clear();
                    objectList.Clear();
                    LoadFolderAssetBundle(folder);
                }
            }
            if (GUILayout.Button("ReloadAll"))
            {
                importerList.Clear();
                objectList.Clear();
                LoadAllAssetBundle();
            }
        }

        void RemoveIfNeed()
        {
            if (waitDelete != null)
            {
                importerList.Remove(waitDelete);
                objectList.Remove(waitDelete);
                waitDelete.assetBundleName = "";
                waitDelete.name = "";
                waitDelete = null;
            }
        }
    }
}
