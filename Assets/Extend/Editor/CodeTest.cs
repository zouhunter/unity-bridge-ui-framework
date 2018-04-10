using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System;
using System.CodeDom;
using Microsoft.CSharp;
using BridgeUI.NRefactory;
using BridgeUI.NRefactory.Ast;
//using BridgeUI.NRefactory.AstBuilder;
using BridgeUI.NRefactory.Parser;
using BridgeUI.NRefactory.Parser.CSharp;
using BridgeUI.NRefactory.PrettyPrinter;
using BridgeUI.NRefactory.Visitors;
using System.IO;
using System.Text;
using System.CodeDom.Compiler;

public class CodeTest {

    [Test]
    public void CompileUnitTest()
    {
        CodeCompileUnit unit = new CodeCompileUnit();
        var path = @"E:\Github\BridgeUI\Assets\Extend\Editor\NewBehaviourScript.cs";
        var text = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
        using (var reader = new System.IO.StringReader(text))
        {
            IParser parser = ParserFactory.CreateParser(reader);
            parser.Parse();
            
            CodeDomVisitor visit = new CodeDomVisitor();
            visit.VisitCompilationUnit(parser.CompilationUnit, null);
            unit = visit.codeCompileUnit;

            CodeGeneratorOptions option = new CodeGeneratorOptions();
            option.BlankLinesBetweenMembers = true;
            CSharpCodeProvider provider = new CSharpCodeProvider();

            using (StringWriter sw = new System.IO.StringWriter())
            {
                provider.GenerateCodeFromCompileUnit(unit, sw, option);
                Debug.Log(sw.ToString())
;            }
        }
       
        
    }
}
