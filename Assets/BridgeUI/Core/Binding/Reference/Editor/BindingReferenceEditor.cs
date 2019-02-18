using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UI;
using System.Linq;
using System.Reflection;
using BridgeUI;
using BridgeUI.Binding;

namespace BridgeUI.Binding
{
   
    [CustomEditor(typeof(BindingReference), true)]
    public class BindingReferenceEditor : Editor
    {
        private bool inEdit;
        //private SerializedProperty refProperty;
        private SerializedProperty scriptProp;
        private List<ReferenceItem> referenceItems;
        private ReorderableList reorderList;
        private const float span = 10;
        private const float half_span = 5;
        private const float quad_span = 0.25f;
        private string nameSpace;
        private ReferenceItem currentItem;
        private List<System.Type> supportedTypes;
        private Texture2D hierarchyEventIcon;
        private Texture2D HierarchyEventIcon
        {
            get
            {
                if (hierarchyEventIcon == null)
                {
                    var path = AssetDatabase.GUIDToAssetPath("dded146e9a6d3a648ad7afa40e6bfdec");
                    hierarchyEventIcon = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
                return hierarchyEventIcon;
            }
        }
        private float currentWidth
        {
            get
            {
                return EditorGUIUtility.currentViewWidth - 40;
            }
        }
        private MonoBehaviour scriptBehaiver;
        private List<int> referencedGameObjects;

        [MenuItem("Component/BindingReference")]
        private static void CreateNewBindingReference()
        {
            if (Selection.activeGameObject != null)
            {
                var prefabName = Selection.activeGameObject.name;
                var scriptFolder = Setting.script_path + "/" + prefabName;
                if (!System.IO.Directory.Exists(scriptFolder))
                {
                    System.IO.Directory.CreateDirectory(scriptFolder);
                }
                var className = prefabName + "_Reference";
                var scriptPath = scriptFolder + "/" + className +".cs";
                var script = CreateScript(Setting.defultNameSpace, className, new List<ReferenceItem>());
                System.IO.File.WriteAllText(scriptPath, script);
                AssetDatabase.Refresh();

                Selection.activeGameObject.gameObject.AddComponent(typeof(BindingReference).Assembly.GetType(Setting.defultNameSpace +"."+ className));
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "按GameObject的名字创建引用代码", "OK");
            }
        }

        private void OnEnable()
        {
            if (target == null)
            {
                DestroyImmediate(this,true);
                return;
            }
            referencedGameObjects = new List<int>();
            scriptBehaiver = target as MonoBehaviour;
            //refProperty = serializedObject.FindProperty("m_data");
            scriptProp = serializedObject.FindProperty("m_Script");
            referenceItems = new List<ReferenceItem>();
            reorderList = new ReorderableList(referenceItems, typeof(ReferenceItem));
            reorderList.drawHeaderCallback = DrawHead;
            reorderList.elementHeight = EditorGUIUtility.singleLineHeight + span;
            reorderList.drawElementCallback = DrawReferenceItem;
            nameSpace = Setting.defultNameSpace;
            InitSupportTypes();
            ReadCacheInfos();
            AnalysisItemFromScript();
            EditorApplication.hierarchyWindowItemOnGUI -= DrawHierarchyIcon;
            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyIcon;
        }

        private void DrawHierarchyIcon(int instanceID, Rect selectionRect)
        {
            if (Application.isPlaying) return;
            if (!scriptBehaiver)
            {
                EditorApplication.hierarchyWindowItemOnGUI -= DrawHierarchyIcon;
                return;
            }
            if (Selection.activeGameObject != scriptBehaiver.gameObject) return;

            if (referenceItems.Count > 0 && referencedGameObjects.Contains(instanceID))
            {
                Rect rect = new Rect(selectionRect.x + selectionRect.width - 16f, selectionRect.y + 4f, 16f, 8f);
                GUI.DrawTexture(rect, HierarchyEventIcon);
            }
        }

        private void InitSupportTypes()
        {
            supportedTypes = new List<System.Type>()
            {
                typeof(LayoutElement),
                typeof(GridLayoutGroup),
                typeof(VerticalLayoutGroup),
                typeof(HorizontalLayoutGroup),

                typeof(Slider),
                typeof(Toggle),
                typeof(Button),
                typeof(Mask),
                typeof(Dropdown),
                typeof(InputField),

                typeof(RawImage),
                typeof(Image),
                typeof(Text),

                typeof(RectTransform),
                typeof(Transform),
                typeof(GameObject),

                typeof(ColorBlock),
                typeof(SpriteState),
                typeof(Vector2),
                typeof(Vector3),
                typeof(Vector4),
                typeof(Vector2Int),
                typeof(Vector3Int),

                typeof(char),
                typeof(byte),
                typeof(ushort),
                typeof(int),
                typeof(float),
                typeof(string),
                typeof(bool)
            };
        }

        private void OnChangedType(object data)
        {
            if (currentItem != null)
            {
                var type = (System.Type)data;
                currentItem.type = type;

                var refType = GetReferenceType(currentItem.type, currentItem.isArray);
                switch (refType)
                {
                    case ReferenceItemType.Reference:
                        {
                            //类型不匹配的问题
                            if (currentItem.referenceTarget != null && currentItem.referenceTarget.GetType() != currentItem.type)
                            {
                                currentItem.referenceTarget = WorpObject(currentItem.referenceTarget, currentItem.type);
                            }

                            //默认值
                            if (currentItem.referenceTarget == null && currentItem.referenceTargets != null && currentItem.referenceTargets.Count > 0)
                            {
                                var arrayItem = currentItem.referenceTargets[0];
                                if (arrayItem.GetType() == currentItem.type)
                                {
                                    currentItem.referenceTarget = arrayItem;
                                }
                                else
                                {
                                    currentItem.referenceTarget = WorpObject(arrayItem, currentItem.type);
                                }
                            }
                        }

                        break;
                    case ReferenceItemType.ConventAble:
                        {
                            try
                            {
                                System.Convert.ChangeType(currentItem.value, currentItem.type);
                            }
                            catch (System.Exception e)
                            {
                                currentItem.value = System.Activator.CreateInstance(currentItem.type).ToString();
                                Debug.Log(e);
                            }
                        }
                        break;
                    case ReferenceItemType.Struct:
                        {
                            try
                            {
                                JsonUtility.FromJson(currentItem.value, currentItem.type);
                            }
                            catch (System.Exception e)
                            {
                                currentItem.value = JsonUtility.ToJson(System.Activator.CreateInstance(currentItem.type));
                                Debug.LogError(e);
                            }
                        }
                        break;
                    case ReferenceItemType.ReferenceArray:
                        {
                            if (currentItem.referenceTargets == null)
                                currentItem.referenceTargets = new List<Object>();

                            if (currentItem.referenceTargets.Count > 0)
                            {
                                for (int i = 0; i < currentItem.referenceTargets.Count; i++)
                                {
                                    var item = currentItem.referenceTargets[i];
                                    if (item != null)
                                    {
                                        if (item.GetType() != currentItem.type)
                                        {
                                            currentItem.referenceTargets[i] = WorpObject(item, currentItem.type);
                                        }
                                    }
                                }
                            }
                            else if (currentItem.referenceTarget != null)
                            {
                                currentItem.referenceTargets.Add(WorpObject(currentItem.referenceTarget, currentItem.type));
                            }
                        }
                        break;
                    case ReferenceItemType.ConventAbleArray:
                        {
                            if (currentItem.values == null)
                                currentItem.values = new List<string>();

                            if (currentItem.values.Count > 0)
                            {
                                for (int i = 0; i < currentItem.values.Count; i++)
                                {
                                    try
                                    {
                                        System.Convert.ChangeType(currentItem.values[i], currentItem.type);
                                    }
                                    catch (System.Exception e)
                                    {
                                        currentItem.values[i] = System.Activator.CreateInstance(currentItem.type).ToString();
                                        Debug.Log(e);
                                    }
                                }
                            }

                            if (currentItem.values.Count == 0)
                            {
                                try
                                {
                                    System.Convert.ChangeType(currentItem.value, currentItem.type);
                                    currentItem.values.Add(currentItem.value);
                                }
                                catch
                                {
                                    currentItem.values.Add(System.Activator.CreateInstance(currentItem.type).ToString());
                                }
                            }
                        }
                        break;
                    case ReferenceItemType.StructArray:
                        {
                            if (currentItem.values == null)
                                currentItem.values = new List<string>();

                            if (currentItem.values.Count > 0)
                            {
                                for (int i = 0; i < currentItem.values.Count; i++)
                                {
                                    try
                                    {
                                        JsonUtility.FromJson(currentItem.values[i], currentItem.type);
                                    }
                                    catch (System.Exception e)
                                    {
                                        currentItem.values[i] = JsonUtility.ToJson(System.Activator.CreateInstance(currentItem.type));
                                        Debug.Log(e);
                                    }
                                }
                            }

                            if (currentItem.values.Count == 0)
                            {
                                try
                                {
                                    JsonUtility.FromJson(currentItem.value, currentItem.type);
                                    currentItem.values.Add(currentItem.value);
                                }
                                catch
                                {
                                    currentItem.values.Add(JsonUtility.ToJson(System.Activator.CreateInstance(currentItem.type)));
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private Object WorpObject(Object currentItemreferenceTarget, System.Type currentItemtype)
        {
            if (currentItemreferenceTarget == null) return null;

            if (currentItemreferenceTarget.GetType() == currentItemtype)
            {
                return currentItemreferenceTarget;
            }

            if (currentItemreferenceTarget is GameObject)
            {
                if (typeof(Component).IsAssignableFrom(currentItemtype))
                {
                    currentItemreferenceTarget = (currentItemreferenceTarget as GameObject).GetComponent(currentItemtype);
                }
                else
                {
                    currentItemreferenceTarget = null;
                }
            }
            else
            {
                if (typeof(Component).IsAssignableFrom(currentItemtype))
                {
                    currentItemreferenceTarget = (currentItemreferenceTarget as Component).GetComponent(currentItemtype);
                }
                else if (currentItemtype == typeof(GameObject))
                {
                    currentItemreferenceTarget = (currentItemreferenceTarget as Component).gameObject;
                }
                else
                {
                    currentItemreferenceTarget = null;
                }
            }
            return currentItemreferenceTarget;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (inEdit)
            {
                var headRect = GUILayoutUtility.GetRect(currentWidth, EditorGUIUtility.singleLineHeight);
                DrawScript(headRect);
                reorderList.DoLayoutList();
            }
            else
            {
                var headRect = GUILayoutUtility.GetRect(0,0);
                var btnRect = new Rect(headRect.x + headRect.width - 60, headRect.y + 5, 60, EditorGUIUtility.singleLineHeight);

                if (GUI.Button(btnRect, "edit"))
                {
                    inEdit = true;
                    AnalysisItemFromScript();
                }

                base.OnInspectorGUI();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHead(Rect rect)
        {
            var buttonRect = new Rect(rect.x + rect.width - 60, rect.y, 60, rect.height);
            if (GUI.Button(buttonRect, "apply"))
            {
                inEdit = false;
                ApplyChangesToScript();
            }

            EditorGUI.LabelField(rect, "数据及引用信息列表");

            if (Event.current.type == EventType.DragUpdated)
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                }
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        Object obj = DragAndDrop.objectReferences[i];
                        if (obj is GameObject){
                            obj = SortObjectType(obj as GameObject);
                        }
                        var item = new ReferenceItem();
                        item.name = obj.name;
                        item.type = obj.GetType();
                        item.referenceTarget = obj;
                        referenceItems.Add(item);
                    }
                }
            }
        }

        private Object SortObjectType(GameObject go)
        {
            var uiControl = go.GetComponent<IUIControl>();
            if(uiControl != null)
            {
                return uiControl as Component;
            }

            for (int i = 0; i < supportedTypes.Count; i++)
            {
                var type = supportedTypes[i];
                if (typeof(Component).IsAssignableFrom(type))
                {
                    var obj = go.GetComponent(type);
                    if (obj != null)
                    {
                        return obj;
                    }
                }
            }
            return go;
        }

        private void DrawReferenceItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (referenceItems.Count > index && index >= 0)
            {
                var boxRect = new Rect(rect.x + half_span, rect.y + quad_span, rect.width, rect.height - quad_span);
                GUI.Box(boxRect, "");
                var innerRect = new Rect(rect.x + span, rect.y + half_span, rect.width - span, rect.height - span);
                var idRect = new Rect(rect.x - 15, rect.y + half_span, 40, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(idRect, index.ToString("00"));
                var referenceItem = referenceItems[index];
                var nameRect = new Rect(innerRect.x, innerRect.y, innerRect.width * 0.2f, innerRect.height);
                EditorGUI.BeginChangeCheck();
                referenceItem.name = EditorGUI.TextField(nameRect, referenceItem.name);
                if (EditorGUI.EndChangeCheck())
                {
                    if (referenceItem.type == typeof(GameObject))
                    {
                        referenceItem.referenceTarget.name = referenceItem.name;
                    }
                }

                var typeRect = new Rect(innerRect.x + innerRect.width * 0.25f, innerRect.y, innerRect.width * 0.25f, innerRect.height);
                GUI.Box(typeRect, "");
                DrawTypeField(typeRect, referenceItem);

                var isArrayRect = new Rect(innerRect.x + innerRect.width * 0.55f, innerRect.y, 20, innerRect.height);
                DrawIsArrayField(isArrayRect, referenceItem);

                var contentRect = new Rect(innerRect.x + innerRect.width * 0.6f, innerRect.y, innerRect.width * 0.4f, innerRect.height);
                if (referenceItem.type != null)
                {
                    var refType = GetReferenceType(referenceItem.type, referenceItem.isArray);

                    switch (refType)
                    {
                        case ReferenceItemType.Reference:
                            referenceItem.referenceTarget = EditorGUI.ObjectField(contentRect, GUIContent.none, referenceItem.referenceTarget, referenceItem.type, true);
                            break;
                        case ReferenceItemType.ConventAble:
                            referenceItem.value = EditorGUI.TextField(contentRect, referenceItem.value);
                            try
                            {
                                if (referenceItem.type != typeof(string))
                                {
                                    if (!string.IsNullOrEmpty(referenceItem.value))
                                    {
                                        System.Convert.ChangeType(referenceItem.value, referenceItem.type);
                                    }
                                    else
                                    {
                                        referenceItem.value = System.Activator.CreateInstance(referenceItem.type).ToString();
                                    }
                                }
                            }
                            catch (System.Exception e)
                            {
                                Debug.Log("值转换失败：" + referenceItem.name + " -> detail:" + e);
                            }
                            break;
                        case ReferenceItemType.Struct:
                            EditorGUI.LabelField(contentRect, "(结构)");
                            break;
                        case ReferenceItemType.ReferenceArray:
                            if (referenceItem.referenceTargets == null)
                                referenceItem.referenceTargets = new List<Object>();
                            EditorGUI.LabelField(contentRect, "(引用数组)" + referenceItem.referenceTargets.Count);
                            break;
                        case ReferenceItemType.ConventAbleArray:
                            if (referenceItem.values == null)
                                referenceItem.values = new List<string>();
                            EditorGUI.LabelField(contentRect, "(简单数组)" + referenceItem.values.Count);
                            break;
                        case ReferenceItemType.StructArray:
                            if (referenceItem.values == null)
                                referenceItem.values = new List<string>();
                            EditorGUI.LabelField(contentRect, "(结构数组)" + referenceItem.values.Count);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void DrawIsArrayField(Rect isArrayRect, ReferenceItem referenceItem)
        {
            EditorGUI.BeginChangeCheck();
            referenceItem.isArray = EditorGUI.Toggle(isArrayRect, referenceItem.isArray, EditorStyles.radioButton);
            if(EditorGUI.EndChangeCheck())
            {
                //将单个类型自动添加到数组
            }
        }

        private void DrawTypeField(Rect rect, ReferenceItem item)
        {
            if (item.type != null)
            {
                EditorGUI.LabelField(rect, item.type.Name);
            }
            else
            {
                EditorGUI.LabelField(rect, "未指定类型");
            }

            if (Event.current.type == EventType.ContextClick)
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    currentItem = item;
                    var changeTypeMenu = new GenericMenu();
                    if(currentItem.referenceTarget != null)
                    {
                        TryAddMenuFromTarget(currentItem.referenceTarget, item.type, changeTypeMenu);
                    }
                    if (currentItem.referenceTargets != null && currentItem.referenceTargets.Count > 0)
                    {
                        TryAddMenuFromTarget(currentItem.referenceTargets[0], item.type, changeTypeMenu);
                    }
                    for (int i = 0; i < supportedTypes.Count; i++)
                    {
                        var type = supportedTypes[i];
                        changeTypeMenu.AddItem(new GUIContent(type.FullName), type == item.type, OnChangedType, type);
                    }
                    changeTypeMenu.ShowAsContext();
                }
            }
        }

        private void TryAddMenuFromTarget(Object target,System.Type currentType,GenericMenu menu)
        {
            var uiControl = TryGetControl(target);

            if (uiControl != null)
            {
                var type = uiControl.GetType();
                var select = currentType == type;
                menu.AddItem(new GUIContent(type.FullName), select, OnChangedType, type);
            }
        }

        private IUIControl TryGetControl(Object target)
        {
            IUIControl uiControl = null;
            if (target is GameObject)
            {
                uiControl = (target as GameObject).GetComponent<IUIControl>();

            }
            if (target is Component)
            {
                uiControl = (target as Component).GetComponent<IUIControl>();
            }

            return uiControl;
        }

        private ReferenceItemType GetReferenceType(System.Type type, bool isArray)
        {
            if (type.IsArray)
            {
                var arryType = type.GetElementType();
                if (typeof(UnityEngine.Object).IsAssignableFrom(arryType))
                {
                    return ReferenceItemType.ReferenceArray;
                }
                else if (typeof(System.IConvertible).IsAssignableFrom(arryType))
                {
                    return ReferenceItemType.ConventAbleArray;
                }
                else
                {
                    return ReferenceItemType.StructArray;
                }
            }
            else if (isArray)
            {
                if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                {
                    return ReferenceItemType.ReferenceArray;
                }
                else if (typeof(System.IConvertible).IsAssignableFrom(type))
                {
                    return ReferenceItemType.ConventAbleArray;
                }
                else
                {
                    return ReferenceItemType.StructArray;
                }
            }
            else
            {
                if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                {
                    return ReferenceItemType.Reference;
                }
                else if (typeof(System.IConvertible).IsAssignableFrom(type))
                {
                    return ReferenceItemType.ConventAble;
                }
                else
                {
                    return ReferenceItemType.Struct;
                }
            }
        }

        private void AnalysisItemFromScript()
        {
            var flags = System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic;
            var fieldsArray = new List<FieldInfo>();
            var innerFields = target.GetType().GetFields(flags|BindingFlags.DeclaredOnly);
            int innerCount = innerFields.Length;
            fieldsArray.AddRange(innerFields);

            var referenceField = target.GetType().BaseType.GetField("m_data", flags);
            object referenceValue = null;
            if (referenceField!= null)
            {
                referenceValue = referenceField.GetValue(target);
                var referenceType = referenceField.FieldType;
                var dataFields = referenceType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField);
                fieldsArray.AddRange(dataFields);
            }
        

            referenceItems.Clear();

            if (fieldsArray != null && fieldsArray.Count > 0)
            {
                for (int i = 0; i < fieldsArray.Count; i++)
                {
                    var fieldItem = fieldsArray[i];
                    var item = new ReferenceItem();
                    item.name = typeof(UnityEngine.Object).IsAssignableFrom(fieldItem.FieldType) ? fieldItem.Name.Substring(2) : fieldItem.Name;
                    item.type = fieldItem.FieldType.IsArray ? fieldItem.FieldType.GetElementType() : fieldItem.FieldType;
                    var value = fieldItem.GetValue(i >= innerCount? referenceValue: target);
                    var arrayValue = value as System.Array;
                    item.isArray = arrayValue != null;
                    if (value != null)
                    {
                        var refType = GetReferenceType(item.type, item.isArray);
                        switch (refType)
                        {
                            case ReferenceItemType.Reference:
                                item.referenceTarget = (UnityEngine.Object)value;
                                RegistReferenceObject(item.referenceTarget);
                                break;
                            case ReferenceItemType.ConventAble:
                                item.value = value.ToString();
                                break;
                            case ReferenceItemType.Struct:
                                item.value = JsonUtility.ToJson(value);
                                break;
                            case ReferenceItemType.ReferenceArray:
                                {
                                    item.referenceTargets = new List<Object>();
                                    var enumerator = arrayValue.GetEnumerator();
                                    while (enumerator.MoveNext())
                                    {
                                        item.referenceTargets.Add((Object)enumerator.Current);
                                    }
                                    RegistReferenceObject(item.referenceTargets.ToArray());
                                }
                                break;
                            case ReferenceItemType.ConventAbleArray:
                                {
                                    item.values = new List<string>();
                                    var enumerator = arrayValue.GetEnumerator();
                                    while (enumerator.MoveNext())
                                    {
                                        item.values.Add(enumerator.Current.ToString());
                                    }
                                }
                                break;
                            case ReferenceItemType.StructArray:
                                {
                                    item.values = new List<string>();
                                    var enumerator = arrayValue.GetEnumerator();
                                    while (enumerator.MoveNext())
                                    {
                                        item.values.Add(JsonUtility.ToJson(enumerator.Current));
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                    }
                    referenceItems.Add(item);
                }
            }
        }

        private void RegistReferenceObject(params Object[] referenceTargets)
        {
            for (int i = 0; i < referenceTargets.Length; i++)
            {
                var referenceTarget = referenceTargets[i];
                if (referenceTarget)
                {
                    if (referenceTarget is Component)
                    {
                        referencedGameObjects.Add((referenceTarget as Component).gameObject.GetInstanceID());
                    }
                    else if (referenceTarget is GameObject)
                    {
                        referencedGameObjects.Add((referenceTarget as GameObject).GetInstanceID());
                    }
                }
            }

        }

        private void ReadCacheInfos()
        {
            if (!target || !(target is MonoBehaviour)) return;
            var referenceBehaiver = (target as MonoBehaviour).gameObject.GetComponent<ReferenceCatchBehaiver>();
            if (referenceBehaiver != null)
            {
                bool canDelete = true;
                referenceItems.Clear();
                var infoItems = referenceBehaiver.GetReferenceItems();
                if (infoItems != null)
                {
                    var flags = System.Reflection.BindingFlags.GetField |
               System.Reflection.BindingFlags.Instance |
               System.Reflection.BindingFlags.NonPublic;

                    System.Type referenceType = null;
                    object referenceValue = null;

                    var referenceField = target.GetType().BaseType.GetField("m_data", flags);

                    if (referenceField != null)
                    {
                        referenceValue = referenceField.GetValue(target);
                        referenceType = referenceField.FieldType;
                    }

                    for (int i = 0; i < infoItems.Count; i++)
                    {
                        var infoItem = infoItems[i];
                        FieldInfo field = null;
                        object targetObj = null;
                        if (typeof(UnityEngine.Object).IsAssignableFrom(infoItem.type))
                        {
                            field = target.GetType().GetField("m_" + infoItem.name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField);
                            targetObj = target;
                        }
                        else
                        {
                            field = referenceType.GetField(infoItem.name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField);
                            targetObj = referenceValue;
                        }

                        if (field != null)
                        {
                            var refType = GetReferenceType(field.FieldType, infoItem.isArray);
                            try
                            {
                                #region SetValues
                                switch (refType)
                                {
                                    case ReferenceItemType.Reference:
                                        field.SetValue(targetObj, infoItem.referenceTarget);
                                        break;
                                    case ReferenceItemType.ConventAble:
                                        field.SetValue(targetObj, System.Convert.ChangeType(infoItem.value, infoItem.type));
                                        break;
                                    case ReferenceItemType.Struct:
                                        field.SetValue(targetObj, JsonUtility.FromJson(infoItem.value, infoItem.type));
                                        break;
                                    case ReferenceItemType.ReferenceArray:
                                        {
                                            if (infoItem.referenceTargets != null)
                                            {
                                                System.Array referenceArray = infoItem.referenceTargets.ToArray();
                                                var arrayType = infoItem.type.MakeArrayType();
                                                var array = System.Activator.CreateInstance(arrayType, referenceArray.Length) as System.Array;
                                                referenceArray.CopyTo(array, 0);
                                                field.SetValue(targetObj, array);
                                            }
                                        }
                                        break;
                                    case ReferenceItemType.ConventAbleArray:
                                        {
                                            if (infoItem.values != null)
                                            {
                                                System.Array valuesArray = infoItem.values.Select(x => System.Convert.ChangeType(x, infoItem.type)).ToArray();
                                                var arrayType = infoItem.type.MakeArrayType();
                                                var array = System.Activator.CreateInstance(arrayType, valuesArray.Length) as System.Array;
                                                valuesArray.CopyTo(array, 0);
                                                field.SetValue(targetObj, array);
                                            }
                                        }

                                        break;
                                    case ReferenceItemType.StructArray:
                                        if (infoItem.values != null)
                                        {
                                            System.Array valuesArray = infoItem.values.Select(x => JsonUtility.FromJson(x, infoItem.type)).ToArray();
                                            var arrayType = infoItem.type.MakeArrayType();
                                            var array = System.Activator.CreateInstance(arrayType, valuesArray.Length) as System.Array;
                                            valuesArray.CopyTo(array, 0);
                                            field.SetValue(targetObj, array);

                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            catch (System.Exception e)
                            {
                                canDelete = false;
                                Debug.Log("类型转换失败：" + field.Name + " detail: " + e);
                            }
                            #endregion SetValues
                        }
                        else
                        {
                            canDelete = false;
                            Debug.Log("类型不存在：" + infoItem.name);
                        }
                    }

                    if(referenceField!= null)
                        referenceField.SetValue(target, referenceValue);
                }
                if (canDelete)
                    Object.DestroyImmediate(referenceBehaiver,true);
            }
        }

        private void CacheInfos()
        {
            var referenceBehaiver = (target as MonoBehaviour).gameObject.AddComponent<ReferenceCatchBehaiver>();
            referenceBehaiver.SetReferenceItems(referenceItems);
            EditorUtility.SetDirty(referenceBehaiver);
        }

        private void ApplyChangesToScript()
        {
            var scriptType = target.GetType();
            var monoScript = MonoScript.FromMonoBehaviour(target as MonoBehaviour);
            var scriptPath = AssetDatabase.GetAssetPath(monoScript);
            var nameSpace = scriptType.Namespace;
            var className = scriptType.Name;

            if (string.IsNullOrEmpty(nameSpace))
            {
                nameSpace = this.nameSpace;
            }

            var script = CreateScript(nameSpace, className,referenceItems);
            System.IO.File.WriteAllText(scriptPath, script);
            CacheInfos();
            AssetDatabase.Refresh();
        }

        private void DrawScript(Rect rect)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(rect, scriptProp);
            EditorGUI.EndDisabledGroup();
        }


        public static string CreateScript(string nameSpace, string className,List<ReferenceItem> referenceItems)
        {
            var referenceItems_ref = referenceItems.Where(x => typeof(UnityEngine.Object).IsAssignableFrom(x.type)).ToArray();
            var referenceItems_data = referenceItems.Where(x => !typeof(UnityEngine.Object).IsAssignableFrom(x.type)).ToArray();
            var haveData = referenceItems_data.Length > 0;
            var baseClassName = haveData ? "BindingReference<" + className + ".Data >": "BindingReference";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("///*************************************************************************************\n");
            sb.Append("///* 更新时间：       "); sb.Append(System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss\n"));
            sb.Append("///* 说    明：       1.本代码由工具自动生成\n");
            sb.Append("///                   2.如需要扩展支持的类型，请在ObjectRefEditor中进行编辑\n");
            sb.Append("///                   3.注意手动添加的代码不一定能正常解析！\n");
            sb.Append("///* ************************************************************************************/\n\n");

            sb.Append("using System;\n");
            sb.Append("using BridgeUI;\n");
            sb.Append("using BridgeUI.Binding;\n");
            sb.Append("using UnityEngine;\n\n");

            sb.Append("namespace "); sb.Append(nameSpace); sb.Append(" {\n");

            sb.Append("\tpublic class "); sb.Append(className); sb.Append(" : "); sb.Append(baseClassName); sb.AppendLine();
            sb.Append("\t{\n");
            for (int i = 0; i < referenceItems_ref.Length; i++)
            {
                var item = referenceItems[i];
                sb.Append("\t\t[SerializeField]\n");
                sb.Append("\t\tprivate "); sb.Append(item.type.FullName);
                if (item.isArray)
                    sb.Append("[]");
                sb.Append(" m_"); sb.Append(item.name); sb.Append(";\n");
            }
            sb.Append("\n");

            if(haveData)
            {
                sb.Append("\t\t[Serializable]\n");
                sb.Append("\t\tpublic struct Data\n");
                sb.Append("\t\t{\n");
                for (int i = 0; i < referenceItems_data.Length; i++)
                {
                    var item = referenceItems_data[i];
                    sb.Append("\t\t\tpublic "); sb.Append(item.type.FullName);
                    if (item.isArray)
                        sb.Append("[]");
                    sb.Append(" "); sb.Append(item.name); sb.Append(";\n");
                }
                sb.Append("\t\t}\n");
            }
           
            sb.Append("\t\t#region Propertys\n");
            for (int i = 0; i < referenceItems_data.Length; i++)
            {
                var item = referenceItems_data[i];
                sb.Append("\t\tpublic "); sb.Append(item.type.FullName);
                if (item.isArray) sb.Append("[]");
                sb.Append(" "); sb.Append(item.name); sb.Append("\n");
                sb.Append("\t\t{\n");
                sb.Append("\t\t\tget { return m_data."); sb.Append(item.name); sb.Append("; }\n");
                sb.Append("\t\t}\n");
            }
            for (int i = 0; i < referenceItems_ref.Length; i++)
            {
                var item = referenceItems_ref[i];
                sb.Append("\t\tpublic "); sb.Append(item.type.FullName);
                if (item.isArray) sb.Append("[]");
                sb.Append(" "); sb.Append(item.name); sb.Append("\n");
                sb.Append("\t\t{\n");
                sb.Append("\t\t\tget { return m_"); sb.Append(item.name); sb.Append("; }\n");
                sb.Append("\t\t}\n");
            }

            sb.Append("\t\t#endregion Propertys\n");
            sb.Append("\t}\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
