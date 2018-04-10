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

public class CodeTest
{

    string code = @"using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NewBehaviourScript : MonoBehaviour {
    public List<string> texts = new List<string>();
	// Use this for initialization
	void Start () {
        Test<string>(" + "\"哈哈\"" + @");
        Test(1);
}

[UnityEditor.MenuItem("+ "\"menu01\"" + @")]
void Initial()
{

}

void Test<T>(T a,out int a,ref int b)
{
    Debug.Log(a);
}
}
";
    [Test]
    public void CompileUnitTest()
    {
        CodeCompileUnit unit = new CodeCompileUnit();
        using (var reader = new System.IO.StringReader(code))
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
;
            }
        }


    }
}
