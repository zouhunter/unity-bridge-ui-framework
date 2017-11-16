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
    [Node(false, "Object/Child Root Node")]
    public class ChildRootNode : ObjectNode
    {
        public const string ID = "childnode";
        public override string GetID { get { return ID; } }
        public override string Title { get { return "Child Root Node"; } }

        [ValueConnectionKnob("Input Left", Direction.In, "Float", NodeSide.Left, 20)]
        public ValueConnectionKnob inputLeft;
    }
}