using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
namespace AssetBundleBuilder
{
    public class LayerNode
    {
        public bool isExpanded { get; private set; }
        public bool isFolder;
        public bool selected { get; private set; }
        public int indent { get; private set; }
        public GUIContent content;
        public LayerNode parent;
        public List<LayerNode> childs = new List<LayerNode>();
        public Object layer { get; private set; }
        public string assetPath { get; private set; }

        private GUIContent _spritenormal;
        private string ContentName
        {
            get
            {
                AssetImporter asset = AssetImporter.GetAtPath(assetPath);
                if (string.IsNullOrEmpty(asset.assetBundleName))
                {
                    return layer.name;
                }
                else
                {
                    return string.Format("{0}  [ab]:{1}", layer.name, asset.assetBundleName);
                }
            }
        }

        public LayerNode(string path)
        {
            this.assetPath = path;
            this.layer = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (layer == null)
            {
                Debug.LogError(path + "null");
            }
            this.indent = assetPath.Split('/').Length;
            isFolder = ProjectWindowUtil.IsFolder(layer.GetInstanceID());
            var name = ContentName;
            _spritenormal = new GUIContent(name, AssetDatabase.GetCachedIcon(assetPath));

            content = _spritenormal;
            isExpanded = true;
        }

        public void Expland(bool on)
        {
            isExpanded = on;
        }
        public void Select(bool on)
        {
            selected = on;
            if (childs != null)
                foreach (var child in childs)
                {
                    child.Select(on);
                }
        }
    }

    [CustomEditor(typeof(ConfigBuildObj))]
    public class ConfigBuildObjDrawer : Editor
    {
        private SerializedProperty script;
        private SerializedProperty localPath_prop;
        private SerializedProperty targetPath_prop;
        private SerializedProperty menuName_prop;

        private LayerNode rootNode;
        private const string lastItem = "lastbuildObj";
        private const string rootFolder = "Assets";
        private Vector2 scrollPos;
        private Dictionary<string, LayerNode> nodeDic;
        private GUIContent _groupff;
        private GUIContent _groupOn;
            
        private float rectHeight = 400;
        private void OnEnable()
        {
            _groupff = new GUIContent(EditorGUIUtility.IconContent("IN foldout focus").image);
            _groupOn = new GUIContent(EditorGUIUtility.IconContent("IN foldout focus on").image);
            script = serializedObject.FindProperty("m_Script");
            localPath_prop = serializedObject.FindProperty("localPath");
            menuName_prop = serializedObject.FindProperty("menuName");
            targetPath_prop = serializedObject.FindProperty("targetPath");
            ReFelsh((target as ConfigBuildObj).needBuilds);
            nodeDic = LoadDicFromObj((ConfigBuildObj)target);
            rootNode = LoadNodesFromDic(nodeDic);
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(script);
            DrawObjOptions();
            DrawHeadTools();
            EditorGUI.indentLevel = 0;
            var lastrect = GUILayoutUtility.GetLastRect();
            var viewRect = new Rect(lastrect.x, lastrect.y + EditorGUIUtility.singleLineHeight * 1.2f, lastrect.width, rectHeight);
            EditorGUI.DrawRect(viewRect, new Color(0, 1, 0, 0.1f));

            AcceptDrop(viewRect);

            using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPos, false, true, GUILayout.Height(rectHeight)))
            {
                scroll.handleScrollWheel = true;
                scrollPos = scroll.scrollPosition;
                DrawData(rootNode);
            }

            EditorGUI.indentLevel = 0;
            DrawBottomTools();
            serializedObject.ApplyModifiedProperties();
        }

        private void AcceptDrop(Rect viewRect)
        {
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    if (viewRect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.None;
                    }
                    break;
                case EventType.DragPerform:
                    if (viewRect.Contains(Event.current.mousePosition))
                    {
                        if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                        {
                            AddGroupOfObject(DragAndDrop.paths);
                        }
                    }
                    Event.current.Use();
                    break;
                case EventType.DragExited:
                    break;
                default:
                    break;
            }
        }
        private void AddGroupOfObject(params string[] paths)
        {
            foreach (var item in paths)
            {
                var assetPath = item;
                RetriveObject(assetPath, (x) =>
                {
                    RetriveAddFolder(assetPath, nodeDic);

                    assetPath = AssetDatabase.GetAssetPath(x);

                    if (!nodeDic.ContainsKey(assetPath))
                    {
                        nodeDic.Add(assetPath, new LayerNode(assetPath));
                    }
                });

            }

            rootNode = LoadNodesFromDic(nodeDic);
        }
        private void DrawObjOptions()
        {
            using (var ver = new EditorGUILayout.VerticalScope())
            {
                using (var hor = new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("[导出路径]:", GUILayout.Width(100));
                    localPath_prop.stringValue = EditorGUILayout.TextField(localPath_prop.stringValue);
                    if (GUILayout.Button("选择"))
                    {
                        var path = EditorUtility.OpenFolderPanel("选择文件路径", localPath_prop.stringValue, "");
                        if(!string.IsNullOrEmpty(path)){
                            localPath_prop.stringValue = path;
                        }
                    }
                }
                using (var hor = new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("[菜单名]:", GUILayout.Width(100));
                    menuName_prop.stringValue = EditorGUILayout.TextField(menuName_prop.stringValue);
                    EditorGUILayout.LabelField("(文件夹独立)");
                }
            }

        }
        private static Dictionary<string, LayerNode> LoadDicFromObj(ConfigBuildObj buildObj)
        {
            if (buildObj.needBuilds == null)
            {
                return null;
            }
            else
            {
                Dictionary<string, LayerNode> nodeDic = new Dictionary<string, LayerNode>();
                nodeDic.Add(rootFolder, new LayerNode(rootFolder));
                foreach (var item in buildObj.needBuilds)
                {
                    RetriveAddFolder(item.assetPath, nodeDic);
                    if (!nodeDic.ContainsKey(item.assetPath))
                    {
                        var path = AssetDatabase.GetAssetPath(item.obj);
                        nodeDic.Add(item.assetPath, new LayerNode(path));
                    }
                }
                return nodeDic;
            }
        }
        private static LayerNode LoadNodesFromDic(Dictionary<string, LayerNode> nodeDic)
        {
            if (nodeDic == null) return null;
            LayerNode root = nodeDic[rootFolder];
            foreach (var item in nodeDic)
            {
                item.Value.parent = null;
                item.Value.childs.Clear();
            }
            foreach (var item in nodeDic)
            {
                foreach (var child in nodeDic)
                {
                    if (child.Key.Contains(item.Key + "/") && !child.Key.Replace(item.Key + "/", "").Contains("/"))
                    {
                        item.Value.childs.Add(child.Value);
                        child.Value.parent = item.Value;
                        if (root == null || AssetDatabase.GetAssetPath(root.layer).Contains(item.Key))
                        {
                            root = item.Value;
                        }
                    }
                }
            }
            return root;

        }

        private static void RetriveAddFolder(string assetPath, Dictionary<string, LayerNode> nodeDic)
        {
            var folder = assetPath.Remove(assetPath.LastIndexOf("/"));
            if (folder != assetPath && !nodeDic.ContainsKey(folder))
            {
                nodeDic.Add(folder, new LayerNode(folder));
            }
            if (folder.Contains("/"))
            {
                RetriveAddFolder(folder, nodeDic);
            }
        }
        private static void StoreLayerNodeToAsset(Dictionary<string, LayerNode> nodeDic, ConfigBuildObj buildObj, bool selectedOnly = false)
        {
            foreach (var item in nodeDic)
            {
                if (!ProjectWindowUtil.IsFolder(item.Value.layer.GetInstanceID()))
                {
                    if (selectedOnly && !item.Value.selected)
                    {
                        continue;
                    }
                    var oitem = buildObj.needBuilds.Find(x => x.obj == item.Value.layer);
                    if (oitem == null)
                    {
                        buildObj.needBuilds.Add(new ConfigBuildObj.ObjectItem(item.Value.layer));
                    }
                }
            }
            ReFelsh(buildObj.needBuilds);
            EditorUtility.SetDirty(buildObj);
        }

        /// <summary>
        /// 遍历文件及子文件
        /// </summary>
        /// <param name="root"></param>
        /// <param name="OnRetrive"></param>
        private static void RetriveObject(string root, UnityAction<Object> OnRetrive)
        {
            var obj = AssetDatabase.LoadAssetAtPath<Object>(root);
            if (obj == null)
            {
                return;
            }
            else
            {
                OnRetrive(obj);
                if (ProjectWindowUtil.IsFolder(obj.GetInstanceID()))
                {
                    var files = System.IO.Directory.GetFiles(root);
                    foreach (var item in files)
                    {
                        if (!item.EndsWith(".meta"))
                        {
                            var path = item.Replace(Application.dataPath, "Assets");
                            RetriveObject(path, OnRetrive);
                        }
                    }
                    var folders = System.IO.Directory.GetDirectories(root);
                    foreach (var item in folders)
                    {
                        var path = item.Replace(Application.dataPath, "Assets");
                        RetriveObject(path, OnRetrive);
                    }
                }
            }

        }

        //绘制添加，删除,重置等功能         
        private void DrawHeadTools()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(new GUIContent("+", "从Project添加"), GUILayout.Width(20)))
                {
                    if (Selection.activeObject == null) return;

                    var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

                    AddGroupOfObject(assetPath);
                }
                if (GUILayout.Button(new GUIContent("-", "移除选中"), GUILayout.Width(20)))
                {
                    var selectedNode = new List<string>();
                    foreach (var item in nodeDic)
                    {
                        if (item.Value.selected && item.Key != rootFolder)
                        {
                            selectedNode.Add(item.Key);
                        }
                    }
                    foreach (var item in selectedNode)
                    {
                        nodeDic.Remove(item);
                    }
                    rootNode = LoadNodesFromDic(nodeDic);
                }
                //导入相关
                if (GUILayout.Button(new GUIContent("~", "导入关联资源"), GUILayout.Width(20)))
                {
                    var needAdd = new List<string>();
                    foreach (var item in nodeDic)
                    {
                        if (item.Value.selected)
                        {
                            var childs = AssetDatabase.GetDependencies(item.Value.assetPath, true);
                            foreach (var child in childs)
                            {
                                if (!needAdd.Contains(child))
                                {
                                    needAdd.Add(child);
                                }
                            }
                        }
                    }
                    foreach (var item in needAdd)
                    {
                        RetriveAddFolder(item, nodeDic);
                        if (!nodeDic.ContainsKey(item))
                        {
                            nodeDic.Add(item, new LayerNode(item));
                        }
                    }
                    rootNode = LoadNodesFromDic(nodeDic);
                }
                //选中所有引用
                if (GUILayout.Button(new GUIContent("&", "关联资源"), GUILayout.Width(20)))
                {
                    var needSelect = new List<string>();
                    foreach (var item in nodeDic)
                    {
                        if (item.Value.selected)
                        {
                            var childs = AssetDatabase.GetDependencies(item.Value.assetPath, true);
                            foreach (var child in childs)
                            {
                                if (!needSelect.Contains(child))
                                {
                                    needSelect.Add(child);
                                }
                            }
                        }
                    }
                    foreach (var item in nodeDic)
                    {
                        item.Value.Select((needSelect.Contains(item.Key)));
                    }
                }
                //刷新
                if (GUILayout.Button(new GUIContent("*", "刷新重置"), GUILayout.Width(20)))
                {
                    var buildObj = target as ConfigBuildObj;
                    nodeDic = LoadDicFromObj(buildObj);
                    rootNode = LoadNodesFromDic(nodeDic);
                }

                if (GUILayout.Button(new GUIContent("s","保存配制"), GUILayout.Width(20)))
                {
                    var buildObj = target as ConfigBuildObj;
                    buildObj.needBuilds.Clear();
                    StoreLayerNodeToAsset(nodeDic, buildObj);
                    EditorUtility.SetDirty(buildObj);
                }
            }
        }

        private void DrawData(LayerNode data)
        {
            if (data.content != null)
            {
                EditorGUI.indentLevel = data.indent;
                DrawGUIData(data);
            }
            if (data.isExpanded)
            {
                for (int i = 0; i < data.childs.Count; i++)
                {
                    LayerNode child = data.childs[i];
                    if (child.content != null)
                    {
                        EditorGUI.indentLevel = child.indent;
                        if (child.childs.Count > 0)
                        {
                            DrawData(child);
                        }
                        else
                        {
                            DrawGUIData(child);
                        }
                    }
                }
            }
        }
        private void DrawGUIData(LayerNode data)
        {
            GUIStyle style = "Label";
            var pointWidth = 10;
            Rect rt = GUILayoutUtility.GetRect(300, EditorGUIUtility.singleLineHeight);
            rt = new Rect(rt.x, rt.y, rt.width, EditorGUIUtility.singleLineHeight);
            var offset = (16 * EditorGUI.indentLevel);
            var srect = new Rect(rt.x + offset, rt.y, rt.width - offset - pointWidth, EditorGUIUtility.singleLineHeight);

            if (Event.current != null && srect.Contains(Event.current.mousePosition)
             && Event.current.button == 0 && Event.current.type <= EventType.mouseUp)
            {
                EditorGUIUtility.PingObject(data.layer.GetInstanceID());
            }

            var selected = EditorGUI.Toggle(srect, data.selected, style);
            if (selected != data.selected)
            {
                data.Select(selected);
            }

           
            if (data.selected)
            {
                EditorGUI.DrawRect(srect, new Color(1, 1, 1, 0.1f));
            }
            if (data.isFolder)
            {
                var btnRect = new Rect(rt.x + offset - pointWidth * 2, rt.y, pointWidth * 2, rt.height);
                if (GUI.Button(btnRect, data.isExpanded ? _groupOn : _groupff, style))
                {
                    data.Expland(!data.isExpanded);
                }
            }

            EditorGUI.LabelField(rt, data.content);
        }

        private void DrawBottomTools()
        {
            var buildObj = target as ConfigBuildObj;
            GUIStyle labStyle = EditorStyles.miniBoldLabel;
            GUILayoutOption labLayout = GUILayout.Width(100);
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("[目标平台]：", labStyle, labLayout);
                buildObj.buildTarget = (BuildTarget)EditorGUILayout.EnumPopup(buildObj.buildTarget);
            }
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("[打包选项]：", labStyle, labLayout);
                buildObj.buildOption = (BuildAssetBundleOptions)EditorGUILayout.EnumMaskField(buildObj.buildOption);
            }
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("[清空文件]：", labStyle, labLayout);
                buildObj.clearOld = EditorGUILayout.Toggle(buildObj.clearOld);
            }

            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("生成AB(全部)",EditorStyles.toolbarDropDown))
                {
                    if (buildObj.clearOld) FileUtil.DeleteFileOrDirectory(buildObj.LocalPath);

                    ConfigBuildObj bo = ScriptableObject.CreateInstance<ConfigBuildObj>();
                    StoreLayerNodeToAsset(nodeDic, bo);
                    ABBUtility.BuildGroupBundles(buildObj.LocalPath, GetBundleBuilds(buildObj.needBuilds), buildObj.buildOption, buildObj.buildTarget);
                }
                if (GUILayout.Button("生成AB(选中)", EditorStyles.toolbarDropDown))
                {
                    if (buildObj.clearOld) FileUtil.DeleteFileOrDirectory(buildObj.LocalPath);

                    ConfigBuildObj bo = ScriptableObject.CreateInstance<ConfigBuildObj>();
                    StoreLayerNodeToAsset(nodeDic, bo, true);
                    ABBUtility.BuildGroupBundles(buildObj.LocalPath, GetBundleBuilds(buildObj.needBuilds), buildObj.buildOption, buildObj.buildTarget);
                }
            }
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("[拷贝到路径]:", GUILayout.Width(100));
                targetPath_prop.stringValue = EditorGUILayout.TextField(targetPath_prop.stringValue);
                if (GUILayout.Button("选择"))
                {
                    var path = EditorUtility.OpenFolderPanel("选择文件路径", targetPath_prop.stringValue, "");
                    if(!string.IsNullOrEmpty(path))
                    {
                        targetPath_prop.stringValue = path;
                    }
                }
                if (GUILayout.Button("拷贝"))
                {
                    if (buildObj.LocalPath != buildObj.TargetPath && !string.IsNullOrEmpty(buildObj.TargetPath))
                    {
                        FileUtil.DeleteFileOrDirectory(buildObj.TargetPath);
                        FileUtil.CopyFileOrDirectory(buildObj.LocalPath, buildObj.TargetPath);
                    }
                }
            }
        }
        public static AssetBundleBuild[] GetBundleBuilds(List<ConfigBuildObj.ObjectItem> needBuilds)
        {
            Dictionary<string, AssetBundleBuild> bundleDic = new Dictionary<string, AssetBundleBuild>();
            foreach (var item in needBuilds)
            {
                if (!bundleDic.ContainsKey(item.assetBundleName))
                {
                    bundleDic.Add(item.assetBundleName, new AssetBundleBuild());
                }
                var asb = bundleDic[item.assetBundleName];

                asb.assetBundleName = item.assetBundleName;
                if (asb.assetNames == null) asb.assetNames = new string[0];
                List<string> assetNames = new List<string>(asb.assetNames);
                assetNames.Add(item.assetPath);
                asb.assetNames = assetNames.ToArray();

                bundleDic[item.assetBundleName] = asb;
            }

            List<AssetBundleBuild> builds = new List<AssetBundleBuild>(bundleDic.Values);
            return builds.ToArray();
        }

        public static void ReFelsh(List<ConfigBuildObj.ObjectItem> needBuilds)
        {
            var oldItems = needBuilds.ToArray();
            foreach (var item in oldItems)
            {
                if (!item.ReFelsh())
                {
                    needBuilds.Remove(item);
                    Debug.LogError("已经移除：" + item.assetPath);
                }
            }
        }
    }

}
