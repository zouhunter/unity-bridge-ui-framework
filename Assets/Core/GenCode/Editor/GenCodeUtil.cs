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

namespace BridgeUI
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
        private const string initcomponentMethod = "InitComponents";
        private const string propbindingsMethod = "PropBindings";

        static GenCodeUtil()
        {
            supportBaseTypes = LoadAllBasePanels();
        }
        /// <summary>
        /// 创建编码字典
        /// </summary>
        /// <returns></returns>
        private static Dictionary<Type, ComponentCode> CreateCoderDic()
        {
            var dic = new Dictionary<Type, ComponentCode>();
            dic.Add(typeof(Button), new ButtonCode());
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

        public static UICoder LoadUICoder(GameObject prefab, GenCodeRule rule)
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
        private static void CompleteMethods(TypeDeclaration classNode, GenCodeRule rule)
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

        private static TypeDeclaration CompleteClass(SyntaxTree tree, GameObject prefab, GenCodeRule rule)
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

        private static void CompleteNameSpace(SyntaxTree tree, GenCodeRule rule)
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

        public static void CreateScript(GameObject go, List<ComponentItem> components, UICoder uiCoder, GenCodeRule rule)
        {
            var baseType = GenCodeUtil.supportBaseTypes[rule.baseTypeIndex];

            var needAdd = FilterExisField(baseType, components);

            var tree = uiCoder.tree;
            var className = uiCoder.className;
            var classNode = tree.Descendants.OfType<TypeDeclaration>().Where(x => x.Name == className).First();

            TryAddInfoToUnit(classNode, needAdd);
            CompleteMethods(classNode, rule);
            BindingInfoToUnit(classNode, needAdd);
            SortClassMembers(classNode);

            var scriptPath = GetScriptPath(go);
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

        private static string GetScriptPath(GameObject go)
        {
            var path = AssetDatabase.GetAssetPath(go).Replace(".prefab", ".cs");
            return path;
        }

        /// <summary>
        /// 对类中的元素进行一定的排序
        /// </summary>
        /// <returns></returns>
        private static void SortClassMembers(TypeDeclaration classNode)
        {
            var fields = classNode.Descendants.OfType<FieldDeclaration>().ToArray();
            var first = fields.FirstOrDefault();
            foreach (var item in fields)
            {
                if(item != first)
                {
                    classNode.Members.Remove(item);
                    classNode.InsertChildAfter(first, item, Roles.TypeMemberRole);
                }

            }
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

        private static void TryAddInfoToUnit(TypeDeclaration classNode, ComponentItem[] components)
        {
            foreach (var item in components)
            {
                var fieldName = string.Format("m_" + item.name);

                if (classNode.Descendants.OfType<FieldDeclaration>().Where(x => x.Variables.Where(y => y.Name == fieldName).Count() > 0).Count() == 0)
                {
                    var field = new FieldDeclaration();
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
        /// 绑定方法体
        /// </summary>
        /// <param name="classNode"></param>
        /// <param name="components"></param>
        private static void BindingInfoToUnit(TypeDeclaration classNode, ComponentItem[] components)
        {
            var InitComponentsNode = classNode.Descendants.OfType<MethodDeclaration>().Where(x => x.Name == initcomponentMethod).FirstOrDefault();
            var PropBindingsNode = classNode.Descendants.OfType<MethodDeclaration>().Where(x => x.Name == propbindingsMethod).FirstOrDefault();

            foreach (var component in components)
            {
                if (!component.binding)
                {
                    var invocations = InitComponentsNode.Body.Descendants.OfType<InvocationExpression>();
                    var invocation = invocations.Where(x => x.Target.ToString().Contains("m_" + component.name) && x.Arguments.Where(ag => ag.ToString() == component.sourceName) != null).FirstOrDefault();
                    var methodName = GetMethodName_InitComponentsNode(component.componentType);
                    if (invocation == null && !string.IsNullOrEmpty(methodName) && !string.IsNullOrEmpty(component.sourceName))
                    {
                        invocation = new InvocationExpression();
                        invocation.Target = new MemberReferenceExpression(new MemberReferenceExpression(new IdentifierExpression("m_" + component.name), methodName, new AstType[0]), "AddListener", new AstType[0]);
                        invocation.Arguments.Add(new IdentifierExpression(component.sourceName));
                        InitComponentsNode.Body.Add(invocation);
                        CompleteMethod(classNode, component);
                    }
                }
                else
                {
                    var invocations = PropBindingsNode.Body.Descendants.OfType<InvocationExpression>();
                    var invocation = invocations.Where(x => x.Target.ToString().Contains("Binder") && x.Arguments.Count > 0 && x.Arguments.First().ToString().Contains("m_" + component.name)).FirstOrDefault();
                    if (invocation == null)
                    {
                        var methodName = GetMethodNameFromComponent(component.componentType);
                        if (!string.IsNullOrEmpty(methodName))
                        {
                            invocation = new InvocationExpression();
                            invocation.Target = new MemberReferenceExpression(new IdentifierExpression("Binder"), methodName, new AstType[0]);
                            invocation.Arguments.Add(new IdentifierExpression("m_" + component.name));
                            invocation.Arguments.Add(new PrimitiveExpression(component.sourceName));
                            PropBindingsNode.Body.Add(invocation);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 完善方法
        /// </summary>
        /// <param name="classNode"></param>
        /// <param name="item"></param>
        private static void CompleteMethod(TypeDeclaration classNode, ComponentItem item)
        {
            var funcNode = classNode.Descendants.OfType<MethodDeclaration>().Where(x => x.Name == item.sourceName).FirstOrDefault();
            if (funcNode == null)
            {
                var argument = GetArgument_InitComponentsNode(item.componentType);
                if (argument != null && !InnerFunctions.Contains(item.sourceName))
                {
                    funcNode = new MethodDeclaration();
                    funcNode.Name = item.sourceName;
                    funcNode.Modifiers |= Modifiers.Protected;
                    funcNode.ReturnType = new ICSharpCode.NRefactory.CSharp.PrimitiveType("void");
                    funcNode.Parameters.Add(argument);
                    funcNode.Body = new BlockStatement();
                    classNode.AddChild(funcNode, Roles.TypeMemberRole);
                }

            }
        }
        /// <summary>
        /// 获取不同组件的事件名
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private static string GetMethodName_InitComponentsNode(Type componentType)
        {
            if (componentType == typeof(Button))
            {
                return "onClick";
            }
            else if (componentType == typeof(Toggle))
            {
                return "onValueChanged";
            }
            else if (componentType == typeof(Slider))
            {
                return "onValueChanged";
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 不同的组件参数不同
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        private static ParameterDeclaration GetArgument_InitComponentsNode(Type componentType)
        {
            if (componentType == typeof(Button))
            {
                return new ParameterDeclaration();
            }
            else if (componentType == typeof(Toggle))
            {
                return new ParameterDeclaration(new PrimitiveType("bool"), "isOn");
            }
            else if (componentType == typeof(Slider))
            {
                return new ParameterDeclaration(new PrimitiveType("float"), "value");
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 不同的组件注册方法名不同
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        private static string GetMethodNameFromComponent(Type componentType)
        {
            if (componentType == typeof(Button))
            {
                return "RegistButtonEvent";
            }
            else if (componentType == typeof(Toggle))
            {
                return "RegistToggleEvent";
            }
            else if (componentType == typeof(Slider))
            {
                return "RegistSliderEvent";
            }
            else if (componentType == typeof(Image))
            {
                return "RegistImageView";
            }
            else if (componentType == typeof(RawImage))
            {
                return "RegistRawImageView";
            }
            else if (componentType == typeof(Text))
            {
                return "RegistTextView";
            }
            else
            {
                return "";
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
        /// 分析组件的绑定信息
        /// </summary>
        /// <param name="component"></param>
        /// <param name="components"></param>
        private static void AnalysisBindings(PanelBase component, List<ComponentItem> components)
        {
            var script = MonoScript.FromMonoBehaviour(component).text;
            var tree = new CSharpParser().Parse(script);

            var classNode = tree.Descendants.OfType<TypeDeclaration>().Where(x => x.Name == component.GetType().Name).FirstOrDefault();
            if (classNode != null)
            {
                var InitComponentsNode = classNode.Descendants.OfType<MethodDeclaration>().Where(x => x.Name == initcomponentMethod).FirstOrDefault();
                if (InitComponentsNode != null)
                {
                    var invctions = InitComponentsNode.Body.Descendants.OfType<InvocationExpression>();
                    foreach (var item in invctions)
                    {
                        var com = components.Find(x => item.Target.ToString().Contains("m_" + x.name));
                        if (com != null)
                        {
                            com.sourceName = item.Arguments.First().ToString();
                            com.binding = false;
                        }

                    }
                }
                var PropBindingsNode = classNode.Descendants.OfType<MethodDeclaration>().Where(x => x.Name == propbindingsMethod).First();
                if (PropBindingsNode != null)
                {
                    var invctions = PropBindingsNode.Body.Descendants.OfType<InvocationExpression>();
                    foreach (var item in invctions)
                    {
                        var com = components.Find(x => item.Arguments.Count > 1 && item.Arguments.First().ToString().Contains("m_" + x.name));
                        if (com != null)
                        {
                            var at = item.Arguments.ToArray();
                            com.sourceName = at[1].ToString().Replace("\"", "");
                            com.binding = true;
                        }
                    }
                }
            }
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

    }
}