using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Cryptography;

using Model=NodeGraph.DataModel.Version2;

namespace NodeGraph {
	public class NodeGraphController {

		private List<NodeException> m_nodeExceptions;
		private Model.ConfigGraph m_targetGraph;
		private bool m_isBuilding;

		public bool IsAnyIssueFound {
			get {
				return m_nodeExceptions.Count > 0;
			}
		}

		public List<NodeException> Issues {
			get {
				return m_nodeExceptions;
			}
		}

		public Model.ConfigGraph TargetGraph {
			get {
				return m_targetGraph;
			}
		}


		public NodeGraphController(Model.ConfigGraph graph) {
			m_targetGraph = graph;
			m_nodeExceptions = new List<NodeException>();
		}

        internal void BuildToSelect()
        {
           var leftNode = TargetGraph.CollectAllLeafNodes();
            foreach (var item in leftNode)
            {
                Debug.Log("CollectAllLeftNodes: " + item.Name);
            }
            var connectons = TargetGraph.Connections;
            foreach (var item in connectons)
            {
                Debug.Log("Connection: " + item.Label);
            }
        }
    }
}
