using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace BridgeUI.CodeGen
{
    public class ViewCoder
    {
        public string refClassName { get; set; }
        public string parentClassName { get; set; }
        public string className { get; set; }
        public string nameSpace { get; set; }
        public FieldInfo[] innerFields { get; set; }
        public ComponentItem[] componentItems { get; set; }
        public string path { get; set; }

        protected string compiledScript;
        public string ScriptValue
        {
            get
            {
                return compiledScript;
            }
        }

        private IViewCoderExecuter executer;
        private BindingViewCoderExecuter bindingcoder;
        private NormalViewCoderExecuter normalViewCoder;

        public ViewCoder()
        {
            bindingcoder = new BindingViewCoderExecuter(this);
            normalViewCoder = new NormalViewCoderExecuter(this);
        }
        public void AnalysisBinding(MonoBehaviour component, ComponentItem[] componentItems, GenCodeRule rule)
        {
            var fullViewName = Setting.defultNameSpace + "." + component.gameObject.name + "_Internal";
            var bindingType = typeof(BridgeUI.Binding.BindingViewBase).Assembly.GetType(fullViewName);
            rule.baseTypeIndex = Array.IndexOf(GenCodeUtil.supportBaseTypes, bindingType.BaseType.ToString());
            SwitchExecuter(bindingType);
            executer.AnalysisBinding(component.gameObject, componentItems);
        }
        /// <summary>
        /// 编译代码
        /// </summary>
        /// <returns></returns>
        public void CompileSave()
        {
            if (string.IsNullOrEmpty(parentClassName)) return;

            var parentType = typeof(ViewBase).Assembly.GetType(parentClassName);
            if (parentType == null) return;

            SwitchExecuter(parentType);

            compiledScript = executer.GenerateScript();
            SaveToFile(compiledScript);
        }

        private void SwitchExecuter(Type parentType)
        {
            if (typeof(BridgeUI.Binding.BindingViewBase).IsAssignableFrom(parentType))
            {
                executer = bindingcoder;
            }
            else if (typeof(BridgeUI.ViewBase).IsAssignableFrom(parentType))
            {
                executer = normalViewCoder;
            }
            else
            {
                Debug.LogError("父类型暂不支持：" + parentType);
                return;
            }
        }

        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="scriptValue"></param>
        protected void SaveToFile(string scriptValue)
        {
            if (string.IsNullOrEmpty(path)) return;
            if (string.IsNullOrEmpty(scriptValue)) return;

            var dir = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            System.IO.File.WriteAllText(path, scriptValue, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 创建代码头
        /// </summary>
        /// <param name="author"></param>
        /// <param name="time"></param>
        /// <param name="detailInfo"></param>
        /// <returns></returns>
        public string CreateHead(string author, string time, params string[] detailInfo)
        {
            var str1 =
               @"///*************************************************************************************
///* 作    者：       {0}
///* 创建时间：       {1}
///* 说    明：       ";
            var str2 = "///                   ";
            var str3 = "///* ************************************************************************************/";

            System.Text.StringBuilder headerStr = new System.Text.StringBuilder();
            headerStr.Append(string.Format(str1, author, time));
            for (int i = 0; i < detailInfo.Length; i++)
            {
                if (i == 0)
                {
                    headerStr.AppendLine(string.Format("{0}.{1}", i + 1, detailInfo[i]));
                }
                else
                {
                    headerStr.AppendLine(string.Format("{0}{1}.{2}", str2, i + 1, detailInfo[i]));
                }
            }
            headerStr.AppendLine(str3);
            return headerStr.ToString();
        }

    }
}