using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using BridgeUI;
using BridgeUI.CodeGen;
using BridgeUI.Binding;
using UnityEditorInternal;

namespace BridgeUI.Drawer
{
    public class BindingWindow : EditorWindow
    {

        [UnityEditor.Callbacks.OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var prefab = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (prefab != null && prefab.GetComponent<RectTransform>())
            {
                GenCodeUtil.ChoiseAnReferenceMonobehiver(prefab, v =>
                {
                    if (v)
                    {
                        var window = GetWindow<BindingWindow>();
                        window.OpenWith(v);
                    }
                    else
                    {
                        Debug.LogError("预制体上未找到引用脚本！");
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
        private GenCodeRule rule;
        private System.Collections.Generic.List<ComponentItem> components = new List<ComponentItem>();
        private ReorderableList preComponentList;
        private ComponentItemDrawer itemDrawer = new ComponentItemDrawer();
        private Vector2 scrollPos;
        private bool bindingAble;

        private void OpenWith(MonoBehaviour behaiver)
        {
            this.prefab = behaiver.gameObject;
            rule = new GenCodeRule(Setting.defultNameSpace);
            InitPanelNode();
            SwitchComponent(behaiver);
        }
        private void UpdateBindingAble()
        {
            var baseTypeStr = GenCodeUtil.supportBaseTypes[rule.baseTypeIndex];
            var baseType = typeof(BindingViewBase).Assembly.GetType(baseTypeStr);
            bindingAble = typeof(BindingViewBase).IsAssignableFrom(baseType);
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
                UpdateBindingAble();

                preComponentList = new ReorderableList(components, typeof(ComponentItem));
                preComponentList.drawHeaderCallback = DrawComponetHeader;
                //preComponentList.elementHeight = itemDrawer.singleLineHeight;
                preComponentList.showDefaultBackground = true;

                preComponentList.elementHeightCallback += (index) =>
                {
                    var prop = components[index];
                    return itemDrawer.GetItemHeight(prop,bindingAble);
                };
                preComponentList.drawElementCallback += (rect, index, isFocused, isActive) =>
                {
                    itemDrawer.DrawItemOnRect(rect, index, components[index], bindingAble);
                };
                preComponentList.drawElementBackgroundCallback += (rect, index, isFocused, isActive) =>
                {
                    if (components.Count > index && index >= 0)
                    {
                        itemDrawer.DrawBackground(rect, isFocused, components[index], bindingAble);
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
        private void SwitchComponent(MonoBehaviour v)
        {
            panelCompnent = v;
            if (panelCompnent != null)
            {
                panelDrawer = UnityEditor.Editor.CreateEditor(panelCompnent);
            }
            GenCodeUtil.AnalysisComponent(panelCompnent, components, rule);
        }

        //private void InitPanelBase(MonoBehaviour v)
        //{
        //    if (v is BindingViewBase)
        //    {
        //        var type = v.GetType();
        //        var find = false;
        //        while (!find && type.BaseType != typeof(object))
        //        {
        //            type = type.BaseType;
        //            for (int i = 0; i < GenCodeUtil.supportBaseTypes.Length; i++)
        //            {
        //                if (type.FullName == GenCodeUtil.supportBaseTypes[i])
        //                {
        //                    rule.baseTypeIndex = i;
        //                    find = true;
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //}

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
                    GenCodeUtil.ChoiseAnReferenceMonobehiver(prefab, v =>
                    {
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
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("→", "快速绑定"), EditorStyles.toolbarButton, GUILayout.Width(20)))
                {
                    BridgeUI.CodeGen.GenCodeUtil.BindingUIComponents(panelCompnent, components);
                }
            }

            GUILayout.Space(5);

            if (panelDrawer != null)
            {
                panelDrawer.OnInspectorGUI();
                EditorUtility.SetDirty(panelDrawer.target);
            }


        }
        private void DrawPreComponents()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("命名空间", GUILayout.Width(60));
                rule.nameSpace = GUILayout.TextField(rule.nameSpace);
                if (GUILayout.Button(new GUIContent("←", "快速解析"), EditorStyles.toolbarButton, GUILayout.Width(20)))
                {
                    GenCodeUtil.ChoiseAnReferenceMonobehiver(prefab, component =>
                    {
                        if (component == null)
                        {
                            EditorApplication.Beep();
                        }
                        else
                        {
                            //从旧的脚本解析出
                            GenCodeUtil.AnalysisComponent(component, components, rule);
                        }
                    });

                }

            }

            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("BaseType:", GUILayout.Width(lableWidth));
                EditorGUI.BeginChangeCheck();
                rule.baseTypeIndex = EditorGUILayout.Popup(rule.baseTypeIndex, GenCodeUtil.supportBaseTypes);
                if(EditorGUI.EndChangeCheck())
                {
                    UpdateBindingAble();
                }
                if (GUILayout.Button("update", EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    var go = prefab;
                    rule.bindingAble = bindingAble;
                    GenCodeUtil.UpdateBindingScripts(go, components, rule);
                }

            }


            preComponentList.DoLayoutList();

            var addRect = GUILayoutUtility.GetRect(BridgeUI.Drawer.BridgeEditorUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);

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
                                var parent = PrefabUtility.GetCorrespondingObjectFromSource(obj);
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

    }
}