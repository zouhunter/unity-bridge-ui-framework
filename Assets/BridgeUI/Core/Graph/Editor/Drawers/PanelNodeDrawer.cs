using BridgeUI;
using BridgeUI.Model;
using BridgeUI.CodeGen;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using NodeGraph;
using NodeGraph.DataModel;

namespace BridgeUI.Drawer
{
    [CustomEditor(typeof(Graph.PanelNode))]
    public class PanelNodeInfoDrawer : Editor
    {
        private Graph.PanelNodeBase panelNode;
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
                if (_panelCompnent == null && !string.IsNullOrEmpty(panelNode.Info.guid))
                {
                    var path = AssetDatabase.GUIDToAssetPath(panelNode.Info.guid);
                    if (!string.IsNullOrEmpty(path))
                    {
                        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        GenCodeUtil.ChoiseAnUserMonobehiver(prefab, v => _panelCompnent = v);
                    }
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
        private string[] options = { "参数配制", "控件指定", "面板脚本", "辅助内容" };
        private System.Type[] supportedAnimPlayers;
        private GenCodeRule rule { get { if (panelNode == null) return default(GenCodeRule); return panelNode.rule; } }
        private System.Collections.Generic.List<ComponentItem> components { get { if (panelNode == null) return null; return panelNode.components; } }
        private ComponentItemDrawer itemDrawer;
        private StringListDrawer nodeProtListDrawer;
        private bool BindingAble
        {
            get
            {
                return typeof(PanelBase).IsAssignableFrom(typeof(PanelBase).Assembly.GetType(GenCodeUtil.supportBaseTypes[rule.baseTypeIndex]));
            }
        }

        private void OnEnable()
        {
            panelNode = target as Graph.PanelNodeBase;
            itemDrawer = new ComponentItemDrawer();
            InitPanelPortDrawer();
            InitAnimPlayers();
            OnPrefabChanged();
            InitComponentList();
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawObjectField();
            SwitchDrawOption();
            serializedObject.ApplyModifiedProperties();
        }
        private void InitPanelPortDrawer()
        {
            if (nodeProtListDrawer == null)
            {
                nodeProtListDrawer = new StringListDrawer();
                nodeProtListDrawer.InitReorderList(panelNode.nodedescribe, typeof(string));
            }
        }

        private void OnPrefabChanged()
        {
            var prefab = nodeInfo.GetPrefab();
            if (prefab != null)
            {
                panelNode.assetName = prefab.name;
            }
        }
        private void InitAnimPlayers()
        {
            supportedAnimPlayers = typeof(AnimPlayer).Assembly.GetTypes().Where(x => typeof(AnimPlayer).IsAssignableFrom(x) && !x.IsAbstract).ToArray();
        }

        protected virtual void InitComponentList()
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
                    if (components.Count > index && index >= 0)
                    {
                        itemDrawer.DrawBackground(rect, isFocused, components[index], BindingAble);
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
            if (panelNode.instenceID != 0)
            {
                var asset = EditorUtility.InstanceIDToObject(panelNode.instenceID);
                if (asset == null)
                {
                    panelNode.instenceID = 0;
                }
            }

            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("预制体:", GUILayout.Width(lableWidth));
                EditorGUI.BeginChangeCheck();
                nodeInfo.SetPrefabGUID(EditorGUILayout.ObjectField(nodeInfo.GetPrefab(), typeof(GameObject), false) as GameObject);
                if (EditorGUI.EndChangeCheck())
                {
                    OnPrefabChanged();
                }
                var btnName = panelNode.instenceID == 0 ? "打开" : "关闭";
                if (GUILayout.Button(btnName, EditorStyles.miniButtonRight, GUILayout.Width(lableWidth)))
                {
                    ToggleOpen();
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
            if (nodeInfo.GetPrefab() == null) return;

            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("←", "快速解析"), EditorStyles.toolbarButton, GUILayout.Width(20)))
                {
                    GenCodeUtil.ChoiseAnUserMonobehiver(nodeInfo.GetPrefab(), component =>
                    {
                        if (component == null)
                        {
                            EditorApplication.Beep();
                        }
                        else
                        {
                            //从旧的脚本解析出
                            GenCodeUtil.AnalysisComponent(component, components,rule);
                        }
                    });

                }

            }

            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("BaseType:", GUILayout.Width(lableWidth));
                rule.baseTypeIndex = EditorGUILayout.Popup(rule.baseTypeIndex, GenCodeUtil.supportBaseTypes);
                if (GUILayout.Button(new GUIContent("update", "更新脚本控件信息"), EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    var go = nodeInfo.GetPrefab();
                    GenCodeUtil.UpdateScripts(go, components, rule);
                }
            }

            if (preComponentList != null)
            {
                preComponentList.DoLayoutList();
            }

            var addRect = GUILayoutUtility.GetRect(BridgeUI.Drawer.BridgeEditorUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);

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
                                var parent = PrefabUtility.GetPrefabObject(obj);
                                if (parent)
                                {
                                    obj = parent as GameObject;
                                }
                                var c_item = new ComponentItem(obj);
                                c_item.components = GenCodeUtil.SortComponent(obj);
                                components.Add(c_item);
                            }
                            else if (item is ScriptableObject)
                            {
                                var c_item = new ComponentItem(item as ScriptableObject);
                                components.Add(c_item);
                            }
                        }
                        DragAndDrop.AcceptDrag();
                    }

                }
            }


        }
        string[] formTypes;
        string[] formTypesNotice = { "固定窗口(只能打开单个)", "可拖拽(可以打开多个小窗体)", "没有关闭按扭(只有场景跳转时关闭)" };
        int formSelected;

        private void DrawFormType()
        {
            if (formTypes == null)
            {
                formTypes = System.Enum.GetNames(typeof(UIFormType));
            }

            formSelected = System.Array.IndexOf(formTypes, nodeInfo.uiType.form.ToString());

            for (int i = 0; i < formTypes.Length; i++)
            {
                var isOn = EditorGUILayout.ToggleLeft(string.Format("{0}--{1}", formTypes[i], formTypesNotice[i]), formSelected == i);
                if (isOn)
                {
                    nodeInfo.uiType.form = (UIFormType)i;
                    formSelected = i;
                }
            }
        }

        string[] layerTypes;
        string[] layerTypesNotice = { "最低层，可以被任何界面复盖", "自动关闭的小提示（自动关，所以可以高点）", "用于程序状态警告（不可以被其他层级掩盖）" };
        int layerSelected;
        private void DrawLayerType()
        {
            if (layerTypes == null)
            {
                layerTypes = System.Enum.GetNames(typeof(UILayerType));
            }

            layerSelected = System.Array.IndexOf(layerTypes, nodeInfo.uiType.layer.ToString());

            for (int i = 0; i < layerTypes.Length; i++)
            {
                var isOn = EditorGUILayout.ToggleLeft(string.Format("{0}--{1}", layerTypes[i], layerTypesNotice[i]), layerSelected == i);
                if (isOn)
                {
                    nodeInfo.uiType.layer = (UILayerType)i;
                    layerSelected = i;
                }
            }
            DrawLayerIndex();
        }
        private void DrawLayerIndex()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                nodeInfo.uiType.layerIndex = (int)EditorGUILayout.Slider(nodeInfo.uiType.layerIndex, 0, 100);
                EditorGUILayout.LabelField("(相对)", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
            }
        }

        string[] closeRule;
        string[] closeRuleNotice = { "普通销毁", "快速销毁(从内存中清除)", "延迟(一帧)销毁", "隐藏(缓存文件),仅当场景切换时销毁" };
        int closeRuleSelected;
        private void DrawCloseRule()
        {
            if (closeRule == null)
            {
                closeRule = System.Enum.GetNames(typeof(CloseRule));
            }
            closeRuleSelected = System.Array.IndexOf(closeRule, nodeInfo.uiType.closeRule.ToString());

            for (int i = 0; i < closeRule.Length; i++)
            {
                var isOn = EditorGUILayout.ToggleLeft(string.Format("{0}--{1}", closeRule[i], closeRuleNotice[i]), closeRuleSelected == i);
                if (isOn)
                {
                    nodeInfo.uiType.closeRule = (CloseRule)i;
                    closeRuleSelected = i;
                }
            }
        }

        string[] hideRule;
        string[] hideRuleNotice = { "直接隐藏自身", "隐藏自己的可见物体" };
        int hideRuleSelected;
        private void DrawHideType()
        {
            if (hideRule == null)
            {
                hideRule = System.Enum.GetNames(typeof(HideRule));
            }

            hideRuleSelected = System.Array.IndexOf(hideRule, nodeInfo.uiType.hideRule.ToString());

            for (int i = 0; i < hideRule.Length; i++)
            {
                var isOn = EditorGUILayout.ToggleLeft(string.Format("{0}--{1}", hideRule[i], hideRuleNotice[i]), hideRuleSelected == i);
                if (isOn)
                {
                    nodeInfo.uiType.hideRule = (HideRule)i;
                    hideRuleSelected = i;
                }
            }
            if (nodeInfo.uiType.hideRule == HideRule.AlaphGameObject)
            {
                DrawHideAlaph();
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

        private void DrawAnim()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("出场动画:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
                nodeInfo.uiType.enterAnim = EditorGUILayout.ObjectField(nodeInfo.uiType.enterAnim, typeof(AnimPlayer), false) as AnimPlayer;
                if (GUILayout.Button("new", GUILayout.Width(60)))
                {
                    SelectAnimPlayer((x) => nodeInfo.uiType.enterAnim = x);
                }
            }
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("关闭动画:", EditorStyles.largeLabel, GUILayout.Width(lableWidth));
                nodeInfo.uiType.quitAnim = EditorGUILayout.ObjectField(nodeInfo.uiType.quitAnim, typeof(AnimPlayer), false) as AnimPlayer;
                if (GUILayout.Button("new", GUILayout.Width(60)))
                {
                    SelectAnimPlayer((x) => nodeInfo.uiType.quitAnim = x);
                }
            }
        }
        private void SelectAnimPlayer(UnityAction<AnimPlayer> onSelect)
        {
            if (supportedAnimPlayers == null || supportedAnimPlayers.Count() == 0) return;

            var options = supportedAnimPlayers.Select(x => new GUIContent(x.FullName)).ToArray();
            EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.zero), options, -1, (x, optionKeys, index) =>
            {
                if (index < options.Length)
                {
                    var type = supportedAnimPlayers[index];
                    var anim = ScriptableObject.CreateInstance(type) as AnimPlayer;
                    onSelect(anim);
                    ProjectWindowUtil.CreateAsset(anim, "new " + options[index] + ".asset");
                }

            }, null);
        }

        private bool ChangeCheckField(UnityAction func)
        {
            EditorGUI.BeginChangeCheck();
            func.Invoke();
            return EditorGUI.EndChangeCheck();

        }
        private void DrawInforamtion()
        {
            DrawOption("窗体类型", DrawFormType);
            DrawOption("层级类型", DrawLayerType);
            DrawOption("关闭规则", DrawCloseRule);
            DrawOption("隐藏规则", DrawHideType);
            DrawOption("动画状态", DrawAnim);
        }

        private void DrawOption(string label, UnityAction body)
        {
            DrawTitleRegion(label);
            using (var ver = new EditorGUILayout.VerticalScope())
            {
                body.Invoke();
            }
        }

        private void DrawTitleRegion(string label)
        {
            EditorGUILayout.LabelField("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
            var rect = GUILayoutUtility.GetRect(BridgeUI.Drawer.BridgeEditorUtility.currentViewWidth, EditorGUIUtility.singleLineHeight * 1.1f);
            GUI.color = Color.gray;
            GUI.Box(rect, "", EditorStyles.miniButton);
            GUI.color = Color.white;
            EditorGUI.LabelField(rect, string.Format("【{0}】", label), EditorStyles.largeLabel);
        }

        private string[] styleOptions = {  "blue", "green", "yellow", "orange", "flesh", "pink", "purple", "cyan" };
        private void DrawView()
        {
            DrawTitleRegion("板式:");
            DrawViewFormat();

            DrawTitleRegion("说明:");
            if (panelNode != null)
            {
                panelNode.Info.discription = EditorGUILayout.TextArea(panelNode.Info.discription, GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));
            }

            DrawTitleRegion("子面板:");
            if (nodeProtListDrawer != null)
            {
                nodeProtListDrawer.DoLayoutList();
            }
        }

        private void DrawViewFormat()
        {
            var xCount = 4;
            var width = (EditorGUIUtility.currentViewWidth - 20) / xCount;
            using (var vertical = new EditorGUILayout.VerticalScope())
            {
                for (int i = 0; i < styleOptions.Length; i += xCount)
                {
                    using (var hor = new EditorGUILayout.HorizontalScope())
                    {
                        for (int j = i; j < i + xCount && j < styleOptions.Length; j++)
                        {
                            var isOn = GUILayout.Toggle(j == panelNode.style - 1, new GUIContent(styleOptions[j]), EditorStyles.radioButton, GUILayout.Width(width));
                            if (isOn)
                            {
                                panelNode.style  = j + 1;
                            }
                        }
                    }
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
                    GenCodeUtil.BindingUI(panelCompnent, components);
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
                    var obj = PrefabUtility.InstantiatePrefab(nodeInfo.GetPrefab()) as GameObject;
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