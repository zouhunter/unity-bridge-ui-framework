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
using System.Linq;

namespace BridgeUI.Drawer
{
    using AssetBundle = UnityEngine.AssetBundle;
    public abstract class PanelGroupBaseDrawer : Editor
    {
        protected SerializedProperty script;
        protected SerializedProperty graphListProp;
        protected SerializedProperty bundleCreateRuleProp;
        private string query;
        private GraphListDrawer graphList;
        protected const float widthBt = 20;
        protected abstract bool drawScript { get; }
        protected UIInfoListDrawer bundleInfoList;
        protected UIInfoListDrawer prefabInfoList;
        protected UIInfoListDrawer resourceInfoList;
        protected BridgeUI.Graph.UIGraph tempGraph;
        protected SerializedObject tempGraphObj;

        protected string[] option = { "关联", "路径", "资源包" };

        protected int selected;
        protected int selectedGraph = -1;

        private void OnEnable()
        {
            script = serializedObject.FindProperty("m_Script");
            graphListProp = serializedObject.FindProperty("graphList");
            bundleCreateRuleProp = serializedObject.FindProperty("bundleCreateRule");
            tempGraph = ScriptableObject.CreateInstance<BridgeUI.Graph.UIGraph>();
            tempGraphObj = new SerializedObject(tempGraph);
            bundleInfoList = new UIInfoListDrawer("资源包");
            bundleInfoList.InitReorderList(tempGraphObj.FindProperty("b_nodes"));
            prefabInfoList = new UIInfoListDrawer("直接关联");
            prefabInfoList.InitReorderList(tempGraphObj.FindProperty("p_nodes"));
            resourceInfoList = new UIInfoListDrawer("资源路径");
            resourceInfoList.InitReorderList(tempGraphObj.FindProperty("r_nodes"));
            UpdateMarchList();
            UpdateRuntimeInfo();
        }
        private void OnDisable()
        {
            UpdateRuntimeInfo();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (drawScript)
                DrawScript();
            DrawGroupList();
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                DrawOption();
                DrawToolButtons();
                DrawMatchField();
            }
            //TryDrawMenu();
            DrawRuntimeItems();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGroupList()
        {
            if (graphList == null)
            {
                graphList = new GraphListDrawer("界面配制图表");
                graphList.onSelectID += OnSelectGraphID;
                graphList.onChanged += OnGraphListChanged;
                graphList.InitReorderList(graphListProp);
            }
            graphList.DoLayoutList();
        }

        private void OnGraphListChanged()
        {
            UpdateRuntimeInfo();
            UpdateMarchList();
        }

        private void OnSelectGraphID(int arg0)
        {
            selectedGraph = arg0;
            UpdateMarchList();
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
            selected = GUILayout.Toolbar(selected, option, EditorStyles.toolbarButton, GUILayout.Width(200));// GUILayout.Toolbar(defultTypeProp.enumValueIndex, option, EditorStyles.toolbarButton);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateMarchList();
            }
        }

        private void DrawMatchField()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Refesh", EditorStyles.miniButtonRight, GUILayout.Width(60)))
                {
                    OnGraphListChanged();
                }

                EditorGUI.BeginChangeCheck();
                query = EditorGUILayout.TextField(query);
                if (EditorGUI.EndChangeCheck())
                {
                    UpdateMarchList();
                }
                
            }
          
        }

        protected virtual void DrawRuntimeItems()
        {
            tempGraphObj.Update();
            if (1 << selected == (int)LoadType.DirectLink)
            {
                prefabInfoList.DoLayoutList();
            }
            else if (1 << selected == (int)LoadType.AssetBundle)
            {
                var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
                DrawBundleCreateRule(rect, bundleCreateRuleProp);
                bundleInfoList.DoLayoutList();
            }
            else if (1 << selected == (int)LoadType.Resources)
            {
                resourceInfoList.DoLayoutList();
            }
            tempGraphObj.ApplyModifiedProperties();
        }

        private void DrawBundleCreateRule(Rect position, SerializedProperty property)
        {
            int labelWidth = 100;
            int btnWidth = 60;
            var rect0 = new Rect(position.x, position.y, labelWidth, position.height);
            var rect1 = new Rect(position.x + labelWidth, position.y, position.width - btnWidth - labelWidth, position.height);
            var rect2 = new Rect(position.x + position.width - btnWidth, position.y, btnWidth, position.height);
            EditorGUI.LabelField(rect0, "资源包加载规则");
            property.objectReferenceValue = EditorGUI.ObjectField(rect1, property.objectReferenceValue, typeof(BundleLoader), false);
            var path = property.propertyPath;
            var obj = property.serializedObject.targetObject;
            if (GUI.Button(rect2, "new", EditorStyles.miniButtonRight))
            {
                BridgeUI.Drawer.BundleUtil.CreateNewBundleCreateRule(x =>
                {
                    property = new SerializedObject(obj).FindProperty(path);
                    property.objectReferenceValue = x;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
        }


        private void UpdateMarchList()
        {
            if (1 << selected == (int)LoadType.DirectLink)
            {
                var prefabs = GetPrefabUIInfos(query);
                tempGraph.p_nodes.Clear();
                tempGraph.p_nodes.AddRange(prefabs);
                EditorUtility.SetDirty(tempGraph);
                tempGraphObj.Update();
            }

            if (1 << selected == (int)LoadType.AssetBundle)
            {
                var bundles = GetBundleUIInfos(query);
                tempGraph.b_nodes.Clear();
                tempGraph.b_nodes.AddRange(bundles);
                EditorUtility.SetDirty(tempGraph);
                tempGraphObj.Update();
            }

            if (1 << selected == (int)LoadType.Resources)
            {
                var resources = GetResourceUIInfos(query);
                tempGraph.r_nodes.Clear();
                tempGraph.r_nodes.AddRange(resources);
                EditorUtility.SetDirty(tempGraph);
                tempGraphObj.Update();
            }


        }

        private void DrawToolButtons()
        {
            var btnStyle = EditorStyles.miniButton;
            var widthSytle = GUILayout.Width(20);
            using (var hor = new EditorGUILayout.HorizontalScope(widthSytle))
            {
                if (GUILayout.Button(new GUIContent("o", "批量加载"), btnStyle))
                {
                    if (1 << selected == (int)LoadType.DirectLink)
                    {
                        GroupLoadItems((tempGraph.p_nodes).ToArray());
                    }
                    else if (1 << selected == (int)LoadType.AssetBundle)
                    {
                        GroupLoadItems((tempGraph.b_nodes).ToArray());
                    }
                    else if (1 << selected == (int)LoadType.Resources)
                    {
                        GroupLoadItems((tempGraph.r_nodes).ToArray());
                    }
                }

                if (GUILayout.Button(new GUIContent("c", "批量关闭"), btnStyle))
                {
                    if (1 << selected == (int)LoadType.DirectLink)
                    {
                        CloseAllCreated((tempGraph.p_nodes).ToArray());
                    }
                    else if (1 << selected == (int)LoadType.AssetBundle)
                    {
                        CloseAllCreated((tempGraph.b_nodes).ToArray());
                    }
                    else if (1 << selected == (int)LoadType.Resources)
                    {
                        CloseAllCreated((tempGraph.r_nodes).ToArray());
                    }
                }
            }
        }


        private void GroupLoadItems(UIInfoBase[] infoList)
        {
            for (int i = 0; i < infoList.Length; i++)
            {
                UIInfoBase item = infoList[i];
                GameObject prefab = null;
                if (item is PrefabUIInfo)
                {
                    prefab = (item as PrefabUIInfo).prefab;
                }
                else if (item is BundleUIInfo)
                {
                    var guid = (item as BundleUIInfo).guid;
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }
                else if (item is ResourceUIInfo)
                {
                    var guid = (item as ResourceUIInfo).guid;
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }

                if (item.instanceID != 0) continue;


                if (prefab == null)
                {
                    UnityEditor.EditorUtility.DisplayDialog("空对象", "找不到预制体" + item.panelName, "确认");
                }
                else
                {
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
                    else
                    {
                        if (go.GetComponent<Transform>() is RectTransform)
                        {
                            var canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
                            go.transform.SetParent(canvas.transform, false);
                        }
                        else
                        {
                            go.transform.SetParent(null);
                        }
                    }

                    item.instanceID = go.GetInstanceID();
                }

            }
        }

        private void CloseAllCreated(UIInfoBase[] infoList)
        {
            TrySaveAllPrefabs(infoList);
            for (int i = 0; i < infoList.Length; i++)
            {
                var item = infoList[i];
                BridgeEditorUtility.SavePrefab(ref item.instanceID);
            }
        }

        private void TrySaveAllPrefabs(UIInfoBase[] proprety)
        {
            for (int i = 0; i < proprety.Length; i++)
            {
                var item = proprety[i];
                var obj = EditorUtility.InstanceIDToObject(item.instanceID);
                if (obj == null) continue;
                var prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);
                if (prefab != null)
                {
                    var root = PrefabUtility.GetOutermostPrefabInstanceRoot((GameObject)prefab);
                    if (root != null)
                    {
                        var prefabPath = AssetDatabase.GetAssetPath(root);
                        bool success;
                        PrefabUtility.SaveAsPrefabAsset(obj as GameObject, prefabPath, out success);
                    }
                }
            }
        }

        protected abstract List<PrefabUIInfo> GetPrefabUIInfos(string fliter);
        protected abstract List<BundleUIInfo> GetBundleUIInfos(string fliter);
        protected abstract List<ResourceUIInfo> GetResourceUIInfos(string fliter);
        protected abstract void UpdateRuntimeInfo();//将信息解析到运行时代码
    }

    [CustomEditor(typeof(PanelGroup))]
    public class PanelGroupDrawer : PanelGroupBaseDrawer
    {
        protected override bool drawScript { get { return true; } }
        protected override List<BundleUIInfo> GetBundleUIInfos(string fliter)
        {
            var panelgroup = target as PanelGroup;
            var nodes = new List<BundleUIInfo>();

            if (selectedGraph == -1)
            {
                nodes.AddRange(panelgroup.B_nodes);
            }
            else
            {
                var graph = GetUIGraph(panelgroup, selectedGraph);
                if (graph != null)
                {
                    nodes.AddRange(graph.b_nodes);
                }
            }

            if (string.IsNullOrEmpty(fliter))
            {
                return nodes;
            }
            else
            {
                return nodes.FindAll(x => x.panelName.ToLower().Contains(fliter.ToLower()));
            }
        }
        protected override List<PrefabUIInfo> GetPrefabUIInfos(string fliter)
        {
            var panelgroup = target as PanelGroup;
            var nodes = new List<PrefabUIInfo>();

            if (selectedGraph == -1)
            {
                nodes.AddRange(panelgroup.P_nodes);
            }
            else
            {
                var graph = GetUIGraph(panelgroup, selectedGraph);
                if (graph != null)
                {
                    nodes.AddRange(graph.p_nodes);
                }
            }

            if (string.IsNullOrEmpty(fliter))
            {
                return nodes;
            }
            else
            {
                return nodes.FindAll(x => x.panelName.ToLower().Contains(fliter.ToLower()));
            }
        }
        protected override List<ResourceUIInfo> GetResourceUIInfos(string fliter)
        {
            var panelgroup = target as PanelGroup;
            var nodes = new List<ResourceUIInfo>();

            if (selectedGraph == -1)
            {
                nodes.AddRange(panelgroup.R_nodes);
            }
            else
            {
                var graph = GetUIGraph(panelgroup, selectedGraph);
                if (graph != null)
                {
                    nodes.AddRange(graph.r_nodes);
                }
            }

            if (string.IsNullOrEmpty(fliter))
            {
                return nodes;
            }
            else
            {
                return nodes.FindAll(x => x.panelName.ToLower().Contains(fliter.ToLower()));
            }
        }

        protected override void UpdateRuntimeInfo()
        {
            if (Application.isPlaying) return;
            //更新graph信息解析
            var panelgroup = target as PanelGroup;
            if (panelgroup != null)
            {
                panelgroup.Bridges.Clear();
                panelgroup.P_nodes.Clear();
                panelgroup.B_nodes.Clear();
                panelgroup.R_nodes.Clear();

                var graphGUIDs = panelgroup.GraphList;
                for (int i = 0; i < graphGUIDs.Count; i++)
                {
                    var graph = GetUIGraph(panelgroup, i);
                    if (graph != null)
                    {
                        if (graph.bridges != null) panelgroup.Bridges.AddRange(graph.bridges);
                        if (graph.p_nodes != null) panelgroup.P_nodes.AddRange(graph.p_nodes);
                        if (graph.b_nodes != null) panelgroup.B_nodes.AddRange(graph.b_nodes);
                        if (graph.r_nodes != null) panelgroup.R_nodes.AddRange(graph.r_nodes);
                        Debug.LogFormat("analysis graph : {0}, recoding complete.",graph.name);
                    }
                }
                EditorUtility.SetDirty(panelgroup);
            }
        }

        private BridgeUI.Graph.UIGraph GetUIGraph(PanelGroup panelgroup, int index)
        {
            var graphGUID = panelgroup.GetGraphAtIndex(index);
            if (!string.IsNullOrEmpty(graphGUID))
            {
                var graphPath = AssetDatabase.GUIDToAssetPath(graphGUID);
                if (!string.IsNullOrEmpty(graphPath))
                {
                    var graph = AssetDatabase.LoadAssetAtPath<BridgeUI.Graph.UIGraph>(graphPath);
                    return graph;
                }
            }
            return null;
        }
    }

    [CustomEditor(typeof(PanelGroupObj))]
    public class PanelGroupObjDrawer : PanelGroupBaseDrawer
    {
        protected override bool drawScript { get { return false; } }
        protected override List<BundleUIInfo> GetBundleUIInfos(string fliter)
        {
            var panelgroup = target as PanelGroupObj;
            var nodes = new List<BundleUIInfo>();
            for (int i = 0; i < panelgroup.graphList.Count; i++)
            {
                var item = panelgroup.graphList[i];
                if (item == null) continue;
                if (selectedGraph == -1 || selectedGraph == i)
                {
                    nodes.AddRange(item.b_nodes);
                }
            }
            if (string.IsNullOrEmpty(fliter))
            {
                return nodes;
            }
            else
            {
                return nodes.FindAll(x => x.panelName.ToLower().Contains(fliter.ToLower()));
            }
        }
        protected override List<PrefabUIInfo> GetPrefabUIInfos(string fliter)
        {
            var panelgroup = target as PanelGroupObj;
            var nodes = new List<PrefabUIInfo>();
            for (int i = 0; i < panelgroup.graphList.Count; i++)
            {
                var item = panelgroup.graphList[i];
                if (item == null) continue;
                if (selectedGraph == -1 || selectedGraph == i)
                {
                    nodes.AddRange(item.p_nodes);
                }
            }
            if (string.IsNullOrEmpty(fliter))
            {
                return nodes;
            }
            else
            {
                return nodes.FindAll(x => x.panelName.ToLower().Contains(fliter.ToLower()));
            }
        }
        protected override List<ResourceUIInfo> GetResourceUIInfos(string fliter)
        {
            var panelgroup = target as PanelGroupObj;
            var nodes = new List<ResourceUIInfo>();
            for (int i = 0; i < panelgroup.graphList.Count; i++)
            {
                var item = panelgroup.graphList[i];
                if (item == null) continue;
                if (selectedGraph == -1 || selectedGraph == i)
                {
                    nodes.AddRange(item.r_nodes);
                }
            }
            if (string.IsNullOrEmpty(fliter))
            {
                return nodes;
            }
            else
            {
                return nodes.FindAll(x => x.panelName.ToLower().Contains(fliter.ToLower()));
            }
        }

        protected override void UpdateRuntimeInfo()
        {

        }
    }
}