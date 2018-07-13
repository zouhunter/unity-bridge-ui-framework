using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Cryptography;
using Model = NodeGraph.DataModel;

namespace NodeGraph
{
    public abstract class NodeGraphController<T>: NodeGraphController where T : Model.NodeGraphObj
    {
        public override Model.NodeGraphObj CreateNodeGraphObject(string path)
        {
            Model.NodeGraphObj graph = ScriptableObject.CreateInstance<T>();
            graph.ControllerType = this.GetType().FullName;
            AssetDatabase.CreateAsset(graph, path);
            return graph;
        }
    }

    [System.Serializable]
    public abstract class NodeGraphController
    {
        private List<NodeException> m_nodeExceptions = new List<NodeException>();
        private Model.NodeGraphObj m_targetGraph;
        public abstract string Group { get; }

        public bool IsAnyIssueFound
        {
            get
            {
                return m_nodeExceptions.Count > 0;
            }
        }

        public List<NodeException> Issues
        {
            get
            {
                return m_nodeExceptions;
            }
        }

        public Model.NodeGraphObj TargetGraph
        {
            get
            {
                return m_targetGraph;
            }
            set
            {
                m_targetGraph = value;
            }
        }

        public void Perform()
        {
            LogUtility.Logger.Log(LogType.Log, "---Setup BEGIN---");

            foreach (var e in m_nodeExceptions)
            {
                var errorNode = m_targetGraph.Nodes.Find(n => n.Id == e.Id);
                // errorNode may not be found if user delete it on graph
                if (errorNode != null)
                {
                    LogUtility.Logger.LogFormat(LogType.Log, "[Perform] {0} is marked to revisit due to last error", errorNode.Name);
                    errorNode.NeedsRevisit = true;
                }
            }

            m_nodeExceptions.Clear();
            JudgeNodeExceptions(m_targetGraph, m_nodeExceptions);
            LogUtility.Logger.Log(LogType.Log, "---Setup END---");
        }
        public void Build()
        {
            if(m_nodeExceptions == null || m_nodeExceptions.Count == 0)
            {
                BuildFromGraph(m_targetGraph);
            }
            else
            {
                Debug.LogError("have exception in build!");
            }
        }
        protected virtual void JudgeNodeExceptions(Model.NodeGraphObj m_targetGraph, List<NodeException> m_nodeExceptions) { }
        protected virtual void BuildFromGraph(Model.NodeGraphObj m_targetGraph) { }
        internal virtual void OnDragUpdated() { }
        internal virtual List<KeyValuePair<string, Model.Node>> OnDragAccept(UnityEngine.Object[] objectReferences) { return null; }
        internal virtual void Validate(NodeGUI node) {
           
        }

        internal virtual string GetConnectType(Model.ConnectionPointData output, Model.ConnectionPointData input)
        {
            if(output.Type == input.Type) {
                return output.Type;
            }
            return null;
        }

        internal virtual void DrawNodeGUI(NodeGUI nodeGUI)
        {

        }
        public abstract Model.NodeGraphObj CreateNodeGraphObject(string path);
    }
}
