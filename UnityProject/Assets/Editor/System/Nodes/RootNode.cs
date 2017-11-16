using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using NodeEditorFramework;
using UnityEditor;
using System;
namespace PrefabGenerate
{
    [Node(false, "Object/Root Node")]
    public class RootNode : ObjectNode
    {
        public const string ID = "rootnode";
        public override string GetID { get { return ID; } }
        public override string Title { get { return "Root Node"; } }
    }
}