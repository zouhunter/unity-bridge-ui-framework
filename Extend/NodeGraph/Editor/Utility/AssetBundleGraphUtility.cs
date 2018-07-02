using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Model=NodeGraph.DataModel;

namespace NodeGraph {

    /// <summary>
    /// Execute graph result.
    /// </summary>
	public class ExecuteGraphResult {
		private Model.NodeGraphObj  	graph;
		private List<NodeException>	issues;

		public ExecuteGraphResult(Model.NodeGraphObj g, List<NodeException> issues) {
			this.graph  = g;
			this.issues = issues;
		}

        /// <summary>
        /// Gets a value indicating whether last graph execution has any issue found.
        /// </summary>
        /// <value><c>true</c> if this instance is any issue found; otherwise, <c>false</c>.</value>
		public bool IsAnyIssueFound {
			get {
				return issues.Count > 0;
			}
		}

        /// <summary>
        /// Gets the executed graph associated with this result.
        /// </summary>
        /// <value>The graph.</value>
		public Model.NodeGraphObj Graph {
			get {
				return graph;
			}
		}

        /// <summary>
        /// Gets the graph asset path.
        /// </summary>
        /// <value>The graph asset path.</value>
		public string GraphAssetPath {
			get {
				return AssetDatabase.GetAssetPath(graph);
			}
		}

        /// <summary>
        /// Gets the list of issues found during last execution.
        /// </summary>
		public IEnumerable<NodeException> Issues {
			get {
				return issues.AsEnumerable();
			}
		}
	}

}
