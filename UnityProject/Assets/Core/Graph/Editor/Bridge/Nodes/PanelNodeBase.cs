using UnityEngine;
using UnityEngine.Events;
using Model = NodeGraph.DataModel.Version2;
using NodeGraph;
using System;
using UnityEditor;


public abstract class PanelNodeBase : Node, IPanelInfoHolder
{
    private const int lableWidth = 120;
    public NodeInfo nodeInfo = new NodeInfo();
    private GameObject prefab;
    public NodeInfo Info
    {
        get
        {
            return nodeInfo;
        }
    }
    protected abstract string HeadInfo { get; }
    public override Node Clone(Model.NodeData newData)
    {
        return newData.Operation.Clone();
    }

    public override void OnInspectorGUI(NodeGUI node, NodeGUIEditor editor, Action onValueChanged)
    {
        EditorGUILayout.HelpBox(HeadInfo, MessageType.Info);
        editor.UpdateNodeName(node);
        LoadRecordIfEmpty();
        DrawNodeInfo(node, onValueChanged);
    }

    protected virtual void DrawNodeInfo(NodeGUI node, Action onValueChanged)
    {
        EditorGUILayout.HelpBox("[窗体信息配制:]", MessageType.Info);

        if (ChangeCheckField(DrawObjectField))
        {
            RecordPrefabInfo(node);
            onValueChanged.Invoke();
        }

        if (ChangeCheckField(DrawInforamtion))
        {
            onValueChanged.Invoke();
        }
    }

    protected virtual void LoadRecordIfEmpty()
    {
        if (prefab == null && !string.IsNullOrEmpty(nodeInfo.prefabGuid))
        {
            var path = AssetDatabase.GUIDToAssetPath(nodeInfo.prefabGuid);
            if (!string.IsNullOrEmpty(path))
            {
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            else
            {
                nodeInfo.prefabGuid = null;
            }
        }
    }

    private void RecordPrefabInfo(NodeGUI node)
    {
        if (prefab != null)
        {
            node.Name = prefab.name;
            var path = AssetDatabase.GetAssetPath(prefab);
            nodeInfo.prefabGuid = AssetDatabase.AssetPathToGUID(path);
        }
    }

    private void DrawObjectField()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("【预制体】:", EditorStyles.largeLabel,GUILayout.Width(lableWidth));
            prefab = EditorGUILayout.ObjectField(prefab, typeof(GameObject), false) as GameObject;
        }
    }
    private void DrawInforamtion()
    {
        //using (var hor = new EditorGUILayout.HorizontalScope())
        //{
        //    EditorGUILayout.LabelField("唯一关键字:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
        //    nodeInfo.mutexKey = EditorGUILayout.TextField(nodeInfo.mutexKey);
        //}
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("可移动机制:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.form = (UIFormType)EditorGUILayout.EnumPopup(nodeInfo.form);
        }
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("绝对显示层:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.layer = (UILayerType)EditorGUILayout.EnumPopup(nodeInfo.layer);
        }
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("相对优先级:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.layerIndex = EditorGUILayout.IntField(nodeInfo.layerIndex);
        }
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("隐藏透明度:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.hideLuceny = (UILucenyType)EditorGUILayout.EnumPopup(nodeInfo.hideLuceny);
        }
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("出场动画组:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.animType = (UIAnimType)EditorGUILayout.EnumPopup(nodeInfo.animType);
        }

    }
    private bool ChangeCheckField(UnityAction func)
    {
        EditorGUI.BeginChangeCheck();
        func.Invoke();
        return EditorGUI.EndChangeCheck();

    }
}