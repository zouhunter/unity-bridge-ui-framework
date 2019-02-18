using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace BridgeUI.CodeGen
{
    public interface IViewCoderExecuter
    {
        void AnalysisBinding(GameObject gameObject, ComponentItem[] componentItems);
        string GenerateScript();
    }

    public abstract class ViewCoderExecuter: IViewCoderExecuter
    {
        protected string refClassName { get { return viewCoder.refClassName; } set { viewCoder.refClassName = value; } }
        protected string parentClassName { get { return viewCoder.parentClassName; } set { viewCoder.parentClassName = value; } }
        protected string className { get { return viewCoder.className; } set { viewCoder.className = value; } }
        protected string nameSpace { get { return viewCoder.nameSpace; } set { viewCoder.nameSpace = value; } }
        protected FieldInfo[] innerFields { get { return viewCoder.innerFields; } set { viewCoder.innerFields = value; } }
        protected ComponentItem[] componentItems { get { return viewCoder.componentItems; } set { viewCoder.componentItems = value; } }
        protected string path { get { return viewCoder.path; } set { viewCoder.path = value; } }


        protected ViewCoder viewCoder;

        public ViewCoderExecuter(ViewCoder viewCoder)
        {
            this.viewCoder = viewCoder;
        }
        public abstract void AnalysisBinding(GameObject gameObject, ComponentItem[] componentItems);
        public abstract string GenerateScript();
    }
}