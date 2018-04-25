using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using System.CodeDom;
using BridgeUI.Model;
using ICSharpCode.NRefactory.CSharp;

namespace BridgeUI.CodeGen
{
    public abstract class ComponentCode
    {
        /// <summary>
        /// 创建一组方法体
        /// </summary>
        /// <returns></returns>
        public virtual void BindingInvocation(TypeDeclaration type,ComponentItem component) {

            //if (!component.binding)
            //{
            //    var invocations = InitComponentsNode.Body.Descendants.OfType<InvocationExpression>();
            //    var invocation = invocations.Where(x => x.Target.ToString().Contains("m_" + component.name) && x.Arguments.Where(ag => ag.ToString() == component.sourceName) != null).FirstOrDefault();
            //    var methodName = GetMethodName_InitComponentsNode(component.componentType);
            //    if (invocation == null && !string.IsNullOrEmpty(methodName) && !string.IsNullOrEmpty(component.sourceName))
            //    {
            //        invocation = new InvocationExpression();
            //        invocation.Target = new MemberReferenceExpression(new MemberReferenceExpression(new IdentifierExpression("m_" + component.name), methodName, new AstType[0]), "AddListener", new AstType[0]);
            //        invocation.Arguments.Add(new IdentifierExpression(component.sourceName));
            //        InitComponentsNode.Body.Add(invocation);
            //    }
            //}
            //else
            //{
            //    var invocations = PropBindingsNode.Body.Descendants.OfType<InvocationExpression>();
            //    var invocation = invocations.Where(x => x.Target.ToString().Contains("Binder") && x.Arguments.Count > 0 && x.Arguments.First().ToString().Contains("m_" + component.name)).FirstOrDefault();
            //    if (invocation == null)
            //    {
            //        var methodName = GetMethodNameFromComponent(component.componentType);
            //        if (!string.IsNullOrEmpty(methodName))
            //        {
            //            invocation = new InvocationExpression();
            //            invocation.Target = new MemberReferenceExpression(new IdentifierExpression("Binder"), methodName, new AstType[0]);
            //            invocation.Arguments.Add(new IdentifierExpression("m_" + component.name));
            //            invocation.Arguments.Add(new PrimitiveExpression(component.sourceName));
            //            PropBindingsNode.Body.Add(invocation);
            //        }

            //    }
            //}
        }
        /// <summary>
        /// 完善方法
        /// </summary>
        /// <param name="classNode"></param>
        /// <param name="item"></param>
        //private static void CompleteMethod(TypeDeclaration classNode, ComponentItem item)
        //{
        //    var funcNode = classNode.Descendants.OfType<MethodDeclaration>().Where(x => x.Name == item.sourceName).FirstOrDefault();
        //    if (funcNode == null)
        //    {
        //        var argument = GetArgument_InitComponentsNode(item.componentType);
        //        if (argument != null && !InnerFunctions.Contains(item.sourceName))
        //        {
        //            funcNode = new MethodDeclaration();
        //            funcNode.Name = item.sourceName;
        //            funcNode.Modifiers |= Modifiers.Protected;
        //            funcNode.ReturnType = new ICSharpCode.NRefactory.CSharp.PrimitiveType("void");
        //            funcNode.Parameters.Add(argument);
        //            funcNode.Body = new BlockStatement();
        //            classNode.AddChild(funcNode, Roles.TypeMemberRole);
        //        }

        //    }
        //}
    }
}