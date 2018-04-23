using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Model=NodeGraph.DataModel;

namespace NodeGraph {
	/*
	 * ScriptableObject helper object to let NodeGUI edit from Inspector
	 */
    public class NodeGUIInspectorHelper : ScriptableObject {
	
        public NodeGUI Node { get { return _node; } }
        public List<string> Errors { get { return _errors; } }

        private NodeGUI _node;

        private List<string> _errors = new List<string>();

        public void UpdateNodeGUI(NodeGUI node)
        {
            this._node = node;
        }

		public void UpdateErrors (List<string> errorsSource) {
			this._errors = errorsSource;
		}
	}
}