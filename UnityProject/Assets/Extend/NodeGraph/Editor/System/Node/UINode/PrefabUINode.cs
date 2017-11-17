using UnityEngine;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

using V1=AssetBundleGraph;
using Model=NodeGraph.DataModel.Version2;
[System.Serializable]
public class PrefabUIInfo
{
    public string panelName;
}

namespace NodeGraph {

	[CustomNode("UINode/PrefabUINode", 1)]
	public class PrefabUINode : Node, Model.NodeDataImporter {

		public override string ActiveStyle {
			get {
				return "node 0 on";
			}
		}

		public override string InactiveStyle {
			get {
				return "node 0";
			}
		}

		public override string Category {
			get {
				return "prefab";
			}
		}

		public override Model.NodeOutputSemantics NodeInputType {
			get {
				return Model.NodeOutputSemantics.None;
			}
		}

        public PrefabUIInfo uiInfo;

        public PrefabUINode() {}
        public PrefabUINode(string path) {
        }

		public override void Initialize(Model.NodeData data) {
			data.AddDefaultOutputPoint();
		}

		public void Import(V1.NodeData v1, Model.NodeData v2) {
		}

		public override Node Clone(Model.NodeData newData) {
			var newNode = new PrefabUINode();

			newData.AddDefaultOutputPoint();
			return newNode;
		}

		//public override bool OnAssetsReimported(
  //          Model.NodeData nodeData,
		//	AssetReferenceStreamManager streamManager,
		//	BuildTarget target, 
		//	string[] importedAssets, 
		//	string[] deletedAssets, 
		//	string[] movedAssets, 
		//	string[] movedFromAssetPaths)
		//{
		//	if (streamManager == null) {
		//		return true;
		//	}

		//	var assetGroup = streamManager.FindAssetGroup(nodeData.OutputPoints[0]);

			
		//	return false;
		//}

		public override void OnInspectorGUI(NodeGUI node, AssetReferenceStreamManager streamManager, NodeGUIEditor editor, Action onValueChanged) {
            EditorGUILayout.TextField(uiInfo.panelName);
		}


		//public override void Prepare (BuildTarget target, 
		//	Model.NodeData node, 
		//	IEnumerable<PerformGraph.AssetGroups> incoming, 
		//	IEnumerable<Model.ConnectionData> connectionsToOutput, 
		//	PerformGraph.Output Output) 
		//{
			
		//}
		
		public static void ValidateLoadPath (string currentLoadPath, string combinedPath, Action NullOrEmpty, Action NotExist) {
			if (string.IsNullOrEmpty(currentLoadPath)) NullOrEmpty();
			if (!Directory.Exists(combinedPath)) NotExist();
		}

	}
}