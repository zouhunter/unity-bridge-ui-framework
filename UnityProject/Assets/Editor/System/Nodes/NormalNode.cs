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
    [Node(false, "Object/Normal Node")]
    public class NormalNode : ObjectNode
    {
        public const string ID = "normalnode";
        public override string GetID { get { return ID; } }
        public override string Title { get { return "Normal Node"; } }

        [ValueConnectionKnob("Input Left", Direction.In, "Float", NodeSide.Left, 20)]
        public ValueConnectionKnob inputLeft;
    }
}