using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;

public class PanelNameGenerater
{
    private string outPutPath;
    private const string panelsName = "PanelNames";
    public PanelNameGenerater(string exportPath)
    {
        this.outPutPath = exportPath;
    }
    public void GenerateParcialPanelName(string[] panelNames)
    {
        var needGenerate = new List<string>();
        var type = typeof(BridgeUI.PanelGroup).Assembly.GetType(panelsName);

        if(type != null)
        {
            var oldNames = Array.ConvertAll<PropertyInfo, string>(type.GetProperties(), x => x.Name);
            needGenerate.AddRange(oldNames);
        }
     

        foreach (var item in panelNames)
        {
            if (!needGenerate.Contains(item))
            {
                //生成没有的
                needGenerate.Add(item);
            }
        }

        GenerateInternal(needGenerate.ToArray());
    }

    private void GenerateInternal(string[] panelNames)
    {
        CodeCompileUnit compunit = new CodeCompileUnit();
        CodeNamespace sample = new CodeNamespace();
        compunit.Namespaces.Add(sample);


        CodeTypeDeclaration wrapProxyClass = new CodeTypeDeclaration("PanelNames");
        wrapProxyClass.TypeAttributes = TypeAttributes.Public;
        //wrapProxyClass.IsPartial = true;
        sample.Types.Add(wrapProxyClass);//把这个类添加到命名空间 ,待会儿才会编译这个类


        foreach (var item in panelNames)
        {
            System.CodeDom.CodeMemberProperty prop = new CodeMemberProperty();
            prop.Name = item;
            prop.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            prop.Type = new CodeTypeReference(typeof(string));
            prop.HasGet = true;
            prop.HasSet = false;
            prop.GetStatements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(item)));
            wrapProxyClass.Members.Add(prop);
        }


        CSharpCodeProvider cprovider = new CSharpCodeProvider();

        StringBuilder fileContent = new StringBuilder();
        using (StringWriter sw = new StringWriter(fileContent)){
            cprovider.GenerateCodeFromCompileUnit(compunit, sw, new CodeGeneratorOptions());//想把生成的代码保存为cs文件
        }

        Debug.Log(fileContent.ToString());
        File.WriteAllText(outPutPath, fileContent.ToString());
    }
}
