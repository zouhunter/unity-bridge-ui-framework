using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.CodeDom;
using BridgeUI.Model;
using System.Reflection;


namespace BridgeUI
{

    public static class GenCodeUtil
    {
        public static Type[] supportControls = new Type[] {
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

        public static string[] supportBaseTypes;

        private static Dictionary<Type, ComponentCode> componentCoder;
        static GenCodeUtil()
        {
            supportBaseTypes = LoadAllBasePanels();
            componentCoder = CreateCoderDic();
        }
        /// <summary>
        /// 创建编码字典
        /// </summary>
        /// <returns></returns>
        private static Dictionary<Type, ComponentCode> CreateCoderDic()
        {
            var dic = new Dictionary<Type, ComponentCode>();
            dic.Add(typeof(Button),new ButtonCode());
            return dic;
        }

        /// <summary>
        /// 按顺序加载组件
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Model.TypeRecod[] SortComponent(GameObject target)
        {
            var components = new List<Component>();
            components.AddRange(target.GetComponents<Component>());
            var list = new List<Model.TypeRecod>();
            var types = supportControls;
            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (components.Find(x => x.GetType() == type))
                {
                    list.Add(new Model.TypeRecod(type));
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 所有支持的父级
        /// </summary>
        /// <returns></returns>
        public static string[] LoadAllBasePanels()
        {
            var types = typeof(PanelBase).Assembly.GetTypes();
            var support = new List<string>() { typeof(PanelBase).FullName, typeof(SinglePanel).FullName, typeof(GroupPanel).FullName, typeof(SingleCloseAblePanel).FullName };
            foreach (var item in types)
            {
                if (typeof(PanelBase).IsAssignableFrom(item) && !item.IsSealed)
                {
                    if (!support.Contains(item.FullName))
                    {
                        support.Add(item.FullName);
                    }
                }
            }
            return support.ToArray();
        }

        public static UICoder LoadUICoder(GameObject prefab, GenCodeRule rule)
        {
            UICoder coder = new UICoder();
            var oldScript = prefab.GetComponent<PanelBase>();
            if (oldScript != null)
            {
                var path = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(oldScript));
                coder.Load(System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8));
            }
            var nameSpace = TryAddNameSpace(coder.unit, rule);
            CodeTypeDeclaration classItem = TryInitClass(nameSpace, prefab, rule);
            TryCreateMethods(classItem,rule);
            return coder;
        }

        /// <summary>
        /// 定义一些模板方法
        /// </summary>
        /// <param name="classItem"></param>
        /// <param name="rule"></param>
        private static void TryCreateMethods(CodeTypeDeclaration classItem, GenCodeRule rule)
        {
            
        }

        private static CodeTypeDeclaration TryInitClass(CodeNamespace nameSpace, GameObject prefab, GenCodeRule rule)
        {
            //var className = prefab.name;
            CodeTypeDeclaration type = null;
            if (nameSpace.Types.Count == 0)
            {
                type = new CodeTypeDeclaration(prefab.name);

                type.IsPartial = true;
                nameSpace.Types.Add(type);
            }
            else
            {
                type = nameSpace.Types[0];
            }

            if (!rule.canInherit)
                type.TypeAttributes |= TypeAttributes.Sealed;

            type.BaseTypes.Clear();
            var baseType = GenCodeUtil.supportBaseTypes[ rule.baseTypeIndex];
            type.BaseTypes.Add(new CodeTypeReference(baseType));
            return nameSpace.Types[0];
        }

        private static CodeNamespace TryAddNameSpace(CodeCompileUnit unit, GenCodeRule rule)
        {
            if (unit.Namespaces.Count == 0)
            {
                unit.Namespaces.Add(new CodeNamespace(""));
            }
            else
            {
                unit.Namespaces[0].Name = "";
            }

            //string[] assebles = {
            //    "BridgeUI",
            //    "UnityEngine",
            //    "UnityEngine.UI",
            //     "System.Collections",
            //    "System.Collections.Generic",
            //};

            //foreach (var item in assebles)
            //{
            //    unit.Namespaces[0].Imports.Add(new CodeNamespaceImport(item));
            //}

            return unit.Namespaces[0];

        }
        public static void CreateScript(GameObject go, List<ComponentItem> components, UICoder uiCoder, GenCodeRule rule)
        {
            var baseType = GenCodeUtil.supportBaseTypes[rule.baseTypeIndex];

            var needAdd = FilterExisField(baseType, components);
            TryAddInfoToUnit(uiCoder.unit.Namespaces[0].Types[0], needAdd);

            var scriptPath = GetScriptPath(go);
            System.IO.File.WriteAllText(scriptPath, uiCoder.Compile());
            string className = go.name;
            var type = typeof(BridgeUI.PanelBase).Assembly.GetType(className);
            if (type != null)
            {
                if (go.GetComponent(type) == null)
                {
                    go.AddComponent(type);
                }
            }
            EditorApplication.delayCall += () =>
            {
                AssetDatabase.Refresh();
            };
        }
        private static string GetScriptPath(GameObject go)
        {
            var path = AssetDatabase.GetAssetPath(go).Replace(".prefab", ".cs");
            return path;
        }
        /// <summary>
        /// 过虑已经存在的变量
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="components"></param>
        /// <returns></returns>
        private static ComponentItem[] FilterExisField(string typeName, List<ComponentItem> components)
        {
            var type = typeof(PanelBase).Assembly.GetType(typeName);
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

        private static void TryAddInfoToUnit(CodeTypeDeclaration codeClass, ComponentItem[] components)
        {
            foreach (var item in components)
            {
                var fieldName = string.Format("m_" + item.name);
                var field = SuarchField(codeClass.Members, fieldName);
                if (field == null)
                {
                    field = new CodeMemberField();
                    field.Attributes = MemberAttributes.Private;
                    codeClass.Members.Add(field);
                }
                field.Type = new CodeTypeReference(item.componentType, CodeTypeReferenceOptions.GenericTypeParameter);
                if (field.CustomAttributes.SuarchAttribute("SerializeField") == null)
                {
                    field.CustomAttributes.Add(new CodeAttributeDeclaration("UnityEngine.SerializeField"));
                }
                field.Name = fieldName;
            }
        }
        /// <summary>
        /// 查看字段
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        private static CodeMemberField SuarchField(this CodeTypeMemberCollection collection, string Name)
        {
            foreach (var item in collection)
            {
                if (item is CodeMemberField)
                {
                    if ((item as CodeMemberField).Name == Name)
                    {
                        return (item as CodeMemberField);
                    }
                }
            }
            return null;
        }

        public static void AnalysisComponent(GameObject prefab, List<ComponentItem> components)
        {
            var component = prefab.GetComponent<PanelBase>();
            if (component == null)
            {
                EditorApplication.Beep();
                return;
            }

            var fields = component.GetType().GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (typeof(MonoBehaviour).IsAssignableFrom(field.FieldType))
                {
                    var compItem = components.Find(x => "m_" + x.name == field.Name || x.name == field.Name);

                    if (compItem == null)
                    {
                        compItem = new ComponentItem();
                        compItem.name = field.Name.Replace("m_", "");
                        components.Add(compItem);
                    }
                    var value = field.GetValue(component);
                    if (value != null)
                    {
                        if (field.FieldType == typeof(GameObject))
                        {
                            compItem.target = value as GameObject;
                        }
                        else
                        {
                            compItem.target = (value as MonoBehaviour).gameObject;
                        }

                        compItem.components = SortComponent(compItem.target);
                    }
                }
            }
        }

        /// <summary>
        /// 查找属性
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        private static CodeAttributeDeclaration SuarchAttribute(this CodeAttributeDeclarationCollection collection, string Name)
        {
            foreach (var item in collection)
            {
                if (item is CodeAttributeDeclaration)
                {
                    if ((item as CodeAttributeDeclaration).Name == Name)
                    {
                        return (item as CodeAttributeDeclaration);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 快速进行控件绑定
        /// </summary>
        /// <param name="go"></param>
        /// <param name="components"></param>
        public static void BindingUI(GameObject go, List<ComponentItem> components)
        {
            if (go == null || go.GetComponent<PanelBase>() == null)
            {
                EditorApplication.Beep();
                return;
            }
            var behaiver = go.GetComponent<PanelBase>();
            foreach (var item in components)
            {
                var filedName = "m_" + item.name;
                UnityEngine.Object obj = item.target;
                if (item.componentType != typeof(GameObject))
                {
                    obj = item.target.GetComponent(item.componentType);
                }
                behaiver.GetType().InvokeMember(filedName,
                                BindingFlags.SetField |
                                BindingFlags.Instance |
                                BindingFlags.NonPublic,
                                null, behaiver, new object[] { obj }, null, null, null);
            }
        }


        [InitializeOnLoadMethod]
        public static void HandleOnReImport()
        {

        }

    }
}