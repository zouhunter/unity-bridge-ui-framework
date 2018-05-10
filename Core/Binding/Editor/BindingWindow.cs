using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using BridgeUI;
using BridgeUI.CodeGen;
using BridgeUI.Binding;
using UnityEditorInternal;

namespace BridgeUIEditor
{
    public class BindingWindow : EditorWindow
    {

        [UnityEditor.Callbacks.OnOpenAsset()]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var prefab = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (prefab != null && prefab.GetComponent<RectTransform>())
            {
                GenCodeUtil.ChoiseAnUserMonobehiver(prefab, v => {
                    if(v)
                    {
                        var window = GetWindow<BindingWindow>();
                        window.OpenWith(v);
                    }
                });
                return true;
            }
            return false;
        }
        private GameObject prefab;
        private const int lableWidth = 60;
        private static int instenceID;
        private int selected;
        private string[] options = { "控件管理", "检视面板" };
        private MonoBehaviour panelCompnent;
        private Editor panelDrawer;
        private GenCodeRule rule = new GenCodeRule();
        private System.Collections.Generic.List<ComponentItem> components = new List<ComponentItem>();
        private ReorderableList preComponentList;
        private ComponentItemDrawer itemDrawer = new ComponentItemDrawer();
        private bool BindingAble
        {
            get
            {
                return typeof(PanelBase).IsAssignableFrom(typeof(PanelBase).Assembly.GetType(GenCodeUtil.supportBaseTypes[rule.baseTypeIndex]));
            }
        }
        private Vector2 scrollPos;
        private void OpenWith(MonoBehaviour behaiver)
        {
            this.prefab = behaiver.gameObject;
            InitPanelNode();
            SwitchComponent(behaiver);
           
        }

        private void OnGUI()
        {
            DrawObjectField();
            using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = scroll.scrollPosition;
                SwitchDrawOption();
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

            if (panelCompnent)
            {
                GenCodeUtil.AnalysisComponent(panelCompnent, components);
            }
        }
        private void SwitchComponent(MonoBehaviour v)
        {
            panelCompnent = v;
            if (panelCompnent != null){
                panelDrawer = UnityEditor.Editor.CreateEditor(panelCompnent);
            }
            GenCodeUtil.AnalysisComponent(panelCompnent, components);
        }
        private void DrawComponetHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "[控件列表]");
        }
        private void SwitchDrawOption()
        {
            selected = GUILayout.Toolbar(selected, options);
            if (prefab == null) return;

            if (selected == 0)
            {
                if (preComponentList == null)
                {
                    InitPanelNode();
                }
                DrawPreComponents();
            }
            else if (selected == 1)
            {
                DrawPanelComponent();
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
                prefab = EditorGUILayout.ObjectField(prefab, typeof(GameObject), false) as GameObject;
                if (EditorGUI.EndChangeCheck())
                {
                    GenCodeUtil.ChoiseAnUserMonobehiver(prefab, v => {
                        OpenWith(v);
                    });
                }
            }
        }

        /// <summary>
        /// 切换面板的打开状态
        /// </summary>
        private void ToggleOpen()
        {
            if (instenceID == 0)
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
                    var obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    obj.transform.SetParent(parent, false);
                    instenceID = obj.GetInstanceID();
                }
            }
            else
            {
                var obj = EditorUtility.InstanceIDToObject(instenceID);
                if (obj != null)
                {
                    DestroyImmediate(obj);
                }
                instenceID = 0;
            }

        }
        private void DrawPanelComponent()
        {
            if (!panelCompnent) return;

            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(new GUIContent("→", "快速绑定"), EditorStyles.toolbarButton, GUILayout.Width(20)))
                {
                    BridgeUI.CodeGen.GenCodeUtil.BindingUI(panelCompnent, components);
                }
            }

            GUILayout.Space(5);
            
            if (panelDrawer != null)
            {
                panelDrawer.OnInspectorGUI();
            }


        }
        private void DrawPreComponents()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("←", "快速解析"), EditorStyles.toolbarButton, GUILayout.Width(20)))
                {
                    var component = prefab.GetComponent<MonoBehaviour>();
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
                    var go = prefab;
                    rule.bindingAble = BindingAble;
                    GenCodeUtil.CreateScript(go, components, rule);
                }
            }

            //}


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
                                if (parent){
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

    }
}