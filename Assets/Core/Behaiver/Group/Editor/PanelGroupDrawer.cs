using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using UnityEngine.Serialization;
using BridgeUI;
using BridgeUI.Model;
using UnityEditorInternal;
namespace BridgeUIEditor
{
    using AssetBundle = UnityEngine.AssetBundle;
    public abstract class PanelGroupBaseDrawer : Editor
    {
        protected SerializedProperty script;
        protected SerializedProperty bridgesProp;
        //protected SerializedProperty groupObjsProp;
        protected SerializedProperty resetMenuProp;
        protected SerializedProperty menuProp;
        protected SerializedProperty bundlesProp;
        protected SerializedProperty prefabsProp;
        protected SerializedProperty graphListProp;
        protected SerializedProperty defultTypeProp;
        protected bool swink;
        private string query;
        private SerializedProperty prefabsPropWorp;
        private SerializedProperty bundlesPropWorp;
        private ReorderableList groupList;
        private ReorderableList prefabsList;
        private ReorderableList bundlesList;
        private ReorderableList prefabsWorpList;
        private ReorderableList bundlesWorpList;
        protected const float widthBt = 20;
        protected abstract bool drawScript { get; }
#if AssetBundleTools
        protected string[] option = { "预制体", "资源包" };
#else
    protected string[] option =  { "预制"};
#endif
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
            graphListProp = serializedObject.FindProperty("graphList");
            defultTypeProp = serializedObject.FindProperty("loadType");
            resetMenuProp = serializedObject.FindProperty("resetMenu");
            menuProp = serializedObject.FindProperty("menu");
            var sobj = new SerializedObject(ScriptableObject.CreateInstance<PanelGroupObj>());
            prefabsPropWorp = sobj.FindProperty("p_nodes");
            bundlesPropWorp = sobj.FindProperty("b_nodes");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (drawScript) DrawScript();
            DrawGroupList();
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                DrawOption();
                DrawToolButtons();
            }
            TryDrawMenu();
            DrawRuntimeItems();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGroupList()
        {
            if (groupList == null)
            {
                groupList = new ReorderableList(serializedObject, graphListProp, true, true, true, true);
                groupList.drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "界面配制图表");
                };
                groupList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var prop = graphListProp.GetArrayElementAtIndex(index);
                    var key = prop.FindPropertyRelative("graphName");
                    var guid = prop.FindPropertyRelative("guid");
                    var btnRect = new Rect(rect.x, rect.y + 2, rect.width - 30, EditorGUIUtility.singleLineHeight);

                    if (GUI.Button(btnRect, key.stringValue, EditorStyles.miniButton))
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid.stringValue);
                        if (!string.IsNullOrEmpty(path))
                        {
                            NodeGraph.DataModel.NodeGraphObj graph = AssetDatabase.LoadAssetAtPath<NodeGraph.DataModel.NodeGraphObj>(path);
                            AssetDatabase.OpenAsset(graph);
                        }
                    }
                    btnRect = new Rect(rect.x + rect.width - 30, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
                    DragGroupObj(btnRect, prop);

                    if (GUI.Button(btnRect, " ", EditorStyles.objectFieldMiniThumb))
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid.stringValue);
                        if (!string.IsNullOrEmpty(path))
                        {
                            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<NodeGraph.DataModel.NodeGraphObj>(path));
                        }
                    }
                };
            }
            groupList.DoLayoutList();
        }

        protected virtual void DragGroupObj(Rect acceptRect,SerializedProperty prop)
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
        private void TryDrawMenu()
        {
            if (defultTypeProp.enumValueIndex == 1 && resetMenuProp.boolValue)
            {
                EditorGUILayout.PropertyField(menuProp);
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
            defultTypeProp.enumValueIndex = EditorGUILayout.Popup(defultTypeProp.enumValueIndex, option, EditorStyles.toolbarPopup);// GUILayout.Toolbar(defultTypeProp.enumValueIndex, option, EditorStyles.toolbarButton);
        }
        protected void DrawListProperty(SerializedProperty listProperty, bool editable)
        {
            float height = 10 + EditorGUIUtility.singleLineHeight;
            var lastRect = GUILayoutUtility.GetLastRect();
            if (listProperty.arraySize == 0)
            {
                if (editable && GUILayout.Button("+", EditorStyles.miniLabel))
                {
                    listProperty.InsertArrayElementAtIndex(0);
                }
            }
            else
            {
                for (int i = 0; i < listProperty.arraySize; i++)
                {
                    var property = listProperty.GetArrayElementAtIndex(i);
                    if (editable)
                    {
                        var rect = GUILayoutUtility.GetRect(lastRect.width, height);
                        var good = AddArrayTools(rect, property);
                        if (good)
                        {
                            Rect rc = rect;
                            rc.width -= widthBt * 4;
                            EditorGUI.PropertyField(rc, property, true);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        var rect = GUILayoutUtility.GetRect(lastRect.width, EditorGUI.GetPropertyHeight(property, new GUIContent(), true));
                        EditorGUI.PropertyField(rect, property, true);
                    }

                }
            }

        }

        private void DrawMatchField(Rect rect)
        {
            EditorGUI.BeginChangeCheck();
            var fieldRect = new Rect(rect.x, rect.y + 2, rect.width, rect.height);
            query = EditorGUI.TextField(fieldRect, query,EditorStyles.miniTextField);
            if (EditorGUI.EndChangeCheck())
            {
                MarchList();
            }
        }
        protected void DrawPrefabList()
        {
            if (prefabsList == null)
            {
                prefabsList = new ReorderableList(serializedObject, prefabsProp,true,true,false,false);
                prefabsList.drawHeaderCallback = DrawElementHead;
                prefabsList.elementHeightCallback = (index) =>
                {
                    var prop = prefabsProp.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(prop);
                };
                prefabsList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var prop = prefabsProp.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, prop);
                };
            }
            prefabsList.DoLayoutList();
        }
        protected void DrawPrefabWorpList()
        {
            if (prefabsWorpList == null)
            {
                prefabsWorpList = new ReorderableList(serializedObject, prefabsPropWorp, true, true, false, false);
                prefabsWorpList.drawHeaderCallback = DrawElementHead;

                prefabsWorpList.elementHeightCallback = (index) =>
                {
                    var prop = prefabsPropWorp.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(prop);
                };
                prefabsWorpList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var prop = prefabsPropWorp.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, prop);
                };
            }
            prefabsWorpList.DoLayoutList();
        }
        protected void DrawBundleList()
        {
            if (bundlesList == null)
            {
                bundlesList = new ReorderableList(serializedObject, bundlesProp, true, true, false, false);
                bundlesList.drawHeaderCallback = DrawElementHead;
                bundlesList.elementHeightCallback = (index) =>
                {
                    var prop = bundlesProp.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(prop);
                };
                bundlesList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var prop = bundlesProp.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, prop);
                };
            }
            bundlesList.DoLayoutList();
        }
        protected void DrawBundleWorpList()
        {
            if (bundlesWorpList == null)
            {
                bundlesWorpList = new ReorderableList(serializedObject, bundlesPropWorp, true, true, false, false);
                bundlesWorpList.drawHeaderCallback = DrawElementHead;
                bundlesWorpList.elementHeightCallback = (index) =>
                {
                    var prop = bundlesPropWorp.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(prop);
                };
                bundlesWorpList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var prop = bundlesPropWorp.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, prop);
                };
            }
            bundlesWorpList.DoLayoutList();
        }

        protected void DrawElementHead(Rect rect)
        {
            EditorGUI.LabelField(rect,"[面板列表]");
            var surchField = new Rect(rect.x + rect.width * 0.7f, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
            DrawMatchField(surchField);
        }

        protected virtual void DrawRuntimeItems()
        {
            switch (EnumIndexToLoadType(defultTypeProp.enumValueIndex))
            {
                case LoadType.Prefab:
                    if (string.IsNullOrEmpty(query))
                    {
                        DrawPrefabList();
                    }
                    else
                    {
                        DrawPrefabWorpList();
                    }
                    break;
                case LoadType.Bundle:
                    if (string.IsNullOrEmpty(query))
                    {
                        DrawBundleList();
                    }
                    else
                    {
                        DrawBundleWorpList();
                    }
                    break;
                default:
                    break;
            }
        }

        protected virtual void DrawGraphItems()
        {
            GUI.backgroundColor = Color.yellow;
            EditorGUILayout.LabelField("界面配制图表", EditorStyles.helpBox);
            GUI.backgroundColor = Color.white;
            for (int i = 0; i < graphListProp.arraySize; i++)
            {
                var item = graphListProp.GetArrayElementAtIndex(i);
                var key = item.FindPropertyRelative("graphName");
                var guid = item.FindPropertyRelative("guid");
                if (GUILayout.Button(key.stringValue, EditorStyles.toolbarButton))
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid.stringValue);
                    if (!string.IsNullOrEmpty(path))
                    {
                        NodeGraph.DataModel.NodeGraphObj graph = AssetDatabase.LoadAssetAtPath<NodeGraph.DataModel.NodeGraphObj>(path);
                        AssetDatabase.OpenAsset(graph);
                    }
                }
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
                    case LoadType.Prefab:
                        property = prefabsProp;
                        targetProperty = prefabsPropWorp;
                        break;
                    case LoadType.Bundle:
                        property = bundlesProp;
                        targetProperty = bundlesPropWorp;
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
                case LoadType.Prefab:
                    using (var hor = new EditorGUILayout.HorizontalScope(widthSytle))
                    {
                        if (GUILayout.Button(new GUIContent("%", "移除重复"), btnStyle))
                        {
                            RemoveBundlesDouble(prefabsProp);
                            RemoveBridgesDouble(bridgesProp);
                        }
                        if (GUILayout.Button(new GUIContent("*", "快速更新"), btnStyle))
                        {
                            prefabsProp.ClearArray();
                            bridgesProp.ClearArray();
                            EditorApplication.delayCall += QuickUpdateFromGraph;
                        }
                        if (GUILayout.Button(new GUIContent("！", "排序"), btnStyle))
                        {
                            SortAllBundles(prefabsProp);
                        }
                        if (GUILayout.Button(new GUIContent("o", "批量加载"), btnStyle))
                        {
                            GroupLoadPrefabs(prefabsProp);
                        }
                        if (GUILayout.Button(new GUIContent("c", "批量关闭"), btnStyle))
                        {
                            CloseAllCreated(prefabsProp);
                        }
                    }
                    break;
                case LoadType.Bundle:
                    using (var hor = new EditorGUILayout.HorizontalScope(widthSytle))
                    {
                        resetMenuProp.boolValue = GUILayout.Toggle(resetMenuProp.boolValue, new GUIContent("r", "重设菜单"), btnStyle);

                        if (GUILayout.Button(new GUIContent("%", "移除重复"), btnStyle))
                        {
                            RemoveBundlesDouble(bundlesProp);
                            RemoveBridgesDouble(bridgesProp);
                        }
                        if (GUILayout.Button(new GUIContent("*", "快速更新"), btnStyle))
                        {
                            QuickUpdateBundles();
                            bundlesProp.ClearArray();
                            bridgesProp.ClearArray();
                            EditorApplication.delayCall += QuickUpdateFromGraph;
                        }
                        if (GUILayout.Button(new GUIContent("!", "排序"), btnStyle))
                        {
                            SortAllBundles(bundlesProp);
                        }
                        if (GUILayout.Button(new GUIContent("o", "批量加载"), btnStyle))
                        {
                            GroupLoadPrefabs(bundlesProp);
                        }
                        if (GUILayout.Button(new GUIContent("c", "批量关闭"), btnStyle))
                        {
                            CloseAllCreated(bundlesProp);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 绘制作快速导入的区域
        /// </summary>
        private void DrawAcceptRegion()
        {
            var rect = GUILayoutUtility.GetRect(new GUIContent("哈哈"), EditorStyles.toolbarButton);
            rect.y -= EditorGUIUtility.singleLineHeight;
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                    break;
                case EventType.DragPerform:
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        var objs = DragAndDrop.objectReferences;
                        for (int i = 0; i < objs.Length; i++)
                        {
                            var obj = objs[i];
                            switch ((LoadType)defultTypeProp.enumValueIndex)
                            {
                                case LoadType.Prefab:
                                    prefabsProp.InsertArrayElementAtIndex(prefabsProp.arraySize);
                                    var itemprefab = prefabsProp.GetArrayElementAtIndex(prefabsProp.arraySize - 1);
                                    itemprefab.FindPropertyRelative("prefab").objectReferenceValue = obj;
                                    break;
                                case LoadType.Bundle:
                                    bundlesProp.InsertArrayElementAtIndex(bundlesProp.arraySize);
                                    var itembundle = bundlesProp.GetArrayElementAtIndex(bundlesProp.arraySize - 1);
                                    var guidProp = itembundle.FindPropertyRelative("guid");
                                    var goodProp = itembundle.FindPropertyRelative("good");
                                    var path = AssetDatabase.GetAssetPath(obj);
                                    guidProp.stringValue = AssetDatabase.AssetPathToGUID(path);
                                    goodProp.boolValue = true;
                                    UpdateOnLocalBundleInfo(itembundle);
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 快速从graph更新
        /// </summary>
        private void QuickUpdateFromGraph()
        {
            for (int i = 0; i < graphListProp.arraySize; i++)
            {
                var guid = graphListProp.GetArrayElementAtIndex(i).FindPropertyRelative("guid").stringValue;
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var graph = AssetDatabase.LoadAssetAtPath<NodeGraph.DataModel.NodeGraphObj>(path);

                if (graph != null)
                {
                    NodeGraph.NodeGraphController controller = new BridgeUIGraphCtrl();
                    controller.TargetGraph = graph;
                    controller.Build();
                }
                else
                {
                    Debug.Log(graphListProp.GetArrayElementAtIndex(i) + ": Lost!  deleted!");
                    graphListProp.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
            }
        }
        private void GroupLoadPrefabs(SerializedProperty proprety)
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
                    //var rematrixProp = itemProp.FindPropertyRelative("reset");
                    GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    if (target is PanelGroup)
                    {
                        if (go.GetComponent<Transform>() is RectTransform)
                        {
                            go.transform.SetParent((target as PanelGroup).transform, false);
                        }
                        else
                        {
                            go.transform.SetParent((target as PanelGroup).transform, true);
                        }
                    }
                    else if (target is PanelGroupObj)
                    {
                        if (go.GetComponent<Transform>() is RectTransform)
                        {
                            var canvas = GameObject.FindObjectOfType<Canvas>();
                            go.transform.SetParent(canvas.transform, false);
                        }
                        else
                        {
                            go.transform.SetParent(null);
                        }
                    }

                    //if (rematrixProp.boolValue)
                    //{
                    //    go.transform.position = Vector3.zero;
                    //    go.transform.localRotation = Quaternion.identity;
                    //}

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
                    Debug.LogFormat("remove bridge:{0}-{1}", innode.stringValue,outnode.stringValue);
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
                var obj = EditorUtility.InstanceIDToObject(instanceIDPorp.intValue);
                if (obj != null)
                {
                    BridgeEditorUtility.ApplyPrefab(obj as GameObject);
                    DestroyImmediate(obj);
                }
                instanceIDPorp.intValue = 0;
            }
        }
        private void SortAllBundles(SerializedProperty property)
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
                var prefab = PrefabUtility.GetPrefabParent(obj);
                if (prefab != null)
                {
                    var root = PrefabUtility.FindPrefabRoot((GameObject)prefab);
                    if (root != null)
                    {
                        PrefabUtility.ReplacePrefab(obj as GameObject, root, ReplacePrefabOptions.ConnectToPrefab);
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
    }
    [CustomEditor(typeof(PanelGroup)), CanEditMultipleObjects]
    public class PanelGroupDrawer : PanelGroupBaseDrawer
    {
        protected override bool drawScript { get { return true; } }
    }

    [CustomEditor(typeof(PanelGroupObj)), CanEditMultipleObjects]
    public class PanelGroupObjDrawer : PanelGroupBaseDrawer
    {
        protected override bool drawScript { get { return false; } }
    }
}