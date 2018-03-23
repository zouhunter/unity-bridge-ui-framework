using BridgeUI;
using BridgeUI.Model;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;
using UnityEngine.Events;

[CustomEditor(typeof(PanelNode))]
public class PanelNodeInfoDrawer : Editor
{
    private PanelNodeBase panelNode;
    public NodeType nodeType { get { return panelNode.nodeType; } set { panelNode.nodeType = value; } }
    private NodeInfo nodeInfo { get { if (panelNode == null) return null; return panelNode.nodeInfo; } }

    private const int lableWidth = 120;
    private PanelBase _panelCompnent;
    private PanelBase panelCompnent
    {
        get
        {
            if (_panelCompnent == null )
            {
                _panelCompnent = panelNode.Info.prefab.GetComponent<PanelBase>();
            }
            return _panelCompnent;
        }
    }
    private Editor panelDrawer;
    private ReorderableList preComponentList;
    private UICoder uiCoder;

    private string HeadInfo
    {
        get
        {
            return "Panel Node : record panel load type and other rule";
        }
    }
    private string[] options = {"参数配制","控件指定","面板脚本","显示效果" };
    private static int selected;

    private void OnEnable()
    {
        panelNode = target as PanelNodeBase;
        OnPrefabChanged();
        InitPanelNode();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawObjectField();
        SwitchDrawOption();
        serializedObject.ApplyModifiedProperties();
    }

    private void OnPrefabChanged()
    {
        if (nodeInfo.prefab != null)
        {
            panelNode.assetName = nodeInfo.prefab.name;
            uiCoder = GenCodeUtil.LoadUICoder(nodeInfo.prefab);
        }
    }

    protected virtual void InitPanelNode()
    {
        if (preComponentList == null)
        {
            preComponentList = new ReorderableList(panelNode.Info.components, typeof(ComponentItem));
            preComponentList.drawHeaderCallback = DrawComponetHeader;
            preComponentList.drawElementCallback = DrawComponetItem;
        }

        if(nodeInfo.components != null)
        {
            nodeInfo.components.ForEach((x) => { x.Update(); });
        }
    }

  
    private void DrawObjectField()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("预制体:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            EditorGUI.BeginChangeCheck();
            nodeInfo.prefab = EditorGUILayout.ObjectField(nodeInfo.prefab, typeof(GameObject), false) as GameObject;
            if(EditorGUI.EndChangeCheck())
            {
                OnPrefabChanged();
            }
        }
    }
    private void DrawComponetHeader(Rect rect)
    {
        rect = new Rect(rect.x + rect.width * 0.03f, rect.y, rect.width, rect.height);
        var height = EditorGUIUtility.singleLineHeight;
        var compRect = new Rect(rect.x, rect.y, rect.width * 0.2f, height);
        var nameRect = new Rect(rect.x + rect.width * 0.25f, rect.y, rect.width * 0.25f, height);
        var typeRect = new Rect(rect.x + rect.width * 0.5f, rect.y, rect.width * 0.5f, height);

        EditorGUI.LabelField(compRect, "对象");
        EditorGUI.LabelField(nameRect, "名称");
        EditorGUI.LabelField(typeRect, "组件");
    }

    private void DrawComponetItem(Rect rect, int index, bool isActive, bool isFocused)
    {
        var height = EditorGUIUtility.singleLineHeight;
        var item = nodeInfo.components[index];

        var compRect = new Rect(rect.x, rect.y, rect.width * 0.2f, height);
        var nameRect = new Rect(rect.x + rect.width * 0.25f, rect.y, rect.width * 0.25f, height);
        var typeRect = new Rect(rect.x + rect.width * 0.5f, rect.y, rect.width * 0.5f, height);

        EditorGUI.BeginChangeCheck();
        item.target = EditorGUI.ObjectField(compRect, item.target, item.componentType, true) as GameObject;
        if (EditorGUI.EndChangeCheck() && item.target)
        {
            var parent = PrefabUtility.GetPrefabParent(item.target);
            if (parent)
            {
                item.target = parent as GameObject;
            }
            if (string.IsNullOrEmpty(item.name))
            {
                item.name = item.target.name;
            }
            item.components = GenCodeUtil.SortComponent(item.target as GameObject);
        }


        EditorGUI.BeginChangeCheck();
        item.name = EditorGUI.TextField(nameRect, item.name);
        if (EditorGUI.EndChangeCheck() && item.target && !string.IsNullOrEmpty(item.name))
        {
            item.target.name = item.name;
        }

        if (item.components != null)
        {
            item.componentID = EditorGUI.Popup(typeRect, item.componentID, item.componentStrs);
        }
        
    }
      

    private void SwitchDrawOption()
    {
        selected = GUILayout.Toolbar(selected, options);
        if(selected == 0)
        {
            DrawHeadSelect();
            DrawInforamtion();
        }
        else if(selected == 1)
        {
            DrawPreComponents();
        }
        else if(selected == 2)
        {
            DrawPanelComponent();
        }
        else if(selected == 3)
        {
            DrawView();
        }
    }

    private void DrawPreComponents()
    {
        if (nodeInfo.prefab == null) return;

        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button(new GUIContent("e", "快速解析"), EditorStyles.miniButtonRight, GUILayout.Width(20)))
            {
                //从旧的脚本解析出
                GenCodeUtil.AnalysisComponent(nodeInfo.prefab, nodeInfo.components);
            }
            if (GUILayout.Button(new GUIContent("g", "生成脚本"), EditorStyles.miniButtonRight, GUILayout.Width(20)))
            {
                if(uiCoder != null)
                {
                    var go = nodeInfo.prefab;
                    GenCodeUtil.CreateComponent(go,nodeInfo.components,uiCoder);
                }
            }
            if (GUILayout.Button(new GUIContent("b", "快速绑定"), EditorStyles.miniButtonRight, GUILayout.Width(20)))
            {
                GenCodeUtil.BindingUI(nodeInfo.prefab,nodeInfo.components);
            }
        }
        preComponentList.DoLayoutList();
        var addRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);

        if (addRect.Contains(Event.current.mousePosition))
        {
            if (Event.current.type == EventType.DragUpdated)
            {
                if (DragAndDrop.objectReferences.Length > 0)
                {
                    foreach (var item in DragAndDrop.objectReferences)
                    {
                        if (item is GameObject)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                        }
                    }
                }
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                if (DragAndDrop.objectReferences.Length > 0)
                {
                    foreach (var item in DragAndDrop.objectReferences)
                    {
                        if (item is GameObject)
                        {
                            var obj = item as GameObject;
                            var parent = PrefabUtility.GetPrefabParent(obj);
                            Debug.Log(parent);
                            if(parent)
                            {
                                nodeInfo.components.Add(new ComponentItem(parent as GameObject));
                            }
                        }
                    }
                    DragAndDrop.AcceptDrag();
                }

            }
        }
        
    }

    private void DrawFormType()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("移动机制:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.uiType.form = (UIFormType)EditorGUILayout.EnumPopup(nodeInfo.uiType.form);
        }
    }
    private void DrawLayerType()
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

    private void DrawHideType()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("隐藏方式:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.uiType.hideRule = (HideRule)EditorGUILayout.EnumPopup(nodeInfo.uiType.hideRule);
        }
    }
    private void DrawHideAlaph()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("隐藏透明:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.uiType.hideAlaph = EditorGUILayout.Slider(nodeInfo.uiType.hideAlaph, 0, 1);
        }
    }
    private void DrawCloseRule()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("关闭方式:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            nodeInfo.uiType.closeRule = (CloseRule)EditorGUILayout.EnumPopup(nodeInfo.uiType.closeRule);
        }
    }
    private void DrawAnim()
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
    private void DrawInforamtion()
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
    private void DrawView()
    {
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
    private void DrawHeadSelect()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            DrawToggleFromNodeType(NodeType.Fixed);
            DrawToggleFromNodeType(NodeType.ZeroLayer);
            DrawToggleFromNodeType(NodeType.HideGO);
            DrawToggleFromNodeType(NodeType.Destroy);
            DrawToggleFromNodeType(NodeType.NoAnim);
        }
       
    }
    private void DrawPanelComponent()
    {
        DrawScritOptions();

        if (!panelCompnent) return;

        GUILayout.Space(5);

        if (panelDrawer == null && panelCompnent != null)
        {
            panelDrawer = UnityEditor.Editor.CreateEditor(panelCompnent);
        }

        if (panelDrawer != null)
        {
            panelDrawer.OnInspectorGUI();
        }
    }
    private void DrawScritOptions()
    {
        if (nodeInfo != null && nodeInfo.prefab)
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(new GUIContent("o","打开"), EditorStyles.miniButtonRight, GUILayout.Width(20)))
                {
                    if (panelNode.instenceID == 0)
                    {
                        Transform parent = null;
                        var group = FindObjectOfType<PanelGroup>();
                        if (group != null)
                        {
                            parent = group.GetComponent<Transform>();
                        }
                        else
                        {
                            var canvas = FindObjectOfType<Canvas>();
                            if (canvas != null)
                            {
                                parent = canvas.GetComponent<Transform>();
                            }
                        }
                        if (parent != null)
                        {
                            var obj = PrefabUtility.InstantiatePrefab(nodeInfo.prefab) as GameObject;
                            obj.transform.SetParent(parent, false);
                            panelNode.instenceID = obj.GetInstanceID();
                        }
                    }
                }
                if (GUILayout.Button(new GUIContent("c", "关闭"), EditorStyles.miniButtonRight, GUILayout.Width(20)))
                {
                    if (panelNode.instenceID != 0)
                    {
                        var obj = EditorUtility.InstanceIDToObject(panelNode.instenceID);
                        if (obj != null)
                        {
                            DestroyImmediate(obj);
                        }
                    }
                    panelNode.instenceID = 0;
                }

            }
        }
    }

}

