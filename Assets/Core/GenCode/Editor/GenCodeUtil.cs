using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

        private static List<string> InnerNameSpace = new List<string>()
        {
            "UnityEngine.UI",
            "UnityEngine",
            "UnityEngine.EventSystems"
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
        public static void BindingUI(MonoBehaviour behaiver, List<ComponentItem> components)
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
                Debug.Log(filedName);
                Debug.Log(behaiver);
                Debug.Log(obj);

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
        public static void AnalysisComponent(MonoBehaviour component, List<ComponentItem> components)
        {
            var fields = component.GetType().GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (typeof(UnityEngine.MonoBehaviour).IsAssignableFrom(field.FieldType) || typeof(ScriptableObject).IsAssignableFrom(field.FieldType))
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
                            compItem.components = SortComponent(compItem.target);
                            compItem.componentID = Array.IndexOf(compItem.components, typeof(GameObject));
                        }
                        else if (typeof(ScriptableObject).IsAssignableFrom(field.FieldType))
                        {
                            compItem.UpdateAsScriptObject(value as ScriptableObject);
                        }
                        else
                        {
                            compItem.target = (value as MonoBehaviour).gameObject;
                            compItem.components = SortComponent(compItem.target);
                            compItem.componentID = Array.IndexOf(Array.ConvertAll(compItem.components, x => x.type), value.GetType());
                        }
                    }
                }
            }
            AnalysisBindings(component, components, new GenCodeRule());
        }

        /// <summary>
        /// 更新View 和 ViewModel的脚本
        /// </summary>
        /// <param name="go"></param>
        /// <param name="components"></param>
        /// <param name="rule"></param>
        public static void UpdateScripts(GameObject go, List<ComponentItem> components, GenCodeRule rule)
        {
            rule.onGenerated = (viewScript) =>
            {
                if (viewScript is PanelBase)
                {
                    var viewModel = (viewScript as PanelBase).ViewModel;
                    if (viewModel)
                    {
                        GenCodeUtil.UpdateViewModelScript(viewModel, components);
                    }
                }
            };
            GenCodeUtil.CreateViewScript(go, components, rule);
        }

        /// <summary>
        /// 创建代码
        /// </summary>
        /// <param name="go"></param>
        /// <param name="components"></param>
        /// <param name="rule"></param>
        public static void CreateViewScript(GameObject go, List<ComponentItem> components, GenCodeRule rule)
        {
            Action<UICoder> onLoad = (uiCoder) =>
            {
                var baseType = GenCodeUtil.supportBaseTypes[rule.baseTypeIndex];

                var needAdd = FilterExisField(baseType, components);

                var tree = uiCoder.tree;
                var className = uiCoder.className;
                var classNode = tree.Descendants.OfType<TypeDeclaration>().Where(x => x.Name == className).First();

                CreateMemberFields(classNode, needAdd);
                BindingInfoMethods(classNode, needAdd, rule);
                SortClassMembers(classNode);

                var prefabPath = AssetDatabase.GetAssetPath(go);
                var folder = prefabPath.Remove(prefabPath.LastIndexOf("/"));
                var scriptPath = string.Format("{0}/{1}.cs", folder, uiCoder.className);
                var scriptValue = uiCoder.Compile();
                System.IO.File.WriteAllText(scriptPath, scriptValue, System.Text.Encoding.UTF8);
                AssetDatabase.Refresh();

                EditorApplication.delayCall += () =>
                {
                    var type = typeof(BridgeUI.PanelBase).Assembly.GetType(className);
                    if (type != null)
                    {
                        var script = go.GetComponent(type);
                        if (script == null)
                        {
                            go.AddComponent(type);
                        }
                        if (rule.onGenerated != null)
                        {
                            rule.onGenerated.Invoke(script);
                        }
                        EditorApplication.update = null;
                    }
                    AssetDatabase.Refresh();
                };
            };

            GenCodeUtil.LoadViewScriptCoder(go, rule, onLoad);
        }

        /// <summary>
        /// 更新viewModelScript
        /// </summary>
        /// <param name="script"></param>
        /// <param name="components"></param>
        public static void UpdateViewModelScript(Binding.ViewModel viewModel, List<ComponentItem> components)
        {
            var monoScript = MonoScript.FromScriptableObject(viewModel);
            var className = monoScript.GetClass().Name;
            UICoder oldViewModel = new UICoder(className);
            oldViewModel.Load(monoScript.text);
            var tree = oldViewModel.tree;
            var classNode = tree.Descendants.OfType<TypeDeclaration>().Where(x => x.Name == className).First();
            AstNode lastNode = classNode.Descendants.OfType<PreProcessorDirective>().FirstOrDefault();
            if (lastNode == null)
            {
                lastNode = classNode.Descendants.OfType<PropertyDeclaration>().FirstOrDefault();
            }

            foreach (var component in components)
            {
                foreach (var viewItem in component.viewItems)
                {
                    if (viewItem.bindingTargetType.type != null)
                    {
                        lastNode = InsertPropertyToClassNode(viewItem.bindingTargetType.type, viewItem.bindingSource, lastNode, classNode);
                    }
                    else
                    {
                        Debug.LogError(viewItem.bindingSource + ":type NULL");
                    }

                }

                foreach (var eventItem in component.eventItems)
                {
                    if (eventItem.type == BindingType.NoBinding) continue;
                    var type = eventItem.bindingTargetType.type;
                    if (type != null)
                    {
                        Type typevalue = null;

                        if (eventItem.type == BindingType.WithTarget)
                        {
                            typevalue = typeof(Binding.PanelAction<>).MakeGenericType(component.componentType);
                        }
                        else
                        {
                            if (type.BaseType.IsGenericType)
                            {
                                var argument = type.BaseType.GetGenericArguments();
                                if (argument.Length == 1)
                                {
                                    typevalue = typeof(Binding.PanelAction<>).MakeGenericType(argument);
                                }
                                else if (argument.Length == 2)
                                {
                                    typevalue = typeof(Binding.PanelAction<,>).MakeGenericType(argument);
                                }
                                else if (argument.Length == 3)
                                {
                                    typevalue = typeof(Binding.PanelAction<,,>).MakeGenericType(argument);
                                }
                                else if (argument.Length == 4)
                                {
                                    typevalue = typeof(Binding.PanelAction<,,,>).MakeGenericType(argument);
                                }
                            }
                            else
                            {
                                typevalue = typeof(Binding.PanelAction);
                            }
                        }


                        lastNode = InsertPropertyToClassNode(typevalue, eventItem.bindingSource, lastNode, classNode);
                    }
                    else
                    {
                        Debug.LogError(eventItem.bindingSource + ":type NULL");
                    }
                }
            }
            var scriptValue = oldViewModel.Compile();
            var scriptPath = AssetDatabase.GetAssetPath(monoScript);
            System.IO.File.WriteAllText(scriptPath, scriptValue);
        }
        /// <summary>
        /// 向viewModel添加属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <param name="beforeNode"></param>
        /// <param name="classNode"></param>
        /// <returns></returns>
        private static AstNode InsertPropertyToClassNode(Type type, string source, AstNode beforeNode, TypeDeclaration classNode)
        {
            var child = classNode.Descendants.OfType<PropertyDeclaration>().Where(x => x.Name == source).FirstOrDefault();

            if (child != null)
            {
                classNode.Members.Remove(child);
            }

            var typeName = TypeStringName(type);

            child = new PropertyDeclaration();
            child.Modifiers = Modifiers.Public;
            child.ReturnType = new PrimitiveType(string.Format("{0}.{1}", type.Namespace, typeName));
            child.Name = source;
            #region Getter
            var accessor = child.Getter = new Accessor();
            var body = accessor.Body = new BlockStatement();
            var expression = new IdentifierExpression("GetValue");
            var typeArguement = new MemberType(new SimpleType(type.Namespace), typeName);
            expression.TypeArguments.Add(typeArguement);
            Statement statement = new ReturnStatement(new InvocationExpression(expression, new PrimitiveExpression(source)));
            body.Add(statement);
            #endregion
            #region Setter
            accessor = child.Setter = new Accessor();
            body = accessor.Body = new BlockStatement();
            expression = new IdentifierExpression("SetValue");
            typeArguement = new MemberType(new SimpleType(type.Namespace), typeName);
            expression.TypeArguments.Add(typeArguement);
            statement = new ExpressionStatement(new InvocationExpression(expression, new PrimitiveExpression(source), new IdentifierExpression("value")));
            body.Add(statement);
            #endregion
            classNode.InsertChildAfter(beforeNode, child, Roles.TypeMemberRole);
            return child;
        }

        /// <summary>
        /// 将类型转换为人可读的字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string TypeStringName(Type type)
        {
            var typeName = type.Name;
            if (type.IsGenericType)
            {
                typeName = type.Name.Remove(type.Name.IndexOf("`"));
                var arguments = type.GetGenericArguments();
                typeName += "<";
                typeName += string.Join(",", Array.ConvertAll<Type, string>(arguments, x => x.FullName));
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
            var components = new List<UnityEngine.Object>();
            components.Add(target);
            var allComponents = target.GetComponents<Component>();
            var innerComponents = allComponents.Where(x => supportControls.Contains(x.GetType()));
            components.AddRange(innerComponents.ToArray());

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

            var userComponents = GetUserMonobehaiver(target).Select(x => new TypeInfo(x.GetType()));
            list.InsertRange(0, userComponents);
            return list.ToArray();
        }


        internal static MonoBehaviour[] GetUserMonobehaiver(GameObject prefab)
        {
            var monobehaivers = prefab.GetComponents<MonoBehaviour>();
            return monobehaivers.Where(x => x != null && !InnerNameSpace.Contains(x.GetType().Namespace)).ToArray();
        }

        internal static void ChoiseAnUserMonobehiver(GameObject prefab, Action<MonoBehaviour> onChoise)
        {
            var behaivers = GetUserMonobehaiver(prefab);
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
        /// 初始化方法
        /// </summary>
        /// <param name="classNode"></param>
        public static MethodDeclaration GetInitComponentMethod(TypeDeclaration classNode)
        {
            var InitComponentsNode = classNode.Descendants.OfType<MethodDeclaration>().Where(x => x.Name == initcomponentMethod).FirstOrDefault();

            if (InitComponentsNode == null)
            {
                InitComponentsNode = new MethodDeclaration();
                InitComponentsNode.Name = initcomponentMethod;
                InitComponentsNode.Modifiers |= Modifiers.Protected | Modifiers.Override;
                InitComponentsNode.ReturnType = new ICSharpCode.NRefactory.CSharp.PrimitiveType("void");
                InitComponentsNode.Body = new BlockStatement();
                classNode.AddChild(InitComponentsNode, Roles.TypeMemberRole);
            }
            return InitComponentsNode;
        }

        public static MethodDeclaration GetAwakeMethod(TypeDeclaration classNode, string baseTypeName)
        {
            var awakeNode = classNode.Descendants.OfType<MethodDeclaration>().Where(x => x.Name == "Awake").FirstOrDefault();

            if (awakeNode == null)
            {
                var type = typeof(PanelBase).Assembly.GetType(baseTypeName);

                awakeNode = new MethodDeclaration();
                awakeNode.Name = "Awake";
                awakeNode.ReturnType = new ICSharpCode.NRefactory.CSharp.PrimitiveType("void");
                awakeNode.Body = new BlockStatement();

                if (type != null)
                {
                    //&& type.GetMethod("Awake")
                    var method = type.GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (method != null)
                    {
                        awakeNode.Modifiers |= Modifiers.Override;

                        if (method.IsVirtual)
                        {
                            var invocation = new InvocationExpression();
                            invocation.Target = new MemberReferenceExpression(new BaseReferenceExpression(), "Awake", new AstType[0]);
                            awakeNode.Body.Add(invocation);
                        }

                        if (method.IsPublic)
                        {
                            awakeNode.Modifiers |= Modifiers.Public;
                        }

                        else if (method.IsPrivate)
                        {
                            awakeNode.Modifiers |= Modifiers.Private;
                        }
                        else
                        {
                            awakeNode.Modifiers |= Modifiers.Protected;
                        }
                    }
                }
                else
                {
                    awakeNode.Modifiers = Modifiers.Private;
                }

                classNode.AddChild(awakeNode, Roles.TypeMemberRole);
            }
            return awakeNode;
        }

        /// <summary>
        /// 代码绑定方法
        /// </summary>
        /// <param name="classNode"></param>
        public static MethodDeclaration GetPropBindingMethod(TypeDeclaration classNode)
        {
            var PropBindingsNode = classNode.Descendants.OfType<MethodDeclaration>().Where(x => x.Name == propbindingsMethod).FirstOrDefault();

            if (PropBindingsNode == null)
            {
                PropBindingsNode = new MethodDeclaration();
                PropBindingsNode.Name = propbindingsMethod;
                PropBindingsNode.Modifiers |= Modifiers.Protected | Modifiers.Override;
                PropBindingsNode.ReturnType = new ICSharpCode.NRefactory.CSharp.PrimitiveType("void");
                PropBindingsNode.Body = new BlockStatement();
                classNode.AddChild(PropBindingsNode, Roles.TypeMemberRole);
            }
            return PropBindingsNode;
        }

        #region private functions
        /// <summary>
        /// 所有支持的父级
        /// </summary>
        /// <returns></returns>
        private static string[] LoadAllBasePanels()
        {
            var types = typeof(PanelBase).Assembly.GetTypes();
            var support = new List<Type>();
            foreach (var item in types)
            {
                var attributes = item.GetCustomAttributes(false);
                if (Array.Find(attributes, x => x is PanelParentAttribute) != null)
                {
                    support.Add(item);
                }
            }
            support.Sort(new ComparePanelParentType());
            support.Add(typeof(MonoBehaviour));
            support.Add(typeof(UIBehaviour));
            return support.ConvertAll(x => x.FullName).ToArray();
        }

        private static void LoadViewScriptCoder(GameObject prefab, GenCodeRule rule, Action<UICoder> onLoadCoder)
        {
            //加载已经存在的脚本
            LoadOldScript(prefab, coder =>
            {
                //添加命名空间和引用
                CompleteNameSpace(coder.tree, rule);
                //添加类
                CompleteClass(coder.tree, coder.className, rule);

                onLoadCoder.Invoke(coder);
            });

        }

        private static void LoadOldScript(GameObject prefab, Action<UICoder> onGet)
        {
            ChoiseAnUserMonobehiver(prefab, (x) =>
            {
                if (x != null)
                {
                    UICoder coder = new UICoder(x.GetType().Name);
                    var path = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(x));
                    coder.Load(System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8));
                    onGet(coder);
                }
                else
                {
                    UICoder coder = new UICoder(prefab.name);
                    onGet(coder);
                }

            });
        }

        /// <summary>
        /// 完善类名
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="prefab"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        private static TypeDeclaration CompleteClass(ICSharpCode.NRefactory.CSharp.SyntaxTree tree, string className, GenCodeRule rule)
        {
            TypeDeclaration classNode = null;
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
            var bs = classNode.BaseTypes.Where(x => Array.Find(basePanels, y => y.Contains(x.ToString())) != null).FirstOrDefault();
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
        private static void BindingInfoMethods(TypeDeclaration classNode, ComponentItem[] components, GenCodeRule rule)
        {
            componentCoder.SetContext(classNode, rule);
            foreach (var component in components)
            {
                componentCoder.CompleteCode(component, rule.bindingAble);
            }
        }

        /// <summary>
        /// 分析组件的绑定信息
        /// </summary>
        /// <param name="component"></param>
        /// <param name="components"></param>
        private static void AnalysisBindings(MonoBehaviour component, List<ComponentItem> components, GenCodeRule rule)
        {
            var script = MonoScript.FromMonoBehaviour(component).text;
            var tree = new CSharpParser().Parse(script);
            var classNode = tree.Descendants.OfType<TypeDeclaration>().Where(x => x.Name == component.GetType().Name).FirstOrDefault();
            componentCoder.SetContext(classNode, rule);
            componentCoder.AnalysisBinding(components);
        }

        #endregion


    }
}