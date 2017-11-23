using UnityEngine;
using UnityEngine.Events;
using Model = NodeGraph.DataModel.Version2;
using NodeGraph;
using System;
using UnityEditor;


public abstract class PanelNodeBase : Node, IPanelInfoHolder
{
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
            EditorGUILayout.LabelField("预制体:", EditorStyles.largeLabel);
            prefab = EditorGUILayout.ObjectField(prefab, typeof(GameObject), false) as GameObject;
        }
    }
    private void DrawInforamtion()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("窗体类型:", EditorStyles.largeLabel);
            nodeInfo.form = (UIFormType)EditorGUILayout.EnumPopup(nodeInfo.form);
        }
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("层级分类:", EditorStyles.largeLabel);
            nodeInfo.layer = (UILayerType)EditorGUILayout.EnumPopup(nodeInfo.layer);
        }
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("层级id:", EditorStyles.largeLabel);
            nodeInfo.layerIndex = EditorGUILayout.IntField(nodeInfo.layerIndex);
        }
    }
    private bool ChangeCheckField(UnityAction func)
    {
        EditorGUI.BeginChangeCheck();
        func.Invoke();
        return EditorGUI.EndChangeCheck();

    }
}