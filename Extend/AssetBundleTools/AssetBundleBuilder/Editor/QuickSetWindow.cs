using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using System.Text;
namespace AssetBundleBuilder
{

    public class QuickSetWindow : EditorWindow
    {
        [MenuItem(ABBUtility.Menu_QuickSetter)]
        static void OpenArtBundleSetterWidow()
        {
            QuickSetWindow window = EditorWindow.GetWindow<QuickSetWindow>("assetBundle批量设置", true);
            window.position = new Rect(400, 300, 600, 400);
            window.Show();
        }
        enum ArtType
        {
            Texture,
            Material,
            Model,
            Prefab
        }
        enum BuildType
        {
            Single,
            UserDef,
            Dir
        }
        class BundleAbleAsset
        {
            public string fileName;
            public string rootPath;
            public string assetBundleName;
            public string bundleappend;
            private AssetImporter importer;
            private bool changed;
            public BundleAbleAsset(string rootPath, string appendName)
            {
                this.fileName = System.IO.Path.GetFileNameWithoutExtension(rootPath);
                this.rootPath = rootPath;
                if (fileName.Contains("."))
                {
                    Debug.LogWarning("文件名不对:" + rootPath + "\n未进行标记");
                    importer = AssetImporter.GetAtPath(rootPath);
                    importer.assetBundleName = assetBundleName = "";
                    importer.SaveAndReimport();
                }
                else
                {
                    this.bundleappend = appendName.Replace(System.IO.Path.GetFileName(rootPath), fileName);
                    importer = AssetImporter.GetAtPath(rootPath);
                    assetBundleName = importer.assetBundleName;
                }

            }
            public void CreateBundleName(string format, bool removename)
            {
                string append = bundleappend;
                if (removename)
                {
                    append = bundleappend.Replace("/" + fileName, "");
                }

                var bname = string.Format(format, append).Replace(".", "");
                if (bname != assetBundleName)
                {
                    assetBundleName = bname;
                    changed = true;
                }
            }
            public void RemoveBundleName()
            {
                if (assetBundleName != "")
                {
                    assetBundleName = "";
                    changed = true;
                }
            }
            public void SaveBundleName()
            {
                if (changed)
                {
                    importer.assetBundleName = assetBundleName;
                    importer.SaveAndReimport();
                    changed = false;
                }

            }
        }

        string artAssetsFolder;
        ArtType artType;
        bool deepSet;
        BuildType buildType;
        string bundleNameTemp = "{0}";
        string bundleName;
        SerializedProperty script;
        List<BundleAbleAsset> assetFind = new List<BundleAbleAsset>();
        void OnEnable()
        {
            artAssetsFolder = Application.dataPath + "/Projects/Experiment/Details";
            script = new SerializedObject(this).FindProperty("m_Script");
        }
        void OnGUI()
        {
            DrawHead();
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                DrawOptions();
                DrawCtrlButtons();
            }
            DrawItemsList();
        }
        void DrawHead()
        {
            EditorGUILayout.PropertyField(script);
        }
        private void DrawOptions()
        {
            using (var ver = new EditorGUILayout.VerticalScope(GUILayout.Width(400)))
            {
                Rect rect = ver.rect;
                rect.size *= 1.01f;
                BackGroundColor(rect, Color.red);
                BackGroundColor(rect, Color.green);

                Horizontal(() =>
                {
                    EditorGUILayout.LabelField("选择文件夹", GUILayout.Width(100));
                    artAssetsFolder = EditorGUILayout.TextField(artAssetsFolder, GUILayout.Width(300));
                    if (GUILayout.Button(" open "))
                    {
                        if (Selection.activeObject != null)
                        {
                            artAssetsFolder = AssetDatabase.GetAssetPath(Selection.activeObject);
                            if (artAssetsFolder.Contains("."))
                            {
                                artAssetsFolder = System.IO.Path.GetDirectoryName(artAssetsFolder);
                            }
                        }
                    //= EditorUtility.OpenFolderPanel("选择文件夹", artAssetsFolder, "");
                }
                });
                Horizontal(() =>
                {
                    EditorGUILayout.LabelField("选择资源类型", GUILayout.Width(100));
                    artType = (ArtType)EditorGUILayout.EnumPopup(artType, GUILayout.Width(200));
                });
                Horizontal(() =>
                {
                    EditorGUILayout.LabelField("深度遍历", GUILayout.Width(100));
                    deepSet = EditorGUILayout.Toggle(deepSet);
                });
                Horizontal(() =>
                {
                    EditorGUILayout.LabelField("单个打包", GUILayout.Width(100));
                    buildType = (BuildType)EditorGUILayout.EnumPopup(buildType, GUILayout.Width(200));
                });
                Horizontal(() =>
                {
                    switch (buildType)
                    {
                        case BuildType.Single:
                        case BuildType.Dir:
                            EditorGUILayout.LabelField("资源包模板", GUILayout.Width(100));
                            bundleNameTemp = EditorGUILayout.TextField(bundleNameTemp);
                            break;
                        case BuildType.UserDef:
                            EditorGUILayout.LabelField("资源包名", GUILayout.Width(100));
                            bundleName = EditorGUILayout.TextField(bundleName);
                            break;
                        default:
                            break;
                    }
                });
            }
        }
        private void DrawCtrlButtons()
        {
            using (var ver = new EditorGUILayout.VerticalScope(GUILayout.Width(200)))
            {
                if (GUILayout.Button("加载文件", GUILayout.Width(80), GUILayout.Height(30)))
                {
                    OnLoadButtonClicked();
                }
                if (GUILayout.Button("批量生成", GUILayout.Width(80), GUILayout.Height(30)))
                {
                    OnCreateButtonClicked();
                }
                if (GUILayout.Button("批量清空", GUILayout.Width(80), GUILayout.Height(30)))
                {
                    OnClearButtonClicked();
                }
            }
            if (GUILayout.Button("保存配制", GUILayout.Width(90), GUILayout.Height(90)))
            {
                OnSaveButtonClicked();
            }
        }
        Vector2 scrPos;
        private void DrawItemsList()
        {
            GUI.color = Color.yellow;
            Horizontal(() =>
            {
                EditorGUILayout.LabelField("文件名", GUILayout.Width(200));
                EditorGUILayout.LabelField("相对路径", GUILayout.Width(300));
                EditorGUILayout.LabelField("资源包名", GUILayout.Width(200));
                EditorGUILayout.LabelField("工具", GUILayout.Width(100));
            });
            GUI.color = Color.white;

            using (var scroll = new EditorGUILayout.ScrollViewScope(scrPos))
            {
                scrPos = scroll.scrollPosition;
                foreach (var item in assetFind)
                {
                    Horizontal(() =>
                    {
                        EditorGUILayout.SelectableLabel(item.fileName, GUILayout.Width(200));
                        EditorGUILayout.SelectableLabel(item.bundleappend, GUILayout.Width(300));
                        item.assetBundleName = EditorGUILayout.TextField(item.assetBundleName, GUILayout.Width(200));
                        DrawItemTool(item);
                    });
                }

            }
        }

        private void DrawItemTool(BundleAbleAsset item)
        {
            Horizontal(() =>
            {
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    item.RemoveBundleName();
                }
                if (GUILayout.Button("@", GUILayout.Width(20)))
                {
                    switch (artType)
                    {
                        case ArtType.Texture:
                            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(item.rootPath, typeof(Texture));
                            Selection.activeObject = obj;
                            break;
                        case ArtType.Model:
                        case ArtType.Prefab:
                            obj = AssetDatabase.LoadAssetAtPath(item.rootPath, typeof(GameObject));
                            Selection.activeObject = obj;
                            break;
                        case ArtType.Material:
                            obj = AssetDatabase.LoadAssetAtPath(item.rootPath, typeof(Material));
                            Selection.activeObject = obj;
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        void Horizontal(UnityAction action)
        {
            EditorGUILayout.BeginHorizontal();
            action();
            EditorGUILayout.EndHorizontal();
        }

        void OnLoadButtonClicked()
        {
            if (!System.IO.Directory.Exists(artAssetsFolder))
            {
                EditorUtility.DisplayDialog("文件夹未选择", "请选择文件夹后重试", "ok");
                return;
            }
            List<string> allFiles = new List<string>();
            switch (artType)
            {
                case ArtType.Texture:
                    ABBUtility.Recursive(artAssetsFolder, "jpg", deepSet, action: (x) => { allFiles.Add(x); });
                    ABBUtility.Recursive(artAssetsFolder, "png", deepSet, action: (x) => { allFiles.Add(x); });
                    ABBUtility.Recursive(artAssetsFolder, "tga", deepSet, action: (x) => { allFiles.Add(x); });
                    ABBUtility.Recursive(artAssetsFolder, "dds", deepSet, action: (x) => { allFiles.Add(x); });
                    break;
                case ArtType.Model:
                    ABBUtility.Recursive(artAssetsFolder, "fbx", deepSet, action: (x) => { allFiles.Add(x); });
                    break;
                case ArtType.Prefab:
                    ABBUtility.Recursive(artAssetsFolder, "prefab", deepSet, action: (x) => { allFiles.Add(x); });
                    break;
                case ArtType.Material:
                    ABBUtility.Recursive(artAssetsFolder, "mat", deepSet, action: (x) => { allFiles.Add(x); });
                    break;
                default:
                    break;
            }
            string path;
            string appendname;
            assetFind.Clear();
            foreach (var item in allFiles)
            {
                path = item.Replace(Application.dataPath, "Assets");
                appendname = item.Replace(artAssetsFolder + "/", "");
                assetFind.Add(new BundleAbleAsset(path, appendname));
            }
        }

        void OnCreateButtonClicked()
        {
            switch (buildType)
            {
                case BuildType.Dir:
                case BuildType.Single:
                    if (!bundleNameTemp.Contains("{0}"))
                    {
                        EditorUtility.DisplayDialog("格式问题", "请填入正确的字符串模板", "ok");
                    }
                    else
                    {
                        foreach (var item in assetFind)
                        {
                            item.CreateBundleName(bundleNameTemp, buildType == BuildType.Dir);
                        }
                    }
                    break;
                case BuildType.UserDef:
                    if (string.IsNullOrEmpty(bundleName))
                    {
                        EditorUtility.DisplayDialog("资源包名不能为空", "请填入bundle字符串", "ok");
                    }
                    else
                    {
                        foreach (var item in assetFind)
                        {
                            item.assetBundleName = bundleName;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        void OnClearButtonClicked()
        {
            foreach (var item in assetFind)
            {
                item.assetBundleName = "";
            }
        }
        void OnSaveButtonClicked()
        {
            for (int i = 0; i < assetFind.Count; i++)
            {
                assetFind[i].SaveBundleName();
                bool cansale = EditorUtility.DisplayCancelableProgressBar("bundle信息保存中...", string.Format("进度{0}/{1}", i, assetFind.Count), (float)i / assetFind.Count);
                if (cansale)
                {
                    break;
                }
            }
            EditorUtility.ClearProgressBar();
        }
        void BackGroundColor(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.Box(rect, "");
            GUI.color = Color.white;
        }


    }
}