using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using System;
using UnityEditor;
using System.Reflection;
namespace PrefabGenerate
{
    public class PrefabCreater
    {
        public string exprotRoot = "Assets/Prefab-Generator/Demo/PrefabGen/";
        public GameObject CreatePrefab(ObjectNode rootNode)
        {
            var name = (rootNode.obj.name == "" ? "Empty" : rootNode.obj.name) + ".prefab";
            var obj = CreateObjFromNode(rootNode);
            var prefab = PGUtility.GenPrefab(exprotRoot + name, obj);
            ExecuteModifys(rootNode);
            return prefab;
        }

        private GameObject CreateObjFromNode(ObjectNode node)
        {
            if (node.obj.item != null)
            {
                node.obj.instence = GameObject.Instantiate(node.obj.item);
                node.obj.instence.name = node.obj.name;
            }
            else
            {
                node.obj.instence = new GameObject("EmptyNode");
            }
            //添加脚本
            foreach (var item in node.sHolds)
            {
                MonoScript monoscript = item.monoScript;
                if (monoscript != null)
                {
                    var script = node.obj.instence.AddComponent(monoscript.GetClass());
                    PGUtility.InistallVariableToBehaiver(item.variables, script);
                }
            }

            foreach (var item in node.outputRight.connections)
            {
                if (item.body != null)
                {
                    var objh = item.body as ObjectNode;
                    if (objh is ChildRootNode)
                    {
                        //新预制体并记录信息
                        CreatePrefab(objh);
                    }
                    else
                    {
                        var child = CreateObjFromNode(objh);
                        child.transform.SetParent(node.obj.instence.transform);
                    }
                }
            }
            return node.obj.instence;
        }

        private void ExecuteModifys(ObjectNode node)
        {
            foreach (var item in node.outputRight.connections)
            {
                if (item.body != null)
                {
                    var objh = item.body as ObjectNode;
                    ExecuteModifys(objh);
                }
            }
            var modify = node.modify;
            if (modify != null)
            {
                modify.ModifyPrefab(node);
            }
        }
    }
}
