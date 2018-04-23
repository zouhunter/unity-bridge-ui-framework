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
using UnityEditor;
using BridgeUI;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using System.IO;
using Microsoft.CSharp;

namespace BridgeUI
{
    public class UICoder
    {
        private string head
        {
            get
            {
                return CalculateHead(Application.companyName, System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "部分代码自动生成", "尽量使用MVVM模式");
            }
        }
        public SyntaxTree tree { get; private set; }
        public string className { get; private set; }
        private bool firstCreate { get; set; }
        public UICoder(string className)
        {
            this.className = className;
            firstCreate = true;
            tree = new SyntaxTree();
        }
        /// <summary>
        /// 从旧的脚本加载
        /// </summary>
        public void Load(string script)
        {
            CompilerSettings setting = new CompilerSettings();
            setting.LanguageVersion = new System.Version(2, 0, 0, 0);
            CSharpParser cpaser = new CSharpParser(setting);
            tree = cpaser.Parse(script);
            firstCreate = false;
        }
        /// <summary>
        /// 编译代码
        /// </summary>
        /// <returns></returns>
        public string Compile()
        {
            if (tree == null) return null;
            if (firstCreate)
            {
                return head + "\r\n" + tree.ToString();
            }
            else
            {
                return tree.ToString();
            }
        }

        private string CalculateHead(string author, string time, params string[] detailInfo)
        {
            var str1 =
               "/*************************************************************************************   \r\n" +
               "    * 作    者：       {0}\r\n" +
               "    * 创建时间：       {1}\r\n" +
               "    * 说    明：       ";
            var str2 = "\r\n                       ";
            var str3 = "\r\n* ************************************************************************************/\r\n";

            var headerStr = string.Format(str1, author, time);
            for (int i = 0; i < detailInfo.Length; i++)
            {
                if (i == 0)
                {
                    headerStr += string.Format("{0}.{1}", i + 1, detailInfo[i]);
                }
                else
                {
                    headerStr += string.Format("{0}{1}.{2}", str2, i + 1, detailInfo[i]);
                }
            }
            headerStr += str3;
            return headerStr;
        }

    }
}