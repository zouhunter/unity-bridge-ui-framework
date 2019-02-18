using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using UnityEditor;
using BridgeUI;
using BridgeUI.Binding;
using BridgeUI.Model;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;

namespace BridgeUI.CodeGen
{
    public static class GenCodeUtil
    {
        public static Type[] supportControls;//支持的控件
        public static string[] supportBaseTypes;//父级类型
        private static List<string> InnerNameSpace;//过滤内部命名空间

        static GenCodeUtil()
        {
            supportBaseTypes = LoadAllBasePanels();
            supportControls = new Type[]
            {
                typeof(ScrollRect),
                typeof(InputField),
                typeof(Dropdown),
                typeof(Button),
                typeof(Toggle),
                typeof(Image),
                typeof(Text),
                typeof(Slider),
                typeof(HorizontalLayoutGroup),
                typeof(VerticalLayoutGroup),
                typeof(LayoutElement),
                typeof(RectTransform),
                typeof(GameObject),
            };
            InnerNameSpace = new List<string>()
            {
                 "UnityEngine.UI",
                 "UnityEngine",
                 "UnityEngine.EventSystems"
             };
        }

        /// <summary>
        /// 快速进行控件绑定
        /// </summary>
        /// <param name="go"></param>
        /// <param name="components"></param>
        public static void BindingUIComponents(MonoBehaviour behaiver, List<ComponentItem> components)
        {
            if (behaiver == null)
            {
                EditorApplication.Beep();
                return;
            }

            foreach (var item in components)
            {
                var filedName = "m_" + item.name;
                UnityEngine.Object obj = item.isScriptComponent ? item.scriptTarget as UnityEngine.Object : item.target;
                if (item.componentType != typeof(GameObject) && !typeof(ScriptableObject).IsAssignableFrom(item.componentType))
                {
                    obj = item.target.GetComponent(item.componentType);
                }
                behaiver.GetType().InvokeMember(filedName,
                                BindingFlags.SetField |
                                BindingFlags.Instance |
                                BindingFlags.NonPublic |
                                BindingFlags.Public,
                                null, behaiver, new object[] { obj }, null, null, null);
            }
        }

        /// <summary>
        /// 分析代码的的组件信息
        /// </summary>
        /// <param name="component"></param>
        /// <param name="components"></param>
        public static void AnalysisComponent(MonoBehaviour component, List<ComponentItem> components, GenCodeRule rule)
        {
            var type = component.GetType();

            rule.nameSpace = type.Namespace;

            var propertys = type.GetProperties(BindingFlags.GetProperty | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var prop in propertys)
            {
                var support = typeof(UnityEngine.MonoBehaviour).IsAssignableFrom(prop.PropertyType)
                    || typeof(ScriptableObject).IsAssignableFrom(prop.PropertyType)
                    || prop.PropertyType == typeof(GameObject);

                if (support)
                {
                    var compItem = components.Find(x => "m_" + x.name == prop.Name || x.name == prop.Name);

                    if (compItem == null)
                    {
                        compItem = new ComponentItem();
                        compItem.name = prop.Name.Replace("m_", "");
                        components.Add(compItem);
                    }

                    var value = prop.GetValue(component, new object[0]);
                    if (value != null)
                    {
                        if (prop.PropertyType == typeof(GameObject))
                        {
                            compItem.target = value as GameObject;
                            compItem.components = SortComponent(compItem.target);
                            var types = Array.ConvertAll(compItem.components, x => x.type);
                            compItem.componentID = Array.IndexOf(types, typeof(GameObject));
                        }
                        else if (typeof(ScriptableObject).IsAssignableFrom(prop.PropertyType))
                        {
                            compItem.UpdateAsScriptObject(value as ScriptableObject);
                        }
                        else
                        {
                            compItem.target = (value as MonoBehaviour).gameObject;
                            compItem.components = SortComponent(compItem.target);
                            var types = Array.ConvertAll(compItem.components, x => x.type);
                            compItem.componentID = Array.IndexOf(types, value.GetType());
                        }
                    }
                }
            }

            ViewCoder coder = new ViewCoder();
            coder.AnalysisBinding(component, components.ToArray(), rule);
        }

        /// <summary>
        /// 创建代码
        /// </summary>
        /// <param name="go"></param>
        /// <param name="components"></param>
        /// <param name="rule"></param>
        public static void UpdateBindingScripts(GameObject go, List<ComponentItem> components, GenCodeRule rule)
        {
            Action<ViewCoder> onLoad = (uiCoder) =>
            {
                var baseType = GenCodeUtil.supportBaseTypes[rule.baseTypeIndex];
                var needAdd = FilterExisField(baseType, components);
                uiCoder.parentClassName = supportBaseTypes[rule.baseTypeIndex];
                uiCoder.componentItems = needAdd;
                uiCoder.CompileSave();
                UnityEditor.EditorApplication.delayCall += AssetDatabase.Refresh;
            };

            LoadBindingScriptAsnyc(go, components, onLoad);
        }

        /// <summary>
        /// 将类型转换为人可读的字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string TypeStringName(Type type)
        {
            var typeName = type.FullName;
            if (type.IsGenericType)
            {
                typeName = type.FullName.Remove(type.FullName.IndexOf("`"));
                var arguments = type.GetGenericArguments();
                typeName += "<";
                typeName += string.Join(",", Array.ConvertAll<Type, string>(arguments, x => TypeStringName(x)));
                typeName += ">";
            }
            return typeName;
        }

        /// <summary>
        /// 按顺序加载组件
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TypeInfo[] SortComponent(GameObject target)
        {
            var innercomponentsTypes = new List<TypeInfo>();
            var innercomponents = target.GetComponents<Component>();
            for (int i = 0; i < supportControls.Length; i++)//按指定的顺序添加控件
            {
                if (Array.Find(innercomponents, x => x.GetType() == supportControls[i]))
                {
                    innercomponentsTypes.Add(new TypeInfo(supportControls[i]));
                }
            }

            var userComponentsTypes = target.GetComponents<IUIControl>().Select(x => new TypeInfo(x.GetType()));
            var supportedlist = new List<TypeInfo>();
            supportedlist.AddRange(userComponentsTypes);
            supportedlist.AddRange(innercomponentsTypes);
            supportedlist.Add(new TypeInfo(typeof(GameObject)));
            return supportedlist.ToArray();
        }

        /// <summary>
        /// 选择引用型组件
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="onChoise"></param>
        public static void ChoiseAnReferenceMonobehiver(GameObject prefab, Action<MonoBehaviour> onChoise)
        {
            var behaivers = GetUserReferenceMonobehaiver(prefab);
            if (behaivers != null && behaivers.Length > 0)
            {
                if (behaivers.Count() == 1)
                {
                    onChoise(behaivers[0]);
                }
                else
                {
                    var rect = new Rect(Event.current.mousePosition, new Vector2(0, 0));
                    var options = Array.ConvertAll<MonoBehaviour, GUIContent>(behaivers, x => new GUIContent(x.GetType().FullName));
                    EditorUtility.DisplayCustomMenu(rect, options, -1, new EditorUtility.SelectMenuItemFunction((obj, _options, index) =>
                    {
                        if (index >= 0)
                        {
                            onChoise(behaivers[index]);
                        }
                    }), null);
                }
            }
            else if (behaivers == null || behaivers.Length == 0)
            {
                onChoise(null);
            }
        }

        /// <summary>
        /// 1.如果预制体上有脚本，则保存到预制体脚本所在路径
        /// 2.如果没有脚本，则保存到默认文件夹
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="dress"></param>
        /// <returns></returns>
        public static string InitScriptPath(GameObject prefab, string dress)
        {
            if (prefab == null) return null;

            var prefabName = prefab.name;
            string folder = null;
            var script = GetUserReferenceMonobehaiver(prefab).FirstOrDefault();

            if (script != null)
            {
                folder = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(script));
            }
            else
            {
                folder = string.Format("{0}/{1}", Setting.script_path, prefabName);
            }

            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }
            return string.Format("{0}/{1}{2}.cs", folder, prefabName, dress);
        }

        #region private functions
        /// <summary>
        /// 获取所有引用型组件
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        private static MonoBehaviour[] GetUserReferenceMonobehaiver(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("预制体传入不能为空");
                return null;
            }

            var monobehaivers = prefab.GetComponents<MonoBehaviour>();

            var supported = from behaiver in monobehaivers
                            where behaiver != null
                            where behaiver is IUIPanelReference
                            where !InnerNameSpace.Contains(behaiver.GetType().Namespace)
                            select behaiver;

            if (supported == null || supported.Count() == 0)
            {
                Debug.LogError("预制体上没有引用脚本", prefab);
                return null;
            }

            var mainScript = (from main in supported
                              where MonoScript.FromMonoBehaviour(main).GetClass().Name == prefab.name
                              select main).FirstOrDefault();
            if (mainScript != null)
            {
                return new MonoBehaviour[] { mainScript };
            }
            return supported.ToArray();
        }
       
        /// <summary>
        /// 所有支持的父级
        /// </summary>
        /// <returns></returns>
        private static string[] LoadAllBasePanels()
        {
            var support = new List<Type>();
            var types = typeof(BindingViewBase).Assembly.GetTypes();

            foreach (var item in types)
            {
                var attributes = item.GetCustomAttributes(false);
                if (Array.Find(attributes, x => x is BridgeUI.Attributes.PanelParentAttribute) != null)
                {
                    support.Add(item);
                }
            }
            support.Sort(ComparerBaseTypes);
            return support.ConvertAll(x => x.FullName).ToArray();
        }

        /// <summary>
        /// 类型比较
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static int ComparerBaseTypes(Type x,Type y)
        {
            var att_x = GetAttribute(x);
            var att_y = GetAttribute(y);
            if (att_x.sortIndex > att_y.sortIndex)
            {
                return 1;
            }
            else if (att_x.sortIndex < att_y.sortIndex)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取指定类型的特性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static BridgeUI.Attributes.PanelParentAttribute GetAttribute(Type type)
        {
            var attributes = type.GetCustomAttributes(false);
            var attribute = Array.Find(attributes, x => x is BridgeUI.Attributes.PanelParentAttribute);
            return attribute as BridgeUI.Attributes.PanelParentAttribute;
        }

        /// <summary>
        /// 异步从已经存在的脚本加载
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="nameSpace"></param>
        /// <param name="onGet"></param>
        private static void LoadBindingScriptAsnyc(GameObject prefab, List<ComponentItem> components, Action<ViewCoder> onGet)
        {
            ChoiseAnReferenceMonobehiver(prefab, (x) =>
            {
                ViewCoder coder = new ViewCoder();
                coder.className = prefab.name + "_Internal";
                coder.refClassName = prefab.name + "_Reference";
                coder.nameSpace = Setting.defultNameSpace;
                coder.path = InitScriptPath(prefab, "_Internal");

                var referenceItems = WorpReferenceItems(components);

                if (x != null)
                {
                    var refClassType = x.GetType();
                    coder.nameSpace = refClassType.Namespace;
                    coder.refClassName = refClassType.Name;

                    var dataField = refClassType.GetField("m_data", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);
                    if (dataField != null)
                    {
                        coder.innerFields = dataField.FieldType.GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.Instance);
                    }
                    onGet(coder);
                }
                else
                {

                    var script = BindingReferenceEditor.CreateScript(coder.nameSpace, coder.refClassName, referenceItems);
                    var path = InitScriptPath(prefab, "_Reference");
                    System.IO.File.WriteAllText(path, script);

                    var scriptType = typeof(BridgeUI.IUIPanelReference).Assembly.GetType(coder.nameSpace + "." + coder.refClassName);
                    if (scriptType != null)
                    {
                        prefab.AddComponent(scriptType);
                    }

                    prefab.AddComponent<ReferenceCatchBehaiver>().SetReferenceItems(referenceItems);
                }

                onGet(coder);

            });
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        private static List<ReferenceItem> WorpReferenceItems(List<ComponentItem> components)
        {
            var referenceItems = new List<ReferenceItem>();
            for (int i = 0; i < components.Count; i++)
            {
                var component = components[i];
                var item = new ReferenceItem();
                item.isArray = false;
                if (component.componentType == typeof(GameObject))
                {
                    item.referenceTarget = component.target;
                }
                else if (typeof(Component).IsAssignableFrom(component.componentType))
                {
                    if (component.target != null)
                    {
                        item.referenceTarget = component.target.GetComponent(component.componentType);
                    }
                }
                item.type = component.componentType;
                item.name = component.name;
                referenceItems.Add(item);
            }
            return referenceItems;
        }


        /// <summary>
        /// 过虑已经存在的变量
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="components"></param>
        /// <returns></returns>
        private static ComponentItem[] FilterExisField(string typeName, List<ComponentItem> components)
        {
            var type = typeof(BindingViewBase).Assembly.GetType(typeName);
            var list = new List<ComponentItem>();
            if (type != null)
            {
                foreach (var item in components)
                {
                    if (type.GetField("m_" + item.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField) == null)
                    {
                        list.Add(item);
                    }
                }
                return list.ToArray();
            }
            else
            {
                return components.ToArray();
            }
        }

        /// <summary>
        /// 按优先级获取类型
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="membername"></param>
        /// <returns></returns>
        public static Type GetTypeClamp(Type baseType, string membername)
        {
            var flag = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
            Type infoType = null;
            var prop = baseType.GetProperty(membername, System.Reflection.BindingFlags.GetProperty | flag);
            if (prop != null)
            {
                infoType = prop.PropertyType;
            }
            var field = baseType.GetField(membername, System.Reflection.BindingFlags.GetField | flag);
            if (field != null)
            {
                infoType = field.FieldType;
            }
            try
            {
                var members = baseType.GetMember(membername, BindingFlags.FlattenHierarchy | flag);
                for (int i = 0; i < members.Length; i++)
                {
                    var member = members[i];
                    if (member is MethodInfo)
                    {
                        var func = member as MethodInfo;
                        if (func != null && func.GetParameters().Count() == 1)
                        {
                            infoType = func.GetParameters()[0].ParameterType;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return infoType;
        }
        #endregion


    }
}