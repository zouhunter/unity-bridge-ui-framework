using UnityEngine;
using System.Collections.Generic;

using Model=NodeGraph.DataModel;

namespace NodeGraph {
	/*
	 * ScriptableObject helper object to let ConnectionGUI edit from Inspector
	 */
	public class ConnectionGUIInspectorHelper : ScriptableObject {
		public ConnectionGUI connectionGUI;

        public void UpdateInspector (ConnectionGUI con) {
			this.connectionGUI = con;
		}
    }
}
