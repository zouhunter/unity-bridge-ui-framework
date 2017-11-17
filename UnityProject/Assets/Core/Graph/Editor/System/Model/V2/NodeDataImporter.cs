using UnityEngine;
using System.Collections;

using NodeGraph;
using V1=AssetBundleGraph;

namespace NodeGraph.DataModel.Version2 {
	public interface NodeDataImporter {
		void Import(V1.NodeData v1, NodeData v2);
	}
}
