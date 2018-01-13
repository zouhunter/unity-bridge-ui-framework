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

[CustomEditor(typeof(PanelNode))]
public class PanelNodeInfoDrawer : Editor
{
    protected GameObject prefab;
    protected PanelNodeBase panelNode;
    protected const int lableWidth = 120;

    public NodeType nodeType { get { return panelNode.nodeType; } set { panelNode.nodeType = value; } }
    private PanelBase _panelCompnent;
    protected NodeInfo nodeInfo { get { return panelNode.nodeInfo; } }
    private bool showComponent;
    protected PanelBase panelCompnent
    {
        get
        {
            if (_panelCompnent == null && prefab != null)
            {
                _panelCompnent = prefab.GetComponent<PanelBase>();
            }
            return _panelCompnent;
        }
    }
    protected Editor panelDrawer;
    protected string HeadInfo
    {
        get
        {
            return "Panel Node : record panel load type and other rule";
        }
    }
    private void OnEnable()
    {
        panelNode = target as PanelNodeBase;
        LoadPrefabInfo();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawObjectFieldInternal();
        DrawShowHide();
        DrawPanelBase();

        EditorGUILayout.HelpBox(HeadInfo, MessageType.Info);
        DrawHeadSelect();

        EditorGUILayout.HelpBox("[窗体信息配制:]", MessageType.Info);
        DrawInforamtion();

        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void LoadPrefabInfo()
    {
        if (prefab == null && !string.IsNullOrEmpty(nodeInfo.prefabGuid))
        {
            var path = AssetDatabase.GUIDToAssetPath(nodeInfo.prefabGuid);
            if (!string.IsNullOrEmpty(path))
            {
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    panelNode.assetName = prefab.name;
                }
            }
            else
            {
                nodeInfo.prefabGuid = null;
            }
        }
    }

    private void RecordPrefabInfo()
    {
        if (prefab != null && panelNode != null)
        {
            var path = AssetDatabase.GetAssetPath(prefab);
            panelNode.nodeInfo.prefabGuid = AssetDatabase.AssetPathToGUID(path);
            panelNode.assetName = prefab.name;
        }
    }
    protected void DrawObjectFieldInternal()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("【预制体】:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            EditorGUI.BeginChangeCheck();
            prefab = EditorGUILayout.ObjectField(prefab, typeof(GameObject), false) as GameObject;
            if (EditorGUI.EndChangeCheck() && prefab != null)
            {
                RecordPrefabInfo();
            }
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
            panelNode.style = (int)EditorGUILayout.Slider(panelNode.style, 1, 7);
        }
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("说明:");
            if (panelNode != null)
            {
                panelNode.description = EditorGUILayout.TextField(panelNode.description);
            }
        }
    }
    protected void DrawShowHide()
    {
        if (prefab != null && panelNode != null)
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {

                if (GUILayout.Button("o", GUILayout.Width(20)))
                {
                    if (panelNode.instenceID == 0)
                    {
                        Transform parent = null;
                        var group = GameObject.FindObjectOfType<PanelGroup>();
                        if (group != null)
                        {
                            parent = group.GetComponent<Transform>();
                        }
                        else
                        {
                            var canvas = GameObject.FindObjectOfType<Canvas>();
                            if (canvas != null)
                            {
                                parent = canvas.GetComponent<Transform>();
                            }
                        }
                        if (parent != null)
                        {
                            var obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                            obj.transform.SetParent(parent, false);
                            panelNode.instenceID = obj.GetInstanceID();
                        }
                    }
                }
                if (GUILayout.Button("c", GUILayout.Width(20)))
                {
                    if (panelNode.instenceID != 0)
                    {
                        var obj = EditorUtility.InstanceIDToObject(panelNode.instenceID);
                        if (obj != null)
                        {
                            GameObject.DestroyImmediate(obj);
                        }
                    }
                    panelNode.instenceID = 0;
                }
                if (GUILayout.Button("Script", EditorStyles.toolbarButton))
                {
                    showComponent = !showComponent;
                }
            }
        }
    }
    protected void DrawPanelBase()
    {
        if (!showComponent) return;

        GUILayout.Space(5);

        if (panelDrawer == null && panelCompnent != null)
        {
            panelDrawer = UnityEditor.Editor.CreateEditor(panelCompnent);
        }

        if (panelDrawer != null)
        {
            panelDrawer.DrawHeader();
            panelDrawer.OnInspectorGUI();
        }
    }

}

[CustomNodeGraphDrawer(typeof(PanelNode))]
public class PanelNodeDrawer : NodeDrawer
{
    protected PanelNodeBase panelNode;
    public override Node target
    {
        get
        {
            return base.target;
        }

        set
        {
            base.target = value;
            panelNode = value as PanelNodeBase;
        }
    }
    public override int Style
    {
        get
        {
            return panelNode.style;
        }
    }
    public override string Category
    {
        get
        {
            return "panel";
        }
    }
    public override float CustomNodeHeight
    {
        get
        {
            if (panelNode != null && !string.IsNullOrEmpty(panelNode.description))
            {
                return EditorGUIUtility.singleLineHeight + 5;
            }
            return 0;
        }
    }
    public override void OnInspectorGUI(NodeGUI gui)
    {
        base.OnInspectorGUI(gui);
        if (target != null)
        {
            gui.Name = (target as PanelNode).assetName;
        }
    }
    public override void OnNodeGUI(Rect position, NodeData data)
    {
        base.OnNodeGUI(position, data);
        if (panelNode != null && !string.IsNullOrEmpty(panelNode.description))
        {
            var rect = new Rect(position.x + 20, position.y, position.width - 40, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, panelNode.description);
        }
    }
    public override void OnClickNodeGUI(NodeGUI nodeGUI, Vector2 mousePosition, ConnectionPointData result)
    {
        base.OnClickNodeGUI(nodeGUI, mousePosition, result);
        if (panelNode == null) return;
        GameObject prefab = null;
        var nodeInfo = panelNode.nodeInfo;
        if (!string.IsNullOrEmpty(nodeInfo.prefabGuid))
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
        if (prefab != null)
        {
            EditorGUIUtility.PingObject(prefab);
        }
    }
}
