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
    private int selected
    {
        get { return panelNode.selected; }
        set { panelNode.selected = value; }
    }
    private const int lableWidth = 60;
    private PanelBase _panelCompnent;
    private PanelBase panelCompnent
    {
        get
        {
            if (_panelCompnent == null && panelNode.Info.prefab != null)
            {
                _panelCompnent = panelNode.Info.prefab.GetComponent<PanelBase>();
            }
            return _panelCompnent;
        }
    }
    private Editor panelDrawer;
    private ReorderableList preComponentList;
    private bool showRule;
    private Color fieldColor = new Color(0.1f, 0.1f, 0.1f, 0.1f);

    private string HeadInfo
    {
        get
        {
            return "Panel Node : record panel load type and other rule";
        }
    }
    private string[] options = { "参数配制", "控件指定", "面板脚本", "显示效果" };
    private GenCodeRule rule { get { if (panelNode == null) return default(GenCodeRule); return panelNode.rule; } }
    private System.Collections.Generic.List<ComponentItem> components { get { if (panelNode == null) return null; return panelNode.components; } }
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
            //uiCoder = GenCodeUtil.LoadUICoder(nodeInfo.prefab, rule);
        }
    }

    protected virtual void InitPanelNode()
    {
        if (preComponentList == null)
        {
            preComponentList = new ReorderableList(components, typeof(ComponentItem));
            preComponentList.elementHeight = 3f * EditorGUIUtility.singleLineHeight;
            preComponentList.drawHeaderCallback = DrawComponetHeader;
            preComponentList.drawElementCallback = DrawComponetItem;
        }

        if (components != null)
        {
            components.ForEach((x) =>
            {
                x.components = GenCodeUtil.SortComponent(x.target);
            });
        }
    }


    private void DrawObjectField()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("预制体:", EditorStyles.toolbarButton, GUILayout.Width(lableWidth)))
            {
                ToggleOpen();
            }
            EditorGUI.BeginChangeCheck();
            nodeInfo.prefab = EditorGUILayout.ObjectField(nodeInfo.prefab, typeof(GameObject), false) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                OnPrefabChanged();
            }
        }
    }
    private void DrawComponetHeader(Rect rect)
    {
        var innerRect = new Rect(rect.x + 40, rect.y, rect.width - 40, rect.height);
        var height = EditorGUIUtility.singleLineHeight;
        var nameRect = new Rect(innerRect.x, innerRect.y, innerRect.width * 0.28f, height);
        var sourceRect = new Rect(innerRect.x + innerRect.width * 0.3f, innerRect.y, innerRect.width * 0.7f, height);
        EditorGUI.LabelField(rect, "[列表]");
        EditorGUI.LabelField(nameRect, "对象{Target}");
        EditorGUI.LabelField(sourceRect, "源{Source}");
    }

    private void DrawComponetItem(Rect rect, int index, bool isActive, bool isFocused)
    {
        var padding = 3;
        var innerRect1 = new Rect(rect.x + padding, rect.y + padding, rect.width - 2 * padding, rect.height - 2 * padding);
        GUI.color = fieldColor;
        GUI.Box(innerRect1, "");
        GUI.color = Color.white;
        padding = 5;
        var innerRect = new Rect(rect.x + padding + 20, rect.y + padding, rect.width - 2 * padding - 20, rect.height - 2 * padding);

        var height = EditorGUIUtility.singleLineHeight;
        var item = components[index];
        var indexRect = new Rect(rect.x + padding, rect.y + EditorGUIUtility.singleLineHeight, 20, EditorGUIUtility.singleLineHeight);
        var nameRect = new Rect(innerRect.x, innerRect.y, innerRect.width * 0.28f, height);
        var sourceRect = new Rect(innerRect.x + innerRect.width * 0.3f, innerRect.y, innerRect.width * 0.7f, height);
        var compRect = new Rect(nameRect.x, nameRect.y + EditorGUIUtility.singleLineHeight + 5, nameRect.width, nameRect.height);
        var typeRect = new Rect(sourceRect.x, sourceRect.y + EditorGUIUtility.singleLineHeight + 5, sourceRect.width, sourceRect.height);
        sourceRect.width *= 0.5f;
        var bindingRect = new Rect(sourceRect.x + sourceRect.width, sourceRect.y, sourceRect.width, sourceRect.height);

        EditorGUI.LabelField(indexRect, index.ToString());

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

        item.sourceName = EditorGUI.TextField(sourceRect, item.sourceName);

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

        item.binding = EditorGUI.ToggleLeft(bindingRect, "binding", item.binding);
    }


    private void SwitchDrawOption()
    {
        selected = GUILayout.Toolbar(selected, options);
        if (selected == 0)
        {
            DrawInforamtion();
        }
        else if (selected == 1)
        {
            DrawPreComponents();
        }
        else if (selected == 2)
        {
            DrawPanelComponent();
        }
        else if (selected == 3)
        {
            DrawView();
        }
    }

    private void DrawPreComponents()
    {
        if (nodeInfo.prefab == null) return;

        using (var hor = new EditorGUILayout.HorizontalScope())
        {

            showRule = GUILayout.Toggle(showRule, new GUIContent("⇆", "生成脚本"), EditorStyles.miniButton);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("←", "快速解析"), EditorStyles.miniButton))
            {
                var component = nodeInfo.prefab.GetComponent<PanelBase>();
                if (component == null)
                {
                    EditorApplication.Beep();
                }
                else
                {
                    //从旧的脚本解析出
                    GenCodeUtil.AnalysisComponent(component, components);
                }
               
            }
            if (GUILayout.Button(new GUIContent("→", "快速绑定"), EditorStyles.miniButton))
            {
                GenCodeUtil.BindingUI(nodeInfo.prefab, components);
            }
        }

        if (showRule)
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("BaseType:", GUILayout.Width(lableWidth));
                rule.baseTypeIndex = EditorGUILayout.Popup(rule.baseTypeIndex, GenCodeUtil.supportBaseTypes);
            }

            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("update", "更新脚本控件信息"), EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    var go = nodeInfo.prefab;
                    var uiCoder = GenCodeUtil.LoadUICoder(go, rule);
                    GenCodeUtil.CreateScript(go, components, uiCoder, rule);
                }
            }

        }

        if (preComponentList != null) preComponentList.DoLayoutList();
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
                            if (parent)
                            {
                                var c_item = new ComponentItem(parent as GameObject);
                                c_item.components = GenCodeUtil.SortComponent(target as GameObject);
                                components.Add(c_item);
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
        DrawOption("窗体类型", () =>
         {
             DrawToggleFromNodeType(NodeType.Fixed, nodeInfo.uiType.form.ToString());
         }, () =>
         {
             if ((nodeType & NodeType.Fixed) == 0)
             {
                 DrawFormType();
             }
             else
             {
                 nodeInfo.uiType.form = BridgeUI.UIFormType.Fixed;
             }
         });


        DrawOption("层级类型", () =>
         {
             DrawToggleFromNodeType(NodeType.ZeroLayer, nodeInfo.uiType.layer.ToString());
         }, () =>
         {
             if ((nodeType & NodeType.ZeroLayer) == 0)
             {
                 DrawLayerType();
             }
             else
             {
                 nodeInfo.uiType.layer = BridgeUI.UILayerType.Base;
                 nodeInfo.uiType.layerIndex = 0;
             }
         });


        DrawOption("层级ID", () =>
        {
            DrawToggleFromNodeType(NodeType.HideGO, nodeInfo.uiType.hideRule.ToString());
        }, () =>
        {
            if ((nodeType & NodeType.HideGO) == 0)
            {
                DrawHideAlaph();
                nodeInfo.uiType.hideRule = BridgeUI.HideRule.AlaphGameObject;
            }
            else
            {
                nodeInfo.uiType.hideRule = BridgeUI.HideRule.HideGameObject;
            }
        });


        DrawOption("关闭规则", () =>
        {
            DrawToggleFromNodeType(NodeType.Destroy, nodeInfo.uiType.closeRule.ToString());
        }, () =>
        {
            if ((nodeType & NodeType.Destroy) == 0)
            {
                DrawCloseRule();
            }
            else
            {
                nodeInfo.uiType.closeRule = BridgeUI.CloseRule.DestroyNoraml;
            }
        });


        DrawOption("动画状态", () =>
        {
            DrawToggleFromNodeType(NodeType.NoAnim, nodeInfo.uiType.enterAnim.ToString());
        }, () =>
        {
            if ((nodeType & NodeType.NoAnim) == 0)
            {
                DrawAnim();
            }
            else
            {
                nodeInfo.uiType.enterAnim = BridgeUI.UIAnimType.NoAnim;
                nodeInfo.uiType.quitAnim = BridgeUI.UIAnimType.NoAnim;
            }
        });
    }

    private void DrawOption(string label, UnityAction head, UnityAction body)
    {
        EditorGUILayout.LabelField("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
        EditorGUILayout.LabelField(string.Format("【{0}】", label), EditorStyles.miniBoldLabel);
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            head.Invoke();
            using (var ver = new EditorGUILayout.VerticalScope())
            {
                body.Invoke();
            }
        }
        GUILayout.Space(20);
    }

    private void DrawToggleFromNodeType(NodeType model, string title)
    {
        var on = (nodeType & model) == model;
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            var thison = GUILayout.Toggle(on, title, EditorStyles.radioButton, GUILayout.Width(60));

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
    private void DrawPanelComponent()
    {
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
    /// <summary>
    /// 切换面板的打开状态
    /// </summary>
    private void ToggleOpen()
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
        else
        {
            var obj = EditorUtility.InstanceIDToObject(panelNode.instenceID);
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
            panelNode.instenceID = 0;
        }

    }

}

