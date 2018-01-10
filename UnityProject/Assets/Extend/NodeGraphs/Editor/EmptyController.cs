using UnityEngine;
using System.Collections.Generic;

namespace NodeGraph
{
    public class EmptyController : NodeGraphController
    {
        public override string Group { get { return "Empty"; } }

        protected override void JudgeNodeExceptions(DataModel. NodeGraphObj m_targetGraph, List<NodeException> m_nodeExceptions)
        {
            Debug.Log("To Judge All Nodes, Is There Have Some Exceptions?");
        }
        protected override void BuildFromGraph(DataModel.NodeGraphObj m_targetGraph)
        {
            Debug.Log("On Build Button Clicked!");
        }

        internal override List<KeyValuePair<string, DataModel.Node>> OnDragAccept(UnityEngine.Object[] objectReferences)
        {
            Debug.Log("You Can Quick Generate Node From DragAndDrop!");
            return new List<KeyValuePair<string, DataModel.Node>>();
        }

        internal override void Validate(NodeGUI node)
        {
            Debug.Log("When One Node Is  Updated, Judge Validate!");
        }
    }

}