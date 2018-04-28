using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEditor;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using BridgeUI.Model;
using System.Reflection;
using System.Linq;
using PrimitiveType = ICSharpCode.NRefactory.CSharp.PrimitiveType;
namespace BridgeUI.CodeGen
{
    public static class GenCodeUtil
    {
        public static Type[] supportControls =  {
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
        public static List<string> InnerFunctions = new List<string>()
        {
            "Close",
        };
        public static string[] supportBaseTypes;
        public const string initcomponentMethod = "InitComponents";
        public const string propbindingsMethod = "PropBindings";
        public static ComponentCoder componentCoder = new ComponentCoder();
        static GenCodeUtil()
        {
            supportBaseTypes = LoadAllBasePanels();
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
        /// <summary>
        /// 分析代码的的组件信息
        /// </summary>
        /// <param name="component"></param>
        /// <param name="components"></param>
        public static void AnalysisComponent(PanelBase component, List<ComponentItem> components)
        {
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
            AnalysisBindings(component, components);
        }

        /// <summary>
        /// 创建代码
        /// </summary>
        /// <param name="go"></param>
        /// <param name="components"></param>
        /// <param name="rule"></param>
        public static void CreateScript(GameObject go, List<ComponentItem> components, GenCodeRule rule)
        {
            var uiCoder = GenCodeUtil.LoadUICoder(go, rule);

            var baseType = GenCodeUtil.supportBaseTypes[rule.baseTypeIndex];

            var needAdd = FilterExisField(baseType, components);

            var tree = uiCoder.tree;
            var className = uiCoder.className;
            var classNode = tree.Descendants.OfType<TypeDeclaration>().Where(x => x.Name == className).First();

            CreateMemberFields(classNode, needAdd);
            CompleteBaseMethods(classNode, rule);
            BindingInfoMethods(classNode, needAdd);
            SortClassMembers(classNode);

            var scriptPath = AssetDatabase.GetAssetPath(go).Replace(".prefab", ".cs");

            System.IO.File.WriteAllText(scriptPath, uiCoder.Compile());
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

        /// <summary>
        /// 按顺序加载组件
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TypeInfo[] SortComponent(GameObject target)
        {
            var components = new List<UnityEngine.Object>();
            components.Add(target);
            components.AddRange(target.GetComponents<Component>());

            var list = new List<TypeInfo>();
            var types = supportControls;
            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (components.Find(x => x.GetType() == type))
                {
                    list.Add(new TypeInfo(type));
                }
            }
            var endList = components.Where(x => !supportControls.Contains(x.GetType())).Select(x=> new TypeInfo(x.GetType()));
            list.AddRange(endList);
            return list.ToArray();
        }

        #region private functions


     

        /// <summary>
        /// 所有支持的父级
        /// </summary>
        /// <returns></returns>
        private static string[] LoadAllBasePanels()
        {
            var types = typeof(PanelBase).Assembly.GetTypes();
            var support = new List<string>();
            foreach (var item in types)
            {
                var attributes = item.GetCustomAttributes(false);
                if (Array.Find(attributes, x => x is PanelParentAttribute) != null)
                {
                    if (!support.Contains(item.FullName))
                    {
                        support.Add(item.FullName);
                    }
                }
            }
            return support.ToArray();
        }

        private static UICoder LoadUICoder(GameObject prefab, GenCodeRule rule)
        {
            UICoder coder = new UICoder(prefab.name);
            //加载已经存在的脚本
            LoadOldScript(prefab, coder);
            //添加命名空间和引用
            CompleteNameSpace(coder.tree, rule);
            //添加类
            CompleteClass(coder.tree, prefab, rule);
            return coder;
        }
        private static void LoadOldScript(GameObject prefab, UICoder coder)
        {
            var oldScript = prefab.GetComponent<PanelBase>();
            if (oldScript != null)
            {
                var path = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(oldScript));
                coder.Load(System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8));
            }
        }

        /// <summary>
        /// 完善方法
        /// </summary>
        /// <param name="classNode"></param>
        /// <param name="rule"></param>
        private static void CompleteBaseMethods(TypeDeclaration classNode, GenCodeRule rule)
        {
            var InitComponentsNode = classNode.Descendants.OfType<MethodDeclaration>().Where(x => x.Name == initcomponentMethod).FirstOrDefault();
            var PropBindingsNode = classNode.Descendants.OfType<MethodDeclaration>().Where(x => x.Name == propbindingsMethod).FirstOrDefault();

            if (InitComponentsNode == null)
            {
                InitComponentsNode = new MethodDeclaration();
                InitComponentsNode.Name = initcomponentMethod;
                InitComponentsNode.Modifiers |= Modifiers.Protected | Modifiers.Override;
                InitComponentsNode.ReturnType = new ICSharpCode.NRefactory.CSharp.PrimitiveType("void");
                InitComponentsNode.Body = new BlockStatement();
                classNode.AddChild(InitComponentsNode, Roles.TypeMemberRole);
            }

            if (PropBindingsNode == null)
            {
                PropBindingsNode = new MethodDeclaration();
                PropBindingsNode.Name = propbindingsMethod;
                PropBindingsNode.Modifiers |= Modifiers.Protected | Modifiers.Override;
                PropBindingsNode.ReturnType = new ICSharpCode.NRefactory.CSharp.PrimitiveType("void");
                PropBindingsNode.Body = new BlockStatement();
                classNode.AddChild(PropBindingsNode, Roles.TypeMemberRole);
            }
        }

        /// <summary>
        /// 完善类名
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="prefab"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        private static TypeDeclaration CompleteClass(ICSharpCode.NRefactory.CSharp.SyntaxTree tree, GameObject prefab, GenCodeRule rule)
        {
            TypeDeclaration classNode = null;
            var className = prefab.name;
            if (tree.Descendants.OfType<TypeDeclaration>().Where(x => x.Name == className).Count() == 0)
            {
                classNode = new TypeDeclaration();
                classNode.Name = className;
                classNode.Modifiers |= Modifiers.Public;

                var comment = new Comment("<summary>", CommentType.Documentation);
                tree.AddChild(comment, Roles.Comment);
                comment = new Comment("[代码说明信息]", CommentType.Documentation);
                tree.AddChild(comment, Roles.Comment);
                comment = new Comment("<summary>", CommentType.Documentation);
                tree.AddChild(comment, Roles.Comment);
                tree.AddChild(classNode, Roles.TypeMemberRole);
            }

            classNode = tree.Descendants.OfType<TypeDeclaration>().Where(x => x.Name == className).First();
            var baseType = GenCodeUtil.supportBaseTypes[rule.baseTypeIndex];
            var basePanels = LoadAllBasePanels();
            var bs = classNode.BaseTypes.Where(x => Array.Find(basePanels, y => x.ToString() == y) != null).FirstOrDefault();
            if (bs != null)
            {
                classNode.BaseTypes.Remove(bs);
            }
            classNode.BaseTypes.Add(new SimpleType(baseType));

            return classNode;
        }

        /// <summary>
        /// 完善命名空间
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="rule"></param>
        private static void CompleteNameSpace(ICSharpCode.NRefactory.CSharp.SyntaxTree tree, GenCodeRule rule)
        {
            string[] usingDeclarations = {
                "BridgeUI",
                "UnityEngine",
                "UnityEngine.UI",
                 "System.Collections",
                "System.Collections.Generic",
            };
            foreach (var item in usingDeclarations)
            {
                if (tree.Descendants.OfType<UsingDeclaration>().Where(x => x.Namespace == item).Count() == 0)
                {
                    tree.AddChild<AstNode>(new UsingDeclaration(item), Roles.Root);
                }
            }
        }

        /// <summary>
        /// 对类中的元素进行一定的排序
        /// </summary>
        /// <returns></returns>
        private static void SortClassMembers(TypeDeclaration classNode)
        {
            var fields = classNode.Descendants.OfType<FieldDeclaration>().ToArray();
            AstNode keyNode = fields.FirstOrDefault();
            foreach (var item in fields)
            {
                if (item != keyNode)
                {
                    classNode.Members.Remove(item);
                    classNode.InsertChildAfter(keyNode, item, Roles.TypeMemberRole);
                    keyNode = item;
                }
            }
            var InitComponentsNode = classNode.Descendants.OfType<MethodDeclaration>().Where(x => x.Name == initcomponentMethod).FirstOrDefault();
            var PropBindingsNode = classNode.Descendants.OfType<MethodDeclaration>().Where(x => x.Name == propbindingsMethod).FirstOrDefault();

            classNode.Members.Remove(InitComponentsNode);
            classNode.InsertChildAfter(keyNode, InitComponentsNode, Roles.TypeMemberRole);
            keyNode = InitComponentsNode;

            classNode.Members.Remove(PropBindingsNode);
            classNode.InsertChildAfter(keyNode, PropBindingsNode, Roles.TypeMemberRole);
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

        /// <summary>
        /// 创建字段列表
        /// </summary>
        /// <param name="classNode"></param>
        /// <param name="components"></param>
        private static void CreateMemberFields(TypeDeclaration classNode, ComponentItem[] components)
        {
            foreach (var item in components)
            {
                var fieldName = string.Format("m_" + item.name);
                var field = classNode.Descendants.OfType<FieldDeclaration>().Where(x => x.Variables.Where(y => y.Name == fieldName).Count() > 0).FirstOrDefault();
               
                if (field != null && (field.ReturnType).ToString() != item.componentType.Name)
                {
                    classNode.Members.Remove(field);
                    field = null;
                }

                if (field == null)
                {
                    field = new FieldDeclaration();
                    field.Modifiers = Modifiers.Private;
                    field.ReturnType = new ICSharpCode.NRefactory.CSharp.PrimitiveType(item.componentType.FullName);
                    field.Variables.Add(new VariableInitializer(fieldName));
                    var att = new ICSharpCode.NRefactory.CSharp.Attribute();
                    att.Type = new SimpleType("SerializeField");
                    field.Attributes.Add(new AttributeSection(att));
                    classNode.AddChild(field, Roles.TypeMemberRole);
                }
            }
        }
        
        /// <summary>
        /// 完善方法内容
        /// </summary>
        /// <param name="classNode"></param>
        /// <param name="components"></param>
        private static void BindingInfoMethods(TypeDeclaration classNode, ComponentItem[] components)
        {
            componentCoder.SetContext(classNode);
            foreach (var component in components)
            {
                componentCoder.CompleteCode(component);
            }
        }

        /// <summary>
        /// 分析组件的绑定信息
        /// </summary>
        /// <param name="component"></param>
        /// <param name="components"></param>
        private static void AnalysisBindings(PanelBase component, List<ComponentItem> components)
        {
            var script = MonoScript.FromMonoBehaviour(component).text;
            var tree = new CSharpParser().Parse(script);
            var classNode = tree.Descendants.OfType<TypeDeclaration>().Where(x => x.Name == component.GetType().Name).FirstOrDefault();
            componentCoder.SetContext(classNode);
            componentCoder.AnalysisBinding(components);
        }

        #endregion


    }
}