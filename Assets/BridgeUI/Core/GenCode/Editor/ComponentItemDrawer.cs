using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using BridgeUI.CodeGen;
using System.Collections.Generic;
using BridgeUI.Binding;
using UnityEditorInternal;
using System;
using System.Linq;
using System.Reflection;
namespace BridgeUI.Drawer
{

    public class MemberViewer
    {
        public Dictionary<System.Type, Type[]> memberTypeDic = new Dictionary<System.Type, Type[]>();
        public Dictionary<System.Type, string[]> memberNameDic = new Dictionary<System.Type, string[]>();
        public Dictionary<System.Type, string[]> memberViewNameDic = new Dictionary<System.Type, string[]>();
        public Dictionary<System.Type, List<string>> memberMethodDic = new Dictionary<Type, List<string>>();
        public Type[] currentTypes;
        public string[] currentNames;
        public string[] currentViewNames;
        public List<string> currentMethods;
    }

    public class ComponentItemDrawer
    {
        Color fieldColor = new Color(.2f, 1f, .5f, 1f);
        Color activeColor = new Color(.2f, .5f, 1f, 1f);
        Dictionary<ComponentItem, ReorderableList> viewDic = new Dictionary<ComponentItem, ReorderableList>();
        Dictionary<ComponentItem, ReorderableList> eventDic = new Dictionary<ComponentItem, ReorderableList>();

        ReorderableList viewList;
        ReorderableList eventList;
        const float padding = 5;
        public float singleLineHeight = EditorGUIUtility.singleLineHeight + padding * 2;
        private float viewHeight;
        private float eventHeight;
        private MemberViewer viewMemberViewer = new MemberViewer();
        private MemberViewer eventMemberViewer = new MemberViewer();
        private static Dictionary<Type, Texture> _previewIcons;
        public static Dictionary<Type, Texture> previewIcons
        {
            get
            {
                if (_previewIcons == null)
                {
                    _previewIcons = new Dictionary<Type, Texture>();
#if UNITY_5_3
                    _previewIcons.Add(typeof(GameObject), EditorGUIUtility.FindTexture("GameObject Icon"));
#elif UNITY_5_6
                    _previewIcons.Add(typeof(GameObject), EditorGUIUtility.IconContent("GameObject Icon").image);
#else
            _previewIcons.Add(typeof(GameObject), EditorGUIUtility.IconContent("GameObject Icon").image);
#endif
                    _previewIcons.Add(typeof(Image), EditorGUIUtility.IconContent("Image Icon").image);
                    _previewIcons.Add(typeof(Text), EditorGUIUtility.IconContent("Text Icon").image);
                    _previewIcons.Add(typeof(Button), EditorGUIUtility.IconContent("Button Icon").image);
                    _previewIcons.Add(typeof(Toggle), EditorGUIUtility.IconContent("Toggle Icon").image);
                    _previewIcons.Add(typeof(Slider), EditorGUIUtility.IconContent("Slider Icon").image);
                    _previewIcons.Add(typeof(Scrollbar), EditorGUIUtility.IconContent("Scrollbar Icon").image);
                    _previewIcons.Add(typeof(Dropdown), EditorGUIUtility.IconContent("Dropdown Icon").image);
                    _previewIcons.Add(typeof(Canvas), EditorGUIUtility.IconContent("Canvas Icon").image);
                    _previewIcons.Add(typeof(RawImage), EditorGUIUtility.IconContent("RawImage Icon").image);
                    _previewIcons.Add(typeof(InputField), EditorGUIUtility.IconContent("InputField Icon").image);
                    _previewIcons.Add(typeof(ScrollRect), EditorGUIUtility.IconContent("ScrollRect Icon").image);
                    _previewIcons.Add(typeof(GridLayoutGroup), EditorGUIUtility.IconContent("GridLayoutGroup Icon").image);
                    _previewIcons.Add(typeof(ScriptableObject), EditorGUIUtility.IconContent("ScriptableObject Icon").image);
                }
                return _previewIcons;
            }
        }

        public float GetItemHeight(ComponentItem item, bool bindingAble)
        {
            if (item.isScriptComponent)
            {
                return singleLineHeight;
            }
            else
            {
                UpdateHeights(item, bindingAble);
                var height = singleLineHeight + (item.open ? ((bindingAble ? viewHeight : 0) + eventHeight) : 0f);
                return height;
            }
        }

        public void DrawItemOnRect(Rect rect, int index, ComponentItem item, bool bindingAble)
        {
            rect.height = GetItemHeight(item, bindingAble);
            DrawInfoHead(rect, item);
            DrawIndex(rect, index);
            if (item.open && !item.isScriptComponent)
            {
                DrawLists(rect, item, bindingAble);
            }
        }
        public void DrawBackground(Rect rect, bool active, ComponentItem item, bool bindingAble)
        {
            item.open = active;
            rect.height = GetItemHeight(item, bindingAble);
            var innerRect1 = new Rect(rect.x + padding, rect.y + padding, rect.width - 2 * padding, rect.height - 2 * padding);
            var color = GUI.backgroundColor;
            GUI.backgroundColor = active ? activeColor : fieldColor;
            GUI.Box(innerRect1, "");
            GUI.backgroundColor = color;
        }
        private void DrawInfoHead(Rect rect, ComponentItem item)
        {
            var innerRect = new Rect(rect.x + padding + 20, rect.y + padding, rect.width - 2 * padding - 20, EditorGUIUtility.singleLineHeight);

            var nameRect = new Rect(innerRect.x, innerRect.y, innerRect.width * 0.2f, innerRect.height);
            var targetRect = new Rect(innerRect.x + innerRect.width * 0.2f, innerRect.y, innerRect.width * 0.25f, innerRect.height);
            var typeRect = new Rect(innerRect.x + innerRect.width * 0.5f, innerRect.y, innerRect.width * 0.5f, innerRect.height);
            var iconRect = new Rect(innerRect.x + innerRect.width * 0.2f + 2.5f, innerRect.y + 2.5f, 12, 12);

            item.name = EditorGUI.TextField(nameRect, item.name);

            if (!item.isScriptComponent)
            {
                DrawTarget(targetRect, item);

                if (item.components != null)
                {
                    item.componentID = EditorGUI.Popup(typeRect, item.componentID, item.componentStrs);
                }

                if (previewIcons.ContainsKey(item.componentType))
                {
                    var icon = previewIcons[item.componentType];
                    EditorGUI.DrawTextureTransparent(iconRect, icon, ScaleMode.ScaleAndCrop);
                }
            }
            else
            {
                DrawScriptTarget(targetRect, item);

                EditorGUI.LabelField(typeRect, item.componentType.FullName);

                var icon = previewIcons[typeof(ScriptableObject)];
                EditorGUI.DrawTextureTransparent(iconRect, icon, ScaleMode.ScaleAndCrop);
            }


        }

        private void DrawScriptTarget(Rect targetRect, ComponentItem item)
        {
            EditorGUI.BeginChangeCheck();
            var newTarget = EditorGUI.ObjectField(targetRect, item.scriptTarget, item.componentType, true);
            if (newTarget != item.scriptTarget)
            {
                var prefabTarget = PrefabUtility.GetPrefabObject(newTarget);
                Debug.Log(prefabTarget);
                if (prefabTarget != null)
                {
                    newTarget = prefabTarget;
                }
                item.scriptTarget = newTarget as ScriptableObject;
            }

            if (EditorGUI.EndChangeCheck() && item.scriptTarget)
            {
                if (string.IsNullOrEmpty(item.name))
                {
                    item.name = item.scriptTarget.name;
                }
            }

        }

        private void DrawTarget(Rect targetRect, ComponentItem item)
        {
            EditorGUI.BeginChangeCheck();
            var newTarget = EditorGUI.ObjectField(targetRect, item.target, item.componentType, true);
            if (newTarget != item.target)
            {
                var prefabTarget = PrefabUtility.GetPrefabObject(newTarget);
                if (prefabTarget != null)
                {
                    newTarget = prefabTarget;
                }
                item.target = newTarget.GetType().GetProperty("gameObject").GetValue(newTarget, null) as GameObject;
            }

            if (EditorGUI.EndChangeCheck() && item.target)
            {
                var parent = PrefabUtility.GetPrefabObject(item.target);
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

        }

        private void DrawIndex(Rect rect, int index)
        {
            var indexRect = new Rect(rect.x + 5, rect.y + padding, 20, singleLineHeight);
            EditorGUI.LabelField(indexRect, index.ToString());
        }

        private void DrawLists(Rect rect, ComponentItem item, bool bindingAble)
        {
            UpdateHeights(item, bindingAble);
            var innerRect1 = new Rect(rect.x + padding, rect.y, rect.width - 30, rect.height - 2 * padding);

            if (bindingAble)
            {
                viewList = GetViewList(item);
                var viewRect = new Rect(innerRect1.x, innerRect1.y + singleLineHeight, innerRect1.width, viewHeight);
                viewList.DoList(viewRect);
            }

            eventList = GetEventList(item, bindingAble);

            var eventRect = new Rect(innerRect1.x, innerRect1.y + viewHeight + singleLineHeight, innerRect1.width, eventHeight);

            eventList.DoList(eventRect);
        }

        private void UpdateHeights(ComponentItem item, bool bindingAble)
        {
            if (bindingAble)
            {
                viewHeight = (EditorGUIUtility.singleLineHeight + padding) * (item.viewItems.Count >= 1 ? item.viewItems.Count + 2 : 3);
            }
            else
            {
                viewHeight = 0;
            }

            eventHeight = (EditorGUIUtility.singleLineHeight + padding) * (item.eventItems.Count >= 1 ? item.eventItems.Count + 2 : 3);
        }


        private ReorderableList GetViewList(ComponentItem item)
        {
            if (!viewDic.ContainsKey(item) || viewDic[item] == null)
            {
                var list = viewDic[item] = new ReorderableList(item.viewItems, typeof(BindingShow));
                list.drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "Members");
                };
                list.drawElementCallback = (rect, index, a, f) =>
                {
                    var viewItem = item.viewItems[index];
                    rect = new Rect(rect.x, rect.y + 2, rect.width, rect.height - 4);
                    var targetRect = new Rect(rect.x + rect.width * 0.1f, rect.y, rect.width * 0.5f, rect.height);
                    var sourceRect = new Rect(rect.x + rect.width * 0.65f, rect.y, rect.width * 0.25f, EditorGUIUtility.singleLineHeight);
                    var enableRect = new Rect(rect.x + rect.width * 0.92f, rect.y, rect.width * 0.1f, rect.height);

                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width * 0.1f, rect.height), new GUIContent("[t]", "Target"));
                    if (string.IsNullOrEmpty(viewItem.bindingSource))
                    {
                        EditorGUI.LabelField(sourceRect, "Source");
                    }
                    UpdateMemberByType(item.componentType);
                    EditorGUI.BeginChangeCheck();
                    var viewNameIndex = Array.IndexOf(viewMemberViewer.currentNames, viewItem.bindingTarget);
                    viewNameIndex = EditorGUI.Popup(targetRect, viewNameIndex, viewMemberViewer.currentViewNames);
                    viewItem.bindingSource = EditorGUI.TextArea(sourceRect, viewItem.bindingSource);
                    if (EditorGUI.EndChangeCheck())
                    {
                        viewItem.bindingTargetType = new BridgeUI.TypeInfo(viewMemberViewer.currentTypes[viewNameIndex]);
                        viewItem.bindingTarget = viewMemberViewer.currentNames[viewNameIndex];
                        viewItem.isMethod = viewMemberViewer.currentMethods.Contains(viewItem.bindingTarget);
                        Debug.Log(viewItem.bindingTarget + ":"+ viewItem.isMethod);
                    }
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.Toggle(enableRect, true);
                    EditorGUI.EndDisabledGroup();
                };
            }
            return viewDic[item];
        }
        private ReorderableList GetEventList(ComponentItem item, bool bindngAble)
        {
            if (!eventDic.ContainsKey(item) || eventDic[item] == null)
            {
                var list = eventDic[item] = new ReorderableList(item.eventItems, typeof(BindingEvent));
                list.drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "Events");
                };
                list.drawElementCallback = (rect, index, a, f) =>
                {
                    var eventItem = item.eventItems[index];
                    UpdateEventByType(item.componentType);
                    var targetRect = new Rect(rect.x + rect.width * 0.1f, rect.y, rect.width * 0.35f, rect.height);
                    var sourceRect = new Rect(rect.x + rect.width * 0.475f, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
                    var enableRect = new Rect(rect.x + rect.width * 0.8f, rect.y, rect.width * 0.2f, rect.height);
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width * 0.1f, rect.height), new GUIContent("[t]", "Target"));

                    if (string.IsNullOrEmpty(eventItem.bindingSource))
                    {
                        EditorGUI.LabelField(sourceRect, "Source");
                    }

                    EditorGUI.BeginChangeCheck();
                    var viewNameIndex = Array.IndexOf(eventMemberViewer.currentNames, eventItem.bindingTarget);
                    viewNameIndex = EditorGUI.Popup(targetRect, viewNameIndex, eventMemberViewer.currentViewNames);
                    eventItem.bindingSource = EditorGUI.TextArea(sourceRect, eventItem.bindingSource);
                    if (EditorGUI.EndChangeCheck())
                    {
                        eventItem.bindingTargetType = new BridgeUI.TypeInfo(eventMemberViewer.currentTypes[viewNameIndex]);
                        eventItem.bindingTarget = eventMemberViewer.currentNames[viewNameIndex];
                    }
                    EditorGUI.BeginDisabledGroup(!bindngAble);
                    eventItem.type = (BindingType)EditorGUI.EnumPopup(enableRect, eventItem.type);
                    EditorGUI.EndDisabledGroup();
                };
            }
            return eventDic[item];
        }

        private void UpdateMemberByType(System.Type type)
        {
            if (!viewMemberViewer.memberTypeDic.TryGetValue(type, out viewMemberViewer.currentTypes))
            {
                var typeList = new List<Type>();
                var nameList = new List<string>();
                var methods = new List<string>();
                var members = type.GetMembers(
                     System.Reflection.BindingFlags.Public |
                     System.Reflection.BindingFlags.Instance |
                     System.Reflection.BindingFlags.SetField);
                foreach (var member in members)
                {
                    switch (member.MemberType)
                    {
                        case MemberTypes.Field:
                            var field = member as FieldInfo;
                            if (!field.IsInitOnly) continue;
                            if (field.IsLiteral) continue;
                            if (IsMemberSupported(field.FieldType))
                            {
                                typeList.Add(field.FieldType);
                                nameList.Add(field.Name);
                            }
                            break;
                        case MemberTypes.Property:
                            var prop = member as PropertyInfo;
                            if (prop.GetSetMethod() == null) continue;
                            if (IsMemberSupported(prop.PropertyType))
                            {
                                typeList.Add(prop.PropertyType);
                                nameList.Add(prop.Name);
                            }
                            break;
                        case MemberTypes.Method:
                            var methodInfo = member as MethodInfo;
                            var parmeters = methodInfo.GetParameters();
                            if (parmeters.Count() == 1 &&
                                methodInfo.IsPublic &&
                                methodInfo.ReturnType == typeof(void) &&
                                !methodInfo.Name.StartsWith("set_"))
                            {
                                var parmeter = parmeters[0];
                                if (IsMemberSupported(parmeter.ParameterType))
                                {
                                    typeList.Add(parmeter.ParameterType);
                                    nameList.Add(methodInfo.Name);
                                    methods.Add(methodInfo.Name);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                viewMemberViewer.memberTypeDic[type] = typeList.ToArray();
                viewMemberViewer.memberNameDic[type] = nameList.ToArray();
                viewMemberViewer.memberMethodDic[type] = methods;
                var viewNameList = new List<string>();
                for (int i = 0; i < typeList.Count; i++)
                {
                    viewNameList.Add(string.Format("{1} ({0})", typeList[i].Name, nameList[i]));
                }
                viewMemberViewer.memberViewNameDic[type] = viewNameList.ToArray();
            }

            viewMemberViewer.currentNames = viewMemberViewer.memberNameDic[type];
            viewMemberViewer.currentTypes = viewMemberViewer.memberTypeDic[type];
            viewMemberViewer.currentViewNames = viewMemberViewer.memberViewNameDic[type];
            viewMemberViewer.currentMethods = viewMemberViewer.memberMethodDic[type];
        }

        private bool IsMemberSupported(System.Type type)
        {
            var attributes = type.GetCustomAttributes(true);
            if (attributes.Where(x => x is System.ObsoleteAttribute).Count() > 0)
            {
                return false;
            }
            return true;
        }

        private void UpdateEventByType(System.Type type)
        {
            if (!eventMemberViewer.memberTypeDic.TryGetValue(type, out eventMemberViewer.currentTypes))
            {
                var typeList = new List<Type>();
                var nameList = new List<string>();
                var props = type.GetProperties(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.GetProperty);

                var goodProps = props.Where(x => typeof(UnityEngine.Events.UnityEventBase).IsAssignableFrom(x.PropertyType));
                typeList.AddRange(goodProps.Select(x => x.PropertyType).ToArray());
                nameList.AddRange(goodProps.Select(x => x.Name).ToArray());


                var fields = type.GetFields(
                  System.Reflection.BindingFlags.Public |
                  System.Reflection.BindingFlags.Instance |
                  System.Reflection.BindingFlags.GetField);

                var goodFields = fields.Where(x => typeof(UnityEngine.Events.UnityEventBase).IsAssignableFrom(x.FieldType));
                typeList.AddRange(goodFields.Select(x => x.FieldType).ToArray());
                nameList.AddRange(goodFields.Select(x => x.Name).ToArray());


                eventMemberViewer.memberTypeDic[type] = typeList.ToArray();
                eventMemberViewer.memberNameDic[type] = nameList.ToArray();
                var viewNameList = new List<string>();
                for (int i = 0; i < typeList.Count; i++)
                {
                    viewNameList.Add(string.Format("{1} ({0})", typeList[i].Name, nameList[i]));
                }
                eventMemberViewer.memberViewNameDic[type] = viewNameList.ToArray();
            }

            eventMemberViewer.currentNames = eventMemberViewer.memberNameDic[type];
            eventMemberViewer.currentTypes = eventMemberViewer.memberTypeDic[type];
            eventMemberViewer.currentViewNames = eventMemberViewer.memberViewNameDic[type];
        }
    }
}