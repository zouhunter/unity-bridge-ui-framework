using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Model=NodeGraph.DataModel.Version2;

namespace NodeGraph {
	[CustomEditor(typeof(ConnectionGUIInspectorHelper))]
	public class ConnectionGUIEditor : Editor {
		
		public override bool RequiresConstantRepaint() {
			return true;
		}

		public override void OnInspectorGUI () {

			ConnectionGUIInspectorHelper helper = target as ConnectionGUIInspectorHelper;

            var con = helper.connectionGUI;
			if (con == null) {
				return;
			}
		}
	}
}