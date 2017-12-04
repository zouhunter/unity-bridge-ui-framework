using UnityEngine;
using UnityEngine.Events;
using Model = NodeGraph.DataModel.Version2;
using NodeGraph;
using System;
using UnityEditor;
using BridgeUI.Model;
using BridgeUI;

public abstract class PanelNodeBase : Node, IPanelInfoHolder
{
    protected const int lableWidth = 120;
    protected GameObject prefab;
    public NodeInfo nodeInfo = new NodeInfo();
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
        DrawHeadSelect();

        EditorGUILayout.HelpBox("[窗体信息配制:]", MessageType.Info);
        
        if (ChangeCheckField(DrawHeadField))
        {
            RecordPrefabInfo(node);
            onValueChanged.Invoke();
        }

        if (ChangeCheckField(DrawInforamtion))
        {
            onValueChanged.Invoke();
        }
    }

    protected abstract void DrawHeadSelect();

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
    protected abstract void DrawHeadField();
    protected abstract void DrawInforamtion();
    protected void DrawObjectFieldInternal()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("【预制体】:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            prefab = EditorGUILayout.ObjectField(prefab, typeof(GameObject), false) as GameObject;
        }
    }
    protected void DrawFormType() {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("移动机制:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.uiType.form = (UIFormType)EditorGUILayout.EnumPopup(nodeInfo.uiType.form);
        }
    }
    protected void DrawLayerType()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("绝对显示:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.uiType.layer = (UILayerType)EditorGUILayout.EnumPopup(nodeInfo.uiType.layer);
        }
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("相对优先:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.uiType.layerIndex = EditorGUILayout.IntField(nodeInfo.uiType.layerIndex);
        }
    }
    protected void DrawHideType()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("隐藏方式:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.uiType.hideRule = (HideRule)EditorGUILayout.EnumPopup(nodeInfo.uiType.hideRule);
        }
    }
    protected void DrawHideAlaph()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("隐藏透明:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.uiType.hideAlaph = EditorGUILayout.Slider(nodeInfo.uiType.hideAlaph, 0, 1);
        }
    }
    protected void DrawCloseRule()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("关闭方式:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.uiType.closeRule = (CloseRule)EditorGUILayout.EnumPopup(nodeInfo.uiType.closeRule);
        }
    }
    protected void DrawAnim() { 
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("出场动画:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.uiType.enterAnim = (UIAnimType)EditorGUILayout.EnumPopup(nodeInfo.uiType.enterAnim);
        }
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("关闭动画:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.uiType.quitAnim = (UIAnimType)EditorGUILayout.EnumPopup(nodeInfo.uiType.quitAnim);
        }
    }

    private bool ChangeCheckField(UnityAction func)
    {
        EditorGUI.BeginChangeCheck();
        func.Invoke();
        return EditorGUI.EndChangeCheck();

    }
}