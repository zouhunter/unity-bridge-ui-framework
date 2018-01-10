using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NodeGraph;
using NodeGraph.DataModel;

public class TestNodeGraph {

    [Test]
    public void EditorTest()
    {
        var graph = NodeGraphObj.CreateInstance<NodeGraphObj>();
        var node = Node.CreateInstance<PanelNode>();
        node.nodeInfo.prefabGuid = "12345";
        graph.Nodes.Add(new NodeData("apanel",node,1,2));
        var json = JSONGraphUtility.SerializeGraph(graph);
        Debug.Log(json);
        NodeGraphObj newGraph = null;
        JSONGraphUtility.DeserializeGraph(json,ref newGraph);
        Debug.Log((newGraph.Nodes[0].Object as PanelNode).nodeInfo.prefabGuid);
    }
}
