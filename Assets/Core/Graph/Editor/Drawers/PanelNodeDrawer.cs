using BridgeUI;
using BridgeUI.Model;
using BridgeUI.CodeGen;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
namespace BridgeUIEditor
{
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
        private MonoBehaviour _panelCompnent;
        private MonoBehaviour panelCompnent
        {
            get
            {
                if (_panelCompnent == null && panelNode.Info.prefab != null)
                {
                    _panelCompnent = panelNode.Info.prefab.GetComponent<MonoBehaviour>();
                }
                return _panelCompnent;
            }
        }
        private Editor panelDrawer;
        private ReorderableList preComponentList;
        private bool showRule;
        private Vector2 scrollPos;
        private string HeadInfo
        {
            get
            {
                return "Panel Node : record panel load type and other rule";
            }
        }
        private bool BindingAble
        {
            get
            {
                return panelCompnent is PanelBase;
            }
        }
        private string[] options = { "参数配制", "控件指定", "面板脚本", "显示效果" };
        private GenCodeRule rule { get { if (panelNode == null) return default(GenCodeRule); return panelNode.rule; } }
        private System.Collections.Generic.List<ComponentItem> components { get { if (panelNode == null) return null; return panelNode.components; } }
        private ComponentItemDrawer itemDrawer;

        private void OnEnable()
        {
            panelNode = target as PanelNodeBase;
            itemDrawer = new ComponentItemDrawer();
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
                preComponentList.drawHeaderCallback = DrawComponetHeader;
                //preComponentList.elementHeight = itemDrawer.singleLineHeight;
                preComponentList.showDefaultBackground = true;
                preComponentList.elementHeightCallback += (index) =>
                {
                    var prop = components[index];
                    return itemDrawer.GetItemHeight(prop, BindingAble);
                };
                preComponentList.drawElementCallback += (rect, index, isFocused, isActive) =>
                {
                    itemDrawer.DrawItemOnRect(rect, index, components[index], BindingAble);
                };
                preComponentList.drawElementBackgroundCallback += (rect, index, isFocused, isActive) =>
                {
                    if(components.Count > index && index >= 0)
                    {
                        itemDrawer.DrawBackground(rect, isFocused, components[index],BindingAble);
                    }
                };

            }

            if (components != null)
            {
                components.ForEach((x) =>
                {
                    if (x.target != null)
                    {
                        x.components = GenCodeUtil.SortComponent(x.target);
                    }
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
            EditorGUI.LabelField(rect, "[控件列表]");
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
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("←", "快速解析"), EditorStyles.toolbarButton, GUILayout.Width(20)))
                {
                    var component = nodeInfo.prefab.GetComponent<MonoBehaviour>();
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

            }

            //if (showRule)
            //{
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("BaseType:", GUILayout.Width(lableWidth));
                rule.baseTypeIndex = EditorGUILayout.Popup(rule.baseTypeIndex, GenCodeUtil.supportBaseTypes);
                if (GUILayout.Button(new GUIContent("update", "更新脚本控件信息"), EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    var go = nodeInfo.prefab;
                    GenCodeUtil.CreateScript(go, components, rule);
                }
            }

            //}

            if (preComponentList != null)
            {
                preComponentList.DoLayoutList();
            }

            var addRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);

            if (addRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.DragUpdated)
                {
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        foreach (var item in DragAndDrop.objectReferences)
                        {
                            if (item is GameObject || item is ScriptableObject)
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
                                if (parent)
                                {
                                    var c_item = new ComponentItem(parent as GameObject);
                                    c_item.components = GenCodeUtil.SortComponent(parent as GameObject);
                                    components.Add(c_item);
                                }
                            }

                            else if(item is ScriptableObject)
                            {
                                var c_item = new ComponentItem(item as ScriptableObject);
                                components.Add(c_item);
                                Debug.Log(c_item);
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

            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(new GUIContent("→", "快速绑定"), EditorStyles.toolbarButton, GUILayout.Width(20)))
                {
                    GenCodeUtil.BindingUI(nodeInfo.prefab, components);
                }
            }

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

}