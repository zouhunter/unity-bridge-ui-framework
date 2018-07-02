using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Documentation;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Analysis;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Completion;

using System.Linq;
using System.IO;
using System.Text;

public class NRefactoryTest
{
    private string classStr = @"
    public class DemoClass
    {
        public int DemoInt;
        public DemoClass()
        {
            DemoInt = 2;
        }
        public void DemoFunction(int arg)
        {
            DemoInt = arg;
            Debug.Log(" + "\"Hellow World\"" + @");
        }
    }
";
    private AstNode GetClassNode()
    {
        CompilerSettings setting = new CompilerSettings();
        CSharpParser cpaser = new CSharpParser(setting);

        ICSharpCode.NRefactory.CSharp.SyntaxTree tree = cpaser.Parse(classStr);
        if(tree.Errors != null && tree.Errors.Count > 0 )
        {
            foreach (var erro in tree.Errors)
            {
                Debug.LogError(erro.Message);
                Debug.LogError(erro.Region);
            }
            return null;
        }
        else
        {
            AstNode classNode = tree.Children.First();
            return classNode;
        }
    }
    private void PrintAstNode(AstNode item)
    {
        Debug.Log("[" + item.ToString() + "]" + " NodeType:" + item.NodeType + " Role:" + item.Role + " Type:" + item.GetType());
    }

    /// <summary>
    /// 测试查找调用函数
    /// </summary>
    [Test]
    public void TryFindAllInvocation()
    {
        var classNode = GetClassNode();
        var cons = classNode.Descendants.OfType<InvocationExpression>();
        foreach (var item in cons)
        {
            PrintAstNode(item);
        }
    }
    class FindInvocationsVisitor : DepthFirstAstVisitor
    {
        public override void VisitInvocationExpression(InvocationExpression invocationExpression)
        {
            Debug.Log(invocationExpression.Target);
            // Call the base method to traverse into nested invocations
            base.VisitInvocationExpression(invocationExpression);
        }
    }
    /// <summary>
    /// 测试使用Visitor进行查找
    /// </summary>
    [Test]
    public void TryVisitorTest()
    {
        ICSharpCode.NRefactory.CSharp.SyntaxTree syntaxTree = new CSharpParser().Parse(classStr);
        syntaxTree.AcceptVisitor(new FindInvocationsVisitor());
    }
    /// <summary>
    /// 测试修改一个方法的名称
    /// </summary>
    [Test]
    public void TryChangeMethodName()
    {
        var classNode = GetClassNode();
        Debug.Log("Before:" + classNode.ToString());

        foreach (var item in classNode.Children)
        {
            PrintAstNode(item);
        }

        AstNodeCollection<EntityDeclaration> methods = classNode.GetChildrenByRole(Roles.TypeMemberRole);
        foreach (var method in methods)
        {
            if (!method.IsFrozen && method.Name == "DemoFunction")
            {
                Identifier identifier = method.NameToken;
                identifier.Name = "New_DemoFunction";
                Debug.Log("After:" + classNode.ToString());
            }
        }

    }

    /// <summary>
    /// 从无到有创建一个代码
    /// </summary>
    [Test]
    public void TryGenerateCode()
    {
        AstNode root = GetRoot();
        foreach (var item in root.Children)
        {
            Debug.Log(item);
        }

        if (root.Descendants.OfType<UsingDeclaration>().Where(x => x.Namespace == "System").Count() == 0)
        {
            root.AddChild<AstNode>(new UsingDeclaration("System"), Roles.Root);
        }

        if (root.Descendants.OfType<UsingDeclaration>().Where(x => x.Namespace == "UnityEngine").Count() == 0)
        {
            root.AddChild<AstNode>(new UsingDeclaration("UnityEngine"), Roles.Root);
        }

        TypeDeclaration classNode = null;

        if (root.Descendants.OfType<TypeDeclaration>().Where(x => x.Name == "DemoClass").Count() != 0)
        {
            classNode = root.Descendants.OfType<TypeDeclaration>().Where(x => x.Name == "DemoClass").First();
        }
        else
        {
            classNode = new TypeDeclaration();
            classNode.Name = "DemoClass";
            classNode.Modifiers = Modifiers.Public;

            var comment = new Comment("<summary>", CommentType.Documentation);
            root.AddChild(comment, Roles.Comment);
            comment = new Comment("代码说明信息", CommentType.Documentation);
            root.AddChild(comment, Roles.Comment);
            comment = new Comment("<summary>", CommentType.Documentation);
            root.AddChild(comment, Roles.Comment);

            root.AddChild(classNode, Roles.TypeMemberRole);
        }

        if (classNode.Descendants.OfType<FieldDeclaration>().Where(x => x.Variables.Where(y => y.Name == "DemoField").Count() > 0).Count() == 0)
        {
            var field = new FieldDeclaration();
            field.Modifiers = Modifiers.Public;
            field.ReturnType = new ICSharpCode.NRefactory.CSharp.PrimitiveType("int");
            field.Variables.Add(new VariableInitializer("DemoField", new IdentifierExpression("0")));
            classNode.AddChild(field, Roles.TypeMemberRole);
        }

        if (classNode.Descendants.OfType<ConstructorDeclaration>().Where(x => x.Name == "DemoClass").Count() == 0)
        {
            var constractNode = new ConstructorDeclaration();
            constractNode.Modifiers = Modifiers.Public;
            constractNode.Name = "DemoClass";
            var statement = new BlockStatement();
            statement.Add(new AssignmentExpression(new IdentifierExpression("DemoField"), new PrimitiveExpression(1)));
            constractNode.Body = statement;
            classNode.AddChild(constractNode, Roles.TypeMemberRole);
        }

        if (classNode.Descendants.OfType<MethodDeclaration>().Where(x => x.Name == "DemoFunction").Count() == 0)
        {
            var mainFuncNode = new MethodDeclaration();
            mainFuncNode.Modifiers = Modifiers.Public;
            mainFuncNode.Name = "DemoFunction";
            mainFuncNode.ReturnType = new ICSharpCode.NRefactory.CSharp.PrimitiveType("void");
            var parameter1 = new ParameterDeclaration();
            parameter1.Type = new ICSharpCode.NRefactory.CSharp.PrimitiveType("int");
            parameter1.Name = "arg1";
            mainFuncNode.Parameters.Add(parameter1);
            var statement = new BlockStatement();
            statement.Add(new InvocationExpression(new MemberReferenceExpression(new IdentifierExpression("Debug"), "Log",new AstType[0]), new PrimitiveExpression("Hellow World")));
            mainFuncNode.Body = statement;

            var parameter2 = new ParameterDeclaration();
            //parameter2.Type = new ICSharpCode.NRefactory.CSharp.PrimitiveType("int[]");
            var type = new ComposedType();
            type.ArraySpecifiers.Add(new ArraySpecifier());
            type.BaseType = new ICSharpCode.NRefactory.CSharp.PrimitiveType("int");
            type.HasNullableSpecifier = true;
            parameter2.Type = type;
            parameter2.Name = "arg2";
            mainFuncNode.Parameters.Add(parameter2);
            classNode.AddChild(mainFuncNode, Roles.TypeMemberRole);
        }

        Debug.Log(root.ToString());
        SaveToAssetFolder(root.ToString());
    }

    private void SaveToAssetFolder(string text)
    {
        System.IO.File.WriteAllText(Application.dataPath + "/generated.cs", text);
        AssetDatabase.Refresh();
    }

    private ICSharpCode.NRefactory.CSharp.SyntaxTree GetRoot()
    {
        var path = Application.dataPath + "/generated.cs";
        if (File.Exists(path))
        {
            return new ICSharpCode.NRefactory.CSharp.CSharpParser().Parse(File.ReadAllText(path, Encoding.UTF8));
        }
        return new ICSharpCode.NRefactory.CSharp.SyntaxTree();
    }

}
//Roles

//Role

//Role<T>

//UsingDeclaration:AstNode

//MemberType: AstType : AstNode

//AstType : AstNode

//C# Syntax Tree 	Unresolved Type System	Resolved Type System
//AstType ITypeReference IType
//TypeDeclaration IUnresolvedTypeDefinition   ITypeDefinition
//EntityDeclaration   IUnresolvedEntity IEntity
//FieldDeclaration IUnresolvedField    IField
//PropertyDeclaration / IndexerDeclaration IUnresolvedProperty IProperty
//MethodDeclaration / ConstructorDeclaration / OperatorDeclaration /
//Accessor IUnresolvedMethod   IMethod
//EventDeclaration    IUnresolvedEvent IEvent
//Attribute IUnresolvedAttribute    IAttribute
//Expression  IConstantValue ResolveResult
//PrivateImplementationType IMemberReference    IMember
//ParameterDeclaration    IUnresolvedParameter IParameter
//Accessor IUnresolvedMethod   IMethod
//NamespaceDeclaration    UsingScope ResolvedUsingScope
//-	-	INamespace
//SyntaxTree  IUnresolvedFile 	-

//- 	IUnresolvedAssembly IAssembly
//- 	IProjectContent ICompilation

//var document = new ICSharpCode.NRefactory.Editor.StringBuilderDocument("");
//var formattingOptions = FormattingOptionsFactory.CreateAllman();
//var options = new TextEditorOptions();
//using (var script = new DocumentScript(document, formattingOptions, options))
//{
//    script.InsertText(0, root.ToString());
//}
//Debug.Log(document.Text);
//SaveToAssetFolder(document.Text);