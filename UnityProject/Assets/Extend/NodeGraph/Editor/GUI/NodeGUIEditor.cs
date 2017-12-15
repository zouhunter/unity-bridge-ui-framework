using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

using Model = NodeGraph.DataModel;

namespace NodeGraph
{
    [CustomEditor(typeof(NodeGUIInspectorHelper))]
    public class NodeGUIEditor : Editor
    {

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            var currentTarget = (NodeGUIInspectorHelper)target;
            var node = currentTarget.Node;

            if (node == null)
            {
                return;
            }

            UnityEngine.Assertions.Assert.IsNotNull(node);

            node.DrawNodeGUI(this);

            var errors = currentTarget.Errors;
            if (errors != null && errors.Any())
            {
                foreach (var error in errors)
                {
                    EditorGUILayout.HelpBox(error, MessageType.Error);
                }
            }
        }

        public void UpdateNodeName(NodeGUI node)
        {
            var newName = EditorGUILayout.TextField("Node Name", node.Name);

            if (NodeGUIUtility.allNodeNames != null)
            {
                var overlapping = NodeGUIUtility.allNodeNames.GroupBy(x => x)
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key);
                if (overlapping.Any() && overlapping.Contains(newName))
                {
                    EditorGUILayout.HelpBox("There are node with the same name. You may want to rename to avoid confusion:" + newName, MessageType.Info);
                }
            }

            if (newName != node.Name)
            {
                using (new RecordUndoScope("Change Node Name", node, true))
                {
                    node.Name = newName;
                }
            }
        }


        public string DrawFolderSelector(string label,
            string dialogTitle,
            string currentDirPath,
            string directoryOpenPath,
            Func<string, string> onValidFolderSelected = null)
        {
            string newDirPath;
            using (new EditorGUILayout.HorizontalScope())
            {
                if (string.IsNullOrEmpty(label))
                {
                    newDirPath = EditorGUILayout.TextField(currentDirPath);
                }
                else
                {
                    newDirPath = EditorGUILayout.TextField(label, currentDirPath);
                }

                if (GUILayout.Button("Select", GUILayout.Width(50f)))
                {
                    var folderSelected =
                        EditorUtility.OpenFolderPanel(dialogTitle, directoryOpenPath, "");
                    if (!string.IsNullOrEmpty(folderSelected))
                    {
                        if (onValidFolderSelected != null)
                        {
                            newDirPath = onValidFolderSelected(folderSelected);
                        }
                        else
                        {
                            newDirPath = folderSelected;
                        }
                    }
                }
            }
            return newDirPath;
        }
    }
}