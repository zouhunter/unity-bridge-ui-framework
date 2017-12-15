using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using NodeGraph.DataModel;
using UnityEditor;
using BridgeUI;
using BridgeUI.Model;
[CustomNodeGraphDrawer(typeof(PanelNode))]
public class PanelNodeDrawer : NodeDrawer
{
    protected const int lableWidth = 120;
    public NodeType nodeType = NodeType.Destroy | NodeType.Fixed | NodeType.HideGO | NodeType.NoAnim | NodeType.ZeroLayer;
    public int style = 1;
    protected GameObject prefab;
    protected NodeInfo nodeInfo { get { return (target as PanelNodeBase).nodeInfo; } }
    public override string ActiveStyle
    {
        get
        {
            return string.Format("node {0} on", style);
        }
    }
    public override string InactiveStyle
    {
        get
        {
            return string.Format("node {0}", style);
        }
    }

    public override string Category
    {
        get
        {
            return "panel";
        }
    }

    protected string HeadInfo
    {
        get
        {
            return "Panel Node : record panel load type and other rule";
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox(HeadInfo, MessageType.Info);
        LoadRecordIfEmpty();
        DrawNodeInfo();
        //if (prefab != null) node.Name = prefab.name;
    }

    protected virtual void DrawNodeInfo()
    {
        EditorGUILayout.HelpBox("[窗体信息配制:]", MessageType.Info);

        DrawHeadField();
        RecordPrefabInfo();
        DrawInforamtion();
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

    private void RecordPrefabInfo()
    {
        if (prefab != null)
        {
            var path = AssetDatabase.GetAssetPath(prefab);
            (target as PanelNodeBase).nodeInfo.prefabGuid = AssetDatabase.AssetPathToGUID(path);
        }
    }
    protected void DrawObjectFieldInternal()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("【预制体】:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            prefab = EditorGUILayout.ObjectField(prefab, typeof(GameObject), false) as GameObject;
        }
    }
    protected void DrawFormType()
    {
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
    protected void DrawAnim()
    {
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
    public override void OnContextMenuGUI(GenericMenu menu)
    {
        base.OnContextMenuGUI(menu);
    }

    protected void DrawInforamtion()
    {
        if ((nodeType & NodeType.Fixed) == 0)
        {
            DrawFormType();
        }
        else
        {
            nodeInfo.uiType.form = BridgeUI.UIFormType.Fixed;
        }

        if ((nodeType & NodeType.ZeroLayer) == 0)
        {
            DrawLayerType();
        }
        else
        {
            nodeInfo.uiType.layer = BridgeUI.UILayerType.Base;
            nodeInfo.uiType.layerIndex = 0;
        }

        if ((nodeType & NodeType.HideGO) == 0)
        {
            DrawHideAlaph();
            nodeInfo.uiType.hideRule = BridgeUI.HideRule.AlaphGameObject;
        }
        else
        {
            nodeInfo.uiType.hideRule = BridgeUI.HideRule.HideGameObject;
        }

        if ((nodeType & NodeType.Destroy) == 0)
        {
            DrawCloseRule();
        }
        else
        {
            nodeInfo.uiType.closeRule = BridgeUI.CloseRule.DestroyNoraml;
        }

        if ((nodeType & NodeType.NoAnim) == 0)
        {
            DrawAnim();
        }
        else
        {
            nodeInfo.uiType.enterAnim = BridgeUI.UIAnimType.NoAnim;
            nodeInfo.uiType.quitAnim = BridgeUI.UIAnimType.NoAnim;
        }
    }
    private void DrawToggleFromNodeType(NodeType model)
    {
        var on = (nodeType & model) == model;
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            var thison = GUILayout.Toggle(on, model.ToString(), EditorStyles.radioButton, GUILayout.Width(60));

            if (thison != on)
            {
                on = thison;
                if (on)
                {
                    nodeType |= model;
                }
                else
                {
                    nodeType &= ~model;
                }
            }
        }
    }
    protected void DrawHeadSelect()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            DrawToggleFromNodeType(NodeType.Fixed);
            DrawToggleFromNodeType(NodeType.ZeroLayer);
            DrawToggleFromNodeType(NodeType.HideGO);
            DrawToggleFromNodeType(NodeType.Destroy);
            DrawToggleFromNodeType(NodeType.NoAnim);
        }
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Style:");
            style = (int)EditorGUILayout.Slider(style, 1, 7);
        }
    }
    protected void DrawHeadField()
    {
        if (nodeType != 0)
        {
            DrawObjectFieldInternal();
        }
    }
}
