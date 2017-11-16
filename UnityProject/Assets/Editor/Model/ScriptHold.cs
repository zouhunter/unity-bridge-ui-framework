using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
namespace PrefabGenerate
{
    [System.Serializable]
    public class ScriptHold
    {
        public MonoScript monoScript;
        public List<Variable> variables = new List<Variable>();
        public ScriptHold()
        {

        }
        public ScriptHold(MonoScript script)
        {
            this.monoScript = script;
        }
        public void InitVariables()
        {
            if (monoScript == null) return;
            var type = monoScript.GetClass();
            FieldInfo[] infos = type.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
            foreach (var item in infos)
            {
                var node = variables.Find(x => x.name == item.Name);
                if (node == null){
                    node = new Variable();
                    variables.Add(node);
                }

                node.name = item.Name;

                //if (item.FieldType == typeof(float) || item.FieldType == typeof(double)
                //    || item.FieldType == typeof(int) || item.FieldType == typeof(long)
                //    || item.FieldType == typeof(string) || item.FieldType == typeof(String))
                //{
                //    node.type = NodeType.Single;
                //    if(node.singleValue==null) node.singleValue = "";
                //}

                //else if (item.FieldType == typeof(Vector3))
                //{
                //    node.type = NodeType.Vector3;
                //}
                //else if (item.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
                //{
                //    node.type = NodeType.Object;
                //}

            }
        }
    }
}
