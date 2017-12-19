using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using NodeGraph.DataModel;
using UnityEngine.Events;
using System.Reflection;

namespace NodeGraph
{
    public class NodeDrawer: DefultDrawer
    {
        public Node target;
        public virtual string ActiveStyle { get { return "node 0 on"; } }
		public virtual string InactiveStyle { get { return "node 0"; } }
        public virtual string Category { get { return "empty"; } }
        public virtual void OnContextMenuGUI(GenericMenu menu) { }
        public virtual float CustomNodeHeight { get { return 0; } }
        public virtual void OnNodeGUI(Rect position) { }
        public virtual void OnInspectorGUI(NodeGUI gui)
        {
            base.OnInspectorGUI(target);
        }
        public virtual void OnClickNodeGUI(NodeGUI nodeGUI, Vector2 mousePosition, ConnectionPointData result) { }
        protected void RecordUnDo(string message,NodeGUI node,bool saveOnScopeEnd,UnityAction action)
        {
            using (new RecordUndoScope("Change Node Name", node, saveOnScopeEnd))
            {
                action.Invoke();
            }
        }
    }
}

