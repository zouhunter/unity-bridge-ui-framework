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

namespace BridgeUIEditor
{
    using AssetBundle = UnityEngine.AssetBundle;
    public abstract class PanelGroupBaseDrawer : Editor
    {
        protected SerializedProperty script;
        protected SerializedProperty graphListProp;
        protected SerializedProperty resetMenuProp;
        protected SerializedProperty menuProp;
        private string query;
        private GraphListDrawer graphList;
        protected const float widthBt = 20;
        protected abstract bool drawScript { get; }
        protected UIInfoListDrawer bundleInfoList;
        protected UIInfoListDrawer prefabInfoList;
        protected BridgeUI.Graph.UIGraph tempGraph;
        protected SerializedObject tempGraphObj;
#if AssetBundleTools
        protected string[] option = { "预制体", "资源包" };
#else
        protected string[] option =  { "预制"};
#endif
        protected int selected;

        private void OnEnable()
        {
            script = serializedObject.FindProperty("m_Script");
            graphListProp = serializedObject.FindProperty("graphList");
            resetMenuProp = serializedObject.FindProperty("resetMenu");
            menuProp = serializedObject.FindProperty("menu");
            tempGraph = ScriptableObject.CreateInstance<BridgeUI.Graph.UIGraph>();
            tempGraphObj = new SerializedObject(tempGraph);
            bundleInfoList = new UIInfoListDrawer("资源包");
            bundleInfoList.InitReorderList(tempGraphObj.FindProperty("b_nodes"));
            prefabInfoList = new UIInfoListDrawer("预制体");
            prefabInfoList.InitReorderList(tempGraphObj.FindProperty("p_nodes"));
            UpdateMarchList();
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
            TryDrawMenu();
            DrawRuntimeItems();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGroupList()
        {
            if (graphList == null)
            {
                graphList = new GraphListDrawer("界面配制图表");
                graphList.InitReorderList(graphListProp);
            }
            graphList.DoLayoutList();
        }

        private void TryDrawMenu()
        {
            if (resetMenuProp.boolValue)
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
            selected = GUILayout.Toolbar(selected, option, EditorStyles.toolbarButton,GUILayout.Width(200));// GUILayout.Toolbar(defultTypeProp.enumValueIndex, option, EditorStyles.toolbarButton);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateMarchList();
            }
        }

        private void DrawMatchField()
        {
            EditorGUI.BeginChangeCheck();
            query = EditorGUILayout.TextField(query);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateMarchList();
            }
        }

        protected virtual void DrawRuntimeItems()
        {
            tempGraphObj.Update();
            if (selected == 0)
            {
                prefabInfoList.DoLayoutList();
            }
            else if (selected == 1)
            {
                bundleInfoList.DoLayoutList();
            }
            tempGraphObj.ApplyModifiedProperties();
        }

        private void UpdateMarchList()
        {
            if (selected == 0)
            {
                var prefabs = GetPrefabUIInfos(query);
                tempGraph.p_nodes.Clear();
                tempGraph.p_nodes.AddRange(prefabs);
                EditorUtility.SetDirty(tempGraph);
                tempGraphObj.Update();
            }

            if (selected == 1)
            {
                var bundles = GetBundleUIInfos(query);
                tempGraph.b_nodes.Clear();
                tempGraph.b_nodes.AddRange(bundles);
                EditorUtility.SetDirty(tempGraph);
                tempGraphObj.Update();
            }
        }

        private void DrawToolButtons()
        {
            var btnStyle = EditorStyles.miniButton;
            var widthSytle = GUILayout.Width(20);
            if(selected == 0)
            {
                using (var hor = new EditorGUILayout.HorizontalScope(widthSytle))
                {
                    if (GUILayout.Button(new GUIContent("o", "批量加载"), btnStyle))
                    {
                        GroupLoadItems( (tempGraph.p_nodes).ToArray());
                    }
                    if (GUILayout.Button(new GUIContent("c", "批量关闭"), btnStyle))
                    {
                        CloseAllCreated((tempGraph.p_nodes).ToArray());
                    }
                }
            }
            else if(selected == 1)
            {
                using (var hor = new EditorGUILayout.HorizontalScope(widthSytle))
                {
                    resetMenuProp.boolValue = GUILayout.Toggle(resetMenuProp.boolValue, new GUIContent("r", "重设菜单"), btnStyle);

                    if (GUILayout.Button(new GUIContent("o", "批量加载"), btnStyle))
                    {
                        GroupLoadItems((tempGraph.b_nodes).ToArray());
                    }
                    if (GUILayout.Button(new GUIContent("c", "批量关闭"), btnStyle))
                    {
                        CloseAllCreated((tempGraph.b_nodes).ToArray());

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
                if(item is PrefabUIInfo)
                {
                    prefab = (item as PrefabUIInfo).prefab;
                }
                else if(item is BundleUIInfo)
                {
                    var guid = (item as BundleUIInfo).guid;
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
                var obj = EditorUtility.InstanceIDToObject(item.instanceID);
                if (obj != null)
                {
                    BridgeEditorUtility.ApplyPrefab(obj as GameObject);
                    DestroyImmediate(obj);
                }
                item.instanceID = 0;
            }
        }

        private void TrySaveAllPrefabs(UIInfoBase[] proprety)
        {
            for (int i = 0; i < proprety.Length; i++)
            {
                var item = proprety[i];
                var obj = EditorUtility.InstanceIDToObject(item.instanceID);
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

        protected abstract List<PrefabUIInfo> GetPrefabUIInfos(string fliter);
        protected abstract List<BundleUIInfo> GetBundleUIInfos(string fliter);
    }
    [CustomEditor(typeof(PanelGroup))]
    public class PanelGroupDrawer : PanelGroupBaseDrawer
    {
        protected override bool drawScript { get { return true; } }
        protected override List<BundleUIInfo> GetBundleUIInfos(string fliter)
        {
            var panelgroup = target as PanelGroup;
            var nodes = new List<BundleUIInfo>();
            foreach (var item in panelgroup.graphList)
            {
                nodes.AddRange(item.b_nodes);
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
            foreach (var item in panelgroup.graphList)
            {
                nodes.AddRange(item.p_nodes);
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
    }

    [CustomEditor(typeof(PanelGroupObj))]
    public class PanelGroupObjDrawer : PanelGroupBaseDrawer
    {
        protected override bool drawScript { get { return false; } }
        protected override List<BundleUIInfo> GetBundleUIInfos(string fliter)
        {
            var panelgroup = target as PanelGroupObj;
            var nodes = new List<BundleUIInfo>();
            foreach (var item in panelgroup.graphList){
                nodes.AddRange(item.b_nodes);
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
            foreach (var item in panelgroup.graphList)
            {
                nodes.AddRange(item.p_nodes);
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
    }
}