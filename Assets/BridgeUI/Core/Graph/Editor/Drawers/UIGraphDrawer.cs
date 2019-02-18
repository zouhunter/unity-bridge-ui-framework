using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AssetBundle = UnityEngine.AssetBundle;
using UnityEditorInternal;
using BridgeUI;
using BridgeUI.Graph;
using Graph = BridgeUI.Graph;
using Model = BridgeUI.Model;

namespace BridgeUI.Drawer
{
    [CustomEditor(typeof(Graph.UIGraph))]
    public class UIGraphDrawer : Editor
    {
        protected SerializedProperty script;
        protected SerializedProperty bridgesProp;
        protected SerializedProperty bundlesProp;
        protected SerializedProperty prefabsProp;
        protected SerializedProperty resourcesProp;
        protected SerializedProperty defultTypeProp;
        private string query;
        private SerializedProperty prefabsPropWorp;
        private SerializedProperty bundlesPropWorp;
        private SerializedProperty resourcesPropWorp;
        private UIInfoListDrawer prefabsList;
        private UIInfoListDrawer bundlesList;
        private UIInfoListDrawer resourcesList;
        private UIInfoListDrawer prefabsWorpList;
        private UIInfoListDrawer bundlesWorpList;
        private UIInfoListDrawer resourcesWorpList;
        protected const float widthBt = 20;
        protected const float padding = 5;

        protected string[] option = { "关联", "路径", "资源包" };

        public enum SortType
        {
            ByName = 0,
            ByLayer = 1
        }
        private SortType currSortType = SortType.ByName;

        private void OnEnable()
        {
            script = serializedObject.FindProperty("m_Script");
            bridgesProp = serializedObject.FindProperty("bridges");
            bundlesProp = serializedObject.FindProperty("b_nodes");
            prefabsProp = serializedObject.FindProperty("p_nodes");
            resourcesProp = serializedObject.FindProperty("r_nodes");
            defultTypeProp = serializedObject.FindProperty("loadType");
            var sobj = new SerializedObject(ScriptableObject.CreateInstance<Graph.UIGraph>());
            prefabsPropWorp = sobj.FindProperty("p_nodes");
            bundlesPropWorp = sobj.FindProperty("b_nodes");
            resourcesPropWorp = sobj.FindProperty("r_nodes");
            (target as UIGraph).ControllerType = typeof(BridgeUI.Drawer.BridgeUIGraphCtrl).FullName;
            QuickUpdateFromGraph();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawScript();
            using (var hor = new EditorGUILayout.HorizontalScope()){
                DrawOption();
                DrawMatchField();
                DrawToolButtons();
            }
            DrawRuntimeItems();
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DragGroupObj(Rect acceptRect, SerializedProperty prop)
        {
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    if (acceptRect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                    break;
                case EventType.DragPerform:
                    if (acceptRect.Contains(Event.current.mousePosition))
                    {
                        if (DragAndDrop.objectReferences.Length > 0)
                        {
                            var obj = DragAndDrop.objectReferences[0];
                            if (obj is NodeGraph.DataModel.NodeGraphObj)
                            {
                                var path = AssetDatabase.GetAssetPath(obj);
                                var guid = AssetDatabase.AssetPathToGUID(path);
                                var keyProp = prop.FindPropertyRelative("graphName");
                                var guidProp = prop.FindPropertyRelative("guid");
                                guidProp.stringValue = guid;
                                keyProp.stringValue = obj.name;
                            }
                        }
                        DragAndDrop.AcceptDrag();
                        Event.current.Use();
                    }
                    break;
            }
        }


        private void DrawScript()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(script);
            EditorGUI.EndDisabledGroup();
        }
        private void DrawOption()
        {
            EditorGUI.BeginChangeCheck();
            defultTypeProp.enumValueIndex = EditorGUILayout.Popup(defultTypeProp.enumValueIndex, option, EditorStyles.toolbarPopup,GUILayout.Width(100));// GUILayout.Toolbar(defultTypeProp.enumValueIndex, option, EditorStyles.toolbarButton);
            var changed = EditorGUI.EndChangeCheck();
            if(changed)
            {
                serializedObject.ApplyModifiedProperties();
                QuickUpdateFromGraph();
            }
        }
        private void DrawMatchField()
        {
            EditorGUI.BeginChangeCheck();
            query = EditorGUILayout.TextField(query);
            if (EditorGUI.EndChangeCheck())
            {
                MarchList();
            }
        }

        protected void DrawPrefabList()
        {
            if (prefabsList == null)
            {
                prefabsList = new UIInfoListDrawer("预制体");
                prefabsList.InitReorderList(prefabsProp);
            }
            prefabsList.DoLayoutList();
        }
        protected void DrawPrefabWorpList()
        {
            if (prefabsWorpList == null)
            {
                prefabsWorpList = new UIInfoListDrawer("预制体筛选");
                prefabsWorpList.InitReorderList(prefabsPropWorp);
            }
            prefabsWorpList.DoLayoutList();
        }
        protected void DrawBundleList()
        {
            if (bundlesList == null)
            {
                bundlesList = new UIInfoListDrawer("资源包");
                bundlesList.InitReorderList(bundlesProp);
            }
            bundlesList.DoLayoutList();
        }
        protected void DrawBundleWorpList()
        {
            if (bundlesWorpList == null)
            {
                bundlesWorpList = new UIInfoListDrawer("资源包筛选");
                bundlesWorpList.InitReorderList(bundlesPropWorp);
            }
            bundlesWorpList.DoLayoutList();
        }
        protected void DrawResourcesList()
        {
            if (resourcesList == null)
            {
                resourcesList = new UIInfoListDrawer("资源路径");
                resourcesList.InitReorderList(resourcesProp);
            }
            resourcesList.DoLayoutList();
        }
        protected void DrawResourcesWorpList()
        {
            if (resourcesWorpList == null)
            {
                resourcesWorpList = new UIInfoListDrawer("资源路径筛选");
                resourcesWorpList.InitReorderList(resourcesPropWorp);
            }
            resourcesWorpList.DoLayoutList();
        }

        protected virtual void DrawRuntimeItems()
        {
            switch (EnumIndexToLoadType(defultTypeProp.enumValueIndex))
            {
                case LoadType.DirectLink:
                    if (string.IsNullOrEmpty(query))
                    {
                        DrawPrefabList();
                    }
                    else
                    {
                        DrawPrefabWorpList();
                    }
                    break;
                case LoadType.AssetBundle:
                    if (string.IsNullOrEmpty(query))
                    {
                        DrawBundleList();
                    }
                    else
                    {
                        DrawBundleWorpList();
                    }
                    break;
                case LoadType.Resources:
                    if (string.IsNullOrEmpty(query))
                    {
                        DrawResourcesList();
                    }
                    else
                    {
                        DrawResourcesWorpList();
                    }
                    break;
                default:
                    break;
            }
        }
        private LoadType EnumIndexToLoadType(int index)
        {
            return (LoadType)(1 << index);
        }
        private void MarchList()
        {
            SerializedProperty property = null;
            SerializedProperty targetProperty = null;
            if (!string.IsNullOrEmpty(query))
            {
                switch (EnumIndexToLoadType(defultTypeProp.enumValueIndex))
                {
                    case LoadType.DirectLink:
                        property = prefabsProp;
                        targetProperty = prefabsPropWorp;
                        break;
                    case LoadType.AssetBundle:
                        property = bundlesProp;
                        targetProperty = bundlesPropWorp;
                        break;
                    case LoadType.Resources:
                        property = resourcesProp;
                        targetProperty = resourcesPropWorp;
                        break;
                    default:
                        break;
                }
                targetProperty.ClearArray();
                for (int i = 0; i < property.arraySize; i++)
                {
                    var assetNameProp = property.GetArrayElementAtIndex(i).FindPropertyRelative("panelName");
                    if (assetNameProp.stringValue.ToLower().Contains(query.ToLower()))
                    {
                        targetProperty.InsertArrayElementAtIndex(0);
                        BridgeEditorUtility.CopyPropertyValue(targetProperty.GetArrayElementAtIndex(0), property.GetArrayElementAtIndex(i));
                    }
                }
            }
        }
        private void DrawToolButtons()
        {
            var btnStyle = EditorStyles.miniButton;
            var widthSytle = GUILayout.Width(20);
            switch (EnumIndexToLoadType(defultTypeProp.enumValueIndex))
            {
                case LoadType.DirectLink:
                    using (var hor = new EditorGUILayout.HorizontalScope(widthSytle))
                    {
                        if (GUILayout.Button(new GUIContent("!", "排序"), btnStyle))
                        {
                            SortUIInfo(prefabsProp);
                        }
                        if (GUILayout.Button(new GUIContent("o", "批量加载"), btnStyle))
                        {
                            GroupLoadUIInfo(prefabsProp);
                        }
                        if (GUILayout.Button(new GUIContent("c", "批量关闭"), btnStyle))
                        {
                            CloseAllCreated(prefabsProp);
                        }
                    }
                    break;
                case LoadType.AssetBundle:
                    using (var hor = new EditorGUILayout.HorizontalScope(widthSytle))
                    {
                        if (GUILayout.Button(new GUIContent("!", "排序"), btnStyle))
                        {
                            SortUIInfo(bundlesProp);
                        }
                        if (GUILayout.Button(new GUIContent("o", "批量加载"), btnStyle))
                        {
                            GroupLoadUIInfo(bundlesProp);
                        }
                        if (GUILayout.Button(new GUIContent("c", "批量关闭"), btnStyle))
                        {
                            CloseAllCreated(bundlesProp);
                        }
                    }
                    break;
                case LoadType.Resources:
                    using (var hor = new EditorGUILayout.HorizontalScope(widthSytle))
                    {
                        if (GUILayout.Button(new GUIContent("!", "排序"), btnStyle))
                        {
                            SortUIInfo(resourcesProp);
                        }
                        if (GUILayout.Button(new GUIContent("o", "批量加载"), btnStyle))
                        {
                            GroupLoadUIInfo(resourcesProp);
                        }
                        if (GUILayout.Button(new GUIContent("c", "批量关闭"), btnStyle))
                        {
                            CloseAllCreated(resourcesProp);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 快速从graph更新
        /// </summary>
        private void QuickUpdateFromGraph()
        {
            var graph = target as Graph.UIGraph;
            NodeGraph.NodeGraphController controller = new BridgeUIGraphCtrl();
            controller.TargetGraph = graph;
            Selection.activeObject = graph;
            controller.BuildFromGraph(graph);
            EditorUtility.SetDirty(graph);
            if(EnumIndexToLoadType(defultTypeProp.enumValueIndex) == LoadType.AssetBundle){
                QuickUpdateBundles();
            }
        }
        private void GroupLoadUIInfo(SerializedProperty proprety)
        {
            for (int i = 0; i < proprety.arraySize; i++)
            {
                var itemProp = proprety.GetArrayElementAtIndex(i);
                GameObject prefab = null;
                var prefabProp = itemProp.FindPropertyRelative("prefab");
                var assetNameProp = itemProp.FindPropertyRelative("panelName");
                var instanceIDProp = itemProp.FindPropertyRelative("instanceID");

                if (instanceIDProp.intValue != 0) continue;

                if (prefabProp == null)
                {
                    var bundleNameProp = itemProp.FindPropertyRelative("bundleName");
                    var guidProp = itemProp.FindPropertyRelative("guid");
                    var paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleNameProp.stringValue, assetNameProp.stringValue);
                    if (paths.Length > 0)
                    {
                        prefab = AssetDatabase.LoadAssetAtPath<GameObject>(paths[0]);
                        guidProp.stringValue = AssetDatabase.AssetPathToGUID(paths[0]);
                    }
                }
                else
                {
                    prefab = prefabProp.objectReferenceValue as GameObject;
                }

                if (prefab == null)
                {
                    UnityEditor.EditorUtility.DisplayDialog("空对象", "找不到预制体" + assetNameProp.stringValue, "确认");
                }
                else
                {
                    GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

                    if (go.GetComponent<Transform>() is RectTransform)
                    {
                        var canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
                        go.transform.SetParent(canvas.transform, false);
                    }
                    else
                    {
                        go.transform.SetParent(null);
                    }

                    instanceIDProp.intValue = go.GetInstanceID();
                }

            }
        }

        private void QuickUpdateBundles()
        {
            for (int i = 0; i < bundlesProp.arraySize; i++)
            {
                var itemProp = bundlesProp.GetArrayElementAtIndex(i);
                UpdateOnLocalBundleInfo(itemProp);
            }
            UnityEditor.EditorUtility.SetDirty(this);

        }

        private void UpdateOnLocalBundleInfo(SerializedProperty itemProp)
        {
            var guidProp = itemProp.FindPropertyRelative("guid");
            var goodProp = itemProp.FindPropertyRelative("good");
            var assetNameProp = itemProp.FindPropertyRelative("panelName");
            var bundleNameProp = itemProp.FindPropertyRelative("bundleName");

            if (!goodProp.boolValue)
            {
                UnityEditor.EditorUtility.DisplayDialog("空对象", assetNameProp.stringValue + "信息错误", "确认");
            }
            else
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guidProp.stringValue);
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                UnityEditor.AssetImporter importer = UnityEditor.AssetImporter.GetAtPath(assetPath);
                assetNameProp.stringValue = obj.name;
                bundleNameProp.stringValue = importer.assetBundleName;
                //if (string.IsNullOrEmpty(bundleNameProp.stringValue))
                //{
                //    UnityEditor.EditorUtility.DisplayDialog("提示", "预制体" + assetNameProp.stringValue + "没有assetBundle标记", "确认");
                //}
            }
        }

        private void RemoveBundlesDouble(SerializedProperty property)
        {
            compair: List<string> temp = new List<string>();

            for (int i = 0; i < property.arraySize; i++)
            {
                var itemProp = property.GetArrayElementAtIndex(i);
                var assetNameProp = itemProp.FindPropertyRelative("panelName");
                if (!temp.Contains(assetNameProp.stringValue))
                {
                    temp.Add(assetNameProp.stringValue);
                }
                else
                {
                    property.DeleteArrayElementAtIndex(i);
                    goto compair;
                }
            }
        }

        private void RemoveBridgesDouble(SerializedProperty property)
        {
            compair: List<string> temp = new List<string>();

            for (int i = 0; i < property.arraySize; i++)
            {
                var itemProp = property.GetArrayElementAtIndex(i);
                var innode = itemProp.FindPropertyRelative("inNode");
                var outnode = itemProp.FindPropertyRelative("outNode");
                var key = innode.stringValue + outnode.stringValue;//输入和输出只有一组
                if (!temp.Contains(key))
                {
                    temp.Add(key);
                }
                else
                {
                    Debug.LogFormat("remove bridge:{0}-{1}", innode.stringValue, outnode.stringValue);
                    property.DeleteArrayElementAtIndex(i);
                    goto compair;
                }
            }
        }

        private void CloseAllCreated(SerializedProperty arrayProp)
        {
            TrySaveAllPrefabs(arrayProp);
            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                var item = arrayProp.GetArrayElementAtIndex(i);
                var instanceIDPorp = item.FindPropertyRelative("instanceID");
                BridgeEditorUtility.SavePrefab(instanceIDPorp);
            }
        }
        private void SortUIInfo(SerializedProperty property)
        {
            if (currSortType == SortType.ByName)
            {
                for (int i = 0; i < property.arraySize; i++)
                {
                    for (int j = i; j < property.arraySize - i - 1; j++)
                    {
                        var itemj = property.GetArrayElementAtIndex(j).FindPropertyRelative("panelName");
                        var itemj1 = property.GetArrayElementAtIndex(j + 1).FindPropertyRelative("panelName");
                        if (string.Compare(itemj.stringValue, itemj1.stringValue) > 0)
                        {
                            property.MoveArrayElement(j, j + 1);
                        }
                    }
                }
                //currSortType = SortType.ByLayer;
            }

            else if (currSortType == SortType.ByLayer)
            {
                for (int i = 0; i < property.arraySize; i++)
                {
                    for (int j = i; j < property.arraySize - i - 1; j++)
                    {
                        var itemj = property.GetArrayElementAtIndex(j).FindPropertyRelative("parentLayer");
                        var itemj1 = property.GetArrayElementAtIndex(j + 1).FindPropertyRelative("parentLayer");
                        if (itemj.intValue > itemj1.intValue)
                        {
                            property.MoveArrayElement(j, j + 1);
                        }
                    }
                }
                currSortType = SortType.ByName;
            }

        }

        private void TrySaveAllPrefabs(SerializedProperty arrayProp)
        {
            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                var item = arrayProp.GetArrayElementAtIndex(i);
                var instanceIDPorp = item.FindPropertyRelative("instanceID");
                var obj = EditorUtility.InstanceIDToObject(instanceIDPorp.intValue);
                if (obj == null) continue;
                var prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);
                if (prefab != null)
                {
                    var root = PrefabUtility.GetOutermostPrefabInstanceRoot((GameObject)prefab);
                    if (root != null)
                    {
                        var assetPath = AssetDatabase.GetAssetPath(root);
                        PrefabUtility.SaveAsPrefabAsset(obj as GameObject, assetPath);
                    }
                }
            }
        }

        protected bool AddArrayTools(Rect position, SerializedProperty property)
        {
            string path = property.propertyPath;
            int arrayInd = path.LastIndexOf(".Array");
            bool bIsArray = arrayInd >= 0;

            if (bIsArray)
            {
                SerializedObject so = property.serializedObject;
                string arrayPath = path.Substring(0, arrayInd);
                SerializedProperty arrayProp = so.FindProperty(arrayPath);

                //Next we need to grab the index from the path string
                int indStart = path.IndexOf("[") + 1;
                int indEnd = path.IndexOf("]");

                string indString = path.Substring(indStart, indEnd - indStart);

                int myIndex = int.Parse(indString);
                Rect rcButton = position;
                rcButton.height = EditorGUIUtility.singleLineHeight;
                rcButton.x = position.xMax - widthBt * 4;
                rcButton.width = widthBt;

                bool lastEnabled = GUI.enabled;

                if (myIndex == 0)
                    GUI.enabled = false;

                if (GUI.Button(rcButton, "^"))
                {
                    arrayProp.MoveArrayElement(myIndex, myIndex - 1);
                    so.ApplyModifiedProperties();

                }

                rcButton.x += widthBt;
                GUI.enabled = lastEnabled;
                if (myIndex >= arrayProp.arraySize - 1)
                    GUI.enabled = false;

                if (GUI.Button(rcButton, "v"))
                {
                    arrayProp.MoveArrayElement(myIndex, myIndex + 1);
                    so.ApplyModifiedProperties();
                }

                GUI.enabled = lastEnabled;

                rcButton.x += widthBt;
                if (GUI.Button(rcButton, "-"))
                {
                    arrayProp.DeleteArrayElementAtIndex(myIndex);
                    so.ApplyModifiedProperties();
                    return false;
                }

                rcButton.x += widthBt;
                if (GUI.Button(rcButton, "+"))
                {
                    arrayProp.InsertArrayElementAtIndex(myIndex);
                    so.ApplyModifiedProperties();
                }
            }
            return true;
        }
        public static Rect DrawBoxRect(Rect orignalRect, string index)
        {
            var idRect = new Rect(orignalRect.x - padding, orignalRect.y + padding, 20, 20);
            EditorGUI.LabelField(idRect, index.ToString());
            var boxRect = PaddingRect(orignalRect, padding * 0.5f);
            GUI.Box(boxRect, "");
            var rect = PaddingRect(orignalRect);
            return rect;
        }
        public static Rect PaddingRect(Rect orignalRect, float padding = padding)
        {
            var rect = new Rect(orignalRect.x + padding, orignalRect.y + padding, orignalRect.width - padding * 2, orignalRect.height - padding * 2);
            return rect;
        }
    }
}