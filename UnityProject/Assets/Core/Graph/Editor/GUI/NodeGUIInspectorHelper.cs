using UnityEngine;
using System.Collections.Generic;

using Model=NodeGraph.DataModel.Version2;

namespace NodeGraph {
	/*
	 * ScriptableObject helper object to let NodeGUI edit from Inspector
	 */
    public class NodeGUIInspectorHelper : ScriptableObject {
		public NodeGUI node;
		public NodeGraphController controller;
		public List<string> errors = new List<string>();

		public void UpdateNode (NodeGraphController c, NodeGUI node) {
			this.controller = c;
			this.node = node;
		}

		public void UpdateErrors (List<string> errorsSource) {
			this.errors = errorsSource;
		}
	}
}