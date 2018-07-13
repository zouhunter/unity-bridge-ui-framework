using System;
using System.Linq;
using System.Collections.Generic;
using BridgeUI.NRefactory.CSharp;

namespace BridgeUI.CodeGen
{
    public class ComponentCoder
    {
        private MethodDeclaration _initComponentNode;
        private MethodDeclaration _propBindingsNode;
        protected MethodDeclaration InitComponentsNode
        {
            get
            {
                if (_initComponentNode == null)
                {
                    _initComponentNode = GenCodeUtil.GetInitComponentMethod(classNode);
                }
                return _initComponentNode;
            }
        }
        protected MethodDeclaration PropBindingsNode
        {
            get
            {
                if (_propBindingsNode == null)
                {
                    _propBindingsNode = GenCodeUtil.GetPropBindingMethod(classNode);
                }
                return _propBindingsNode;
            }
        }

        private MethodDeclaration _awakeNode;
        protected MethodDeclaration AwakeNode
        {
            get
            {
                if (_awakeNode == null)
                {
                    var baseTypeName = GenCodeUtil.supportBaseTypes[rule.baseTypeIndex];
                    _awakeNode = GenCodeUtil.GetAwakeMethod(classNode, baseTypeName);
                }
                return _awakeNode;
            }
        }

        protected TypeDeclaration classNode;
        protected GenCodeRule rule;

        public void SetContext(TypeDeclaration classNode, GenCodeRule rule)
        {
            this.classNode = classNode;
            this.rule = rule;
            _initComponentNode = null;
            _propBindingsNode = null;
        }

        /// <summary>
        /// Binding关联
        /// </summary>
        /// <returns></returns>
        public virtual void CompleteCode(ComponentItem component, bool bindingAble)
        {
            foreach (var item in component.viewItems)
            {
                BindingMemberInvocations(component.name, item);
            }

            foreach (var item in component.eventItems)
            {
                switch (item.type)
                {
                    case BindingType.NoBinding:
                        LocalEventInvocations(component.name, item, bindingAble);
                        break;
                    case BindingType.Normal:
                    case BindingType.WithTarget:
                        BindingEventInvocations(component.name, component.componentType, item);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 远端member关联
        /// </summary>
        protected virtual void BindingMemberInvocations(string name, BindingShow bindingInfo)
        {
            var invocations = PropBindingsNode.Body.Descendants.OfType<InvocationExpression>();
            var arg0_name = "m_" + name + "." + bindingInfo.bindingTarget;
            var arg0 = arg0_name;
            if (!bindingInfo.isMethod){
                arg0 = string.Format("x=>{0}=x", arg0_name);
            }
            
            UnityEngine.Debug.Log (bindingInfo.bindingTarget) ;
            UnityEngine.Debug.Log(bindingInfo.bindingTargetType.type) ;

            var arg1 = string.Format("\"{0}\"", bindingInfo.bindingSource);
            var invocation = invocations.Where(
                x =>x.Target.ToString().Contains("Binder") &&
                     x.Arguments.Count > 0 &&
                     x.Arguments.First().ToString().Replace(" ","") == arg0 &&
                     x.Arguments.ToArray()[1].ToString() == arg1).FirstOrDefault();

            if (invocation == null)
            {
                string methodName = "";
                UnityEngine.Debug.Assert(bindingInfo.bindingTargetType.type != null, name + ":" + bindingInfo.bindingSource + " type Null!");
                if (!bindingInfo.bindingTargetType.type.IsGenericType)
                {
                    var typeName = bindingInfo.bindingTargetType.typeName;
                    methodName = string.Format("RegistMember<{0}>", typeName);
                }
                else
                {
                    var type = bindingInfo.bindingTargetType.type;
                    var baseName = type.Name.Remove(type.Name.IndexOf("`"));
                    var arguments = type.GetGenericArguments();
                    baseName += "<";
                    baseName += string.Join(",", Array.ConvertAll<Type, string>(arguments, x => x.FullName));
                    baseName += ">";
                    methodName = string.Format("RegistMember<{0}>", baseName);
                }

                if (!string.IsNullOrEmpty(methodName))
                {
                    invocation = new InvocationExpression();
                    invocation.Target = new MemberReferenceExpression(new IdentifierExpression("Binder"), methodName, new AstType[0]);
                    invocation.Arguments.Add(new IdentifierExpression(arg0));
                    invocation.Arguments.Add(new PrimitiveExpression(bindingInfo.bindingSource));
                    PropBindingsNode.Body.Add(invocation);
                }

            }
        }

        /// <summary>
        /// 远端关联事件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bindingInfo"></param>

        protected virtual void BindingEventInvocations(string name, Type componentType, BindingEvent bindingInfo)
        {
            var invocations = PropBindingsNode.Body.Descendants.OfType<InvocationExpression>();
            var arg0_name = "m_" + name + "." + bindingInfo.bindingTarget;//绑定目标
            var arg1_name = string.Format("\"{0}\"", bindingInfo.bindingSource);//绑定源

            var may_invocations = invocations.Where(
                x => x.Target.ToString().Contains("Binder") &&
                x.Arguments.Count > 0 &&
                x.Arguments.First().ToString() == arg0_name &&
                x.Arguments.ToArray().Length > 1 &&
                x.Arguments.ToArray()[1].ToString() == arg1_name);

            InvocationExpression invocation = null;
            if (may_invocations != null)
            {
                //两个参数
                if (bindingInfo.type == BindingType.Normal)
                {
                    invocation = may_invocations.Where(x => x.Arguments.Count() == 2).FirstOrDefault();
                }
                //三个参数
                else if (bindingInfo.type == BindingType.WithTarget)
                {
                    invocation = may_invocations.Where(x => x.Arguments.ToArray().Length > 2 && x.Arguments.ToArray()[2].ToString() == "m_" + name).FirstOrDefault();
                }
            }

            if (invocation == null)
            {
                var methodName = "RegistEvent";

                UnityEngine.Debug.Assert(bindingInfo.bindingTargetType.type != null, bindingInfo.bindingSource + ";" + bindingInfo.bindingTarget + "  type Null");
                ///从UnityEvent和UnityEvent<T>派生
                if (bindingInfo.bindingTargetType.type.BaseType.IsGenericType)
                {
                    var arguments = bindingInfo.bindingTargetType.type.BaseType.GetGenericArguments().Select(x => x.FullName).ToList();
                    if (bindingInfo.type == BindingType.WithTarget) arguments.Add(componentType.FullName);
                    methodName = "RegistEvent<" + string.Join(",", arguments.ToArray()) + ">";
                }

                if (!string.IsNullOrEmpty(methodName))
                {
                    invocation = new InvocationExpression();
                    invocation.Target = new MemberReferenceExpression(new IdentifierExpression("Binder"), methodName, new AstType[0]);
                    invocation.Arguments.Add(new IdentifierExpression(arg0_name));
                    invocation.Arguments.Add(new PrimitiveExpression(bindingInfo.bindingSource));
                    if (bindingInfo.type == BindingType.WithTarget)
                    {
                        invocation.Arguments.Add(new IdentifierExpression("m_" + name));
                    }
                    PropBindingsNode.Body.Add(invocation);
                }

            }
        }

        /// <summary>
        /// 本地事件关联
        /// </summary>
        protected virtual void LocalEventInvocations(string name, BindingEvent bindingInfo, bool bindingAble)
        {
            var node = bindingAble ? InitComponentsNode : AwakeNode;
            var invocations = node.Body.Descendants.OfType<InvocationExpression>();

            var targetName = "m_" + name;
            var invocation = invocations.Where(x =>
            x.Target.ToString().Contains(targetName) &&
            x.Arguments.FirstOrDefault().ToString() == bindingInfo.bindingSource).FirstOrDefault();

            var eventName = bindingInfo.bindingTarget;//如onClick
            if (invocation == null && !string.IsNullOrEmpty(eventName) && !string.IsNullOrEmpty(bindingInfo.bindingSource))
            {
                invocation = new InvocationExpression();
                invocation.Target = new MemberReferenceExpression(new MemberReferenceExpression(new IdentifierExpression("m_" + name), eventName, new AstType[0]), "AddListener", new AstType[0]);
                invocation.Arguments.Add(new IdentifierExpression(bindingInfo.bindingSource));
                node.Body.Add(invocation);
                CompleteMethod(bindingInfo);
            }
        }

        /// <summary>
        /// 完善本地绑定的方法
        /// </summary>
        /// <param name="item"></param>
        protected void CompleteMethod(BindingEvent item)
        {
            var funcNode = classNode.Descendants.OfType<MethodDeclaration>().Where(x => x.Name == item.bindingSource).FirstOrDefault();
            if (funcNode == null)
            {
                UnityEngine.Debug.Assert(item.bindingTargetType.type != null, item.bindingSource + ":" + item.bindingTarget);
                var parameter = item.bindingTargetType.type.GetMethod("AddListener").GetParameters()[0];
                List<ParameterDeclaration> arguments = new List<ParameterDeclaration>();
                var parameters = parameter.ParameterType.GetGenericArguments();
                int count = 0;

                var oldType = typeof(PanelBase).Assembly.GetType(classNode.Name);
                System.Reflection.MethodInfo oldMethod = null;
                if (oldType != null)
                {
                    oldMethod = oldType.GetMethod(item.bindingSource, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                }

                System.Reflection.ParameterInfo[] oldParamters = null;

                if (oldMethod != null)
                {
                    if (parameters.Count() == 0)
                    {
                        return;
                    }
                    else
                    {
                        oldParamters = oldMethod.GetParameters();
                    }
                }

                bool sameFunc = parameters.Length > 0;

                for (int i = 0; i < parameters.Length; i++)
                {
                    var para = parameters[i];
                    if (oldParamters != null)
                    {
                        var oldParam = oldParamters[i];
                        if (oldParam.ParameterType.FullName != para.FullName)
                        {
                            sameFunc = false;
                        }
                    }
                    else
                    {
                        sameFunc = false;
                    }

                    ParameterDeclaration argument = new ParameterDeclaration(new BridgeUI.NRefactory.CSharp.PrimitiveType(para.FullName), "arg" + count++);
                    arguments.Add(argument);
                }

                if (!sameFunc)
                {
                    funcNode = new MethodDeclaration();
                    funcNode.Name = item.bindingSource;
                    funcNode.Modifiers |= Modifiers.Protected;
                    funcNode.ReturnType = new BridgeUI.NRefactory.CSharp.PrimitiveType("void");
                    funcNode.Parameters.AddRange(arguments);
                    funcNode.Body = new BlockStatement();
                    classNode.AddChild(funcNode, Roles.TypeMemberRole);
                }
            }
        }

        /// <summary>
        /// 分析代码中的绑定关系
        /// </summary>
        /// <param name="components"></param>
        internal void AnalysisBinding(List<ComponentItem> components)
        {
            if (classNode != null)
            {
                if (InitComponentsNode != null)
                {
                    var invctions = InitComponentsNode.Body.Descendants.OfType<InvocationExpression>();
                    foreach (var item in invctions)
                    {
                        AnalysisLoaclInvocation(item, components);
                    }
                }
                if (PropBindingsNode != null)
                {
                    var invctions = PropBindingsNode.Body.Descendants.OfType<InvocationExpression>();
                    foreach (var item in invctions)
                    {
                        if (item.Target.ToString().Contains("RegistMember"))
                        {
                            AnalysisBindingMembers(item, components);
                        }
                        else if (item.Target.ToString().Contains("RegistEvent"))
                        {
                            AnalysisBindingEvents(item, components);
                        }

                    }
                }
            }
        }
        /// <summary>
        /// 分析本地方法
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="components"></param>
        protected virtual void AnalysisLoaclInvocation(InvocationExpression invocation, List<ComponentItem> components)
        {
            var component = components.Find(x => invocation.Target.ToString().Contains("m_" + x.name));
            if (component != null)
            {
                string bindingSource = invocation.Arguments.First().ToString();
                var info = component.eventItems.Find(x => invocation.Target.ToString().Contains("m_" + component.name + "." + x.bindingTarget) && x.type == BindingType.NoBinding && x.bindingSource == bindingSource);

                if (info == null)
                {
                    var express = invocation.Target as MemberReferenceExpression;
                    var target = (express.Target as MemberReferenceExpression).MemberNameToken.Name;
                    Type infoType = GetTypeClamp(component.componentType, target);
                    info = new BindingEvent();
                    info.type = BindingType.NoBinding;
                    info.bindingSource = bindingSource;
                    info.bindingTarget = target;
                    info.bindingTargetType.Update(infoType);
                    component.eventItems.Add(info);
                }
            }
        }
        /// <summary>
        /// 分析绑定member
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="components"></param>
        protected virtual void AnalysisBindingMembers(InvocationExpression invocation, List<ComponentItem> components)
        {
            var component = components.Find(x => invocation.Arguments.Count > 1 && invocation.Arguments.First().ToString().Contains(string.Format("m_{0}.", x.name)));
            if (component != null)
            {
                var source = invocation.Arguments.ToArray()[1].ToString().Replace("\"", "");
                var info = component.viewItems.Find(x => x.bindingSource == source);
                var isMethod = false;
                var targetName = AnalysisTargetFromLamdaArgument(invocation.Arguments.First().ToString(),out isMethod);
                UnityEngine.Debug.Log(targetName);

                if (targetName == null)
                {
                    UnityEngine.Debug.Assert(!string.IsNullOrEmpty(targetName), "annalysis err:" + invocation.Arguments.First().ToString());
                    return;
                }
                var type = GetTypeClamp(component.componentType, targetName);
                if (info == null)
                {
                    info = new BindingShow();
                    info.bindingSource = source;
                    info.bindingTarget = targetName;
                    info.isMethod = isMethod;
                    component.viewItems.Add(info);
                }
                info.bindingTargetType.Update(type);
            }
        }

        private string AnalysisTargetFromLamdaArgument(string arg,out bool isMethod)
        {
            arg = arg.Replace(" ", "");
            if(arg.Contains("=>"))
            {
                isMethod = false;
                var pattem = "x =>(.*)=x";
                var match = System.Text.RegularExpressions.Regex.Match(arg, pattem);
                if (match != null)
                {
                    var value = match.Groups[1].Value;
                    if (value.Contains("."))
                    {
                        value = value.Substring(value.IndexOf('.') + 1);
                    }
                    return value;
                }
                return null;
            }
            else
            {
                isMethod = true;
                var value = arg;
                if (value.Contains("."))
                {
                    value = value.Substring(value.IndexOf('.') + 1);
                }
                return value;
            }
           
        }

        /// <summary>
        /// 分析绑定方法
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="components"></param>
        protected virtual void AnalysisBindingEvents(InvocationExpression invocation, List<ComponentItem> components)
        {
            var component = components.Find(x => invocation.Arguments.Count > 1 && invocation.Arguments.First().ToString().Contains("m_" + x.name));

            if (component != null)
            {
                var source = invocation.Arguments.ToArray()[1].ToString().Replace("\"", "");
                var info = component.eventItems.Find(x => x.bindingSource == source);
                var arg0 = invocation.Arguments.First().ToString();
                var targetName = arg0.Substring(arg0.IndexOf(".") + 1);
                Type infoType = GetTypeClamp(component.componentType, targetName);

                if (info == null)
                {
                    info = new BindingEvent();
                    info.bindingSource = source;
                    info.bindingTarget = targetName;
                    info.bindingTargetType.Update(infoType);
                    component.eventItems.Add(info);
                }

                info.bindingTargetType.Update(infoType);

                if (invocation.Arguments.Count() > 2)
                {
                    info.type = BindingType.WithTarget;//3个参数
                }
                else
                {
                    info.type = BindingType.Normal;//2个参数
                }
            }
        }
        protected Type GetTypeClamp(Type baseType, string membername)
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

            var func = baseType.GetMethod(membername, System.Reflection.BindingFlags.GetField | flag);
            if (func != null && func.GetParameters().Count() == 1)
            {
                infoType = func.GetParameters()[0].ParameterType;
            }
            return infoType;
        }
    }
}