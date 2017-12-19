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
    public abstract class NodeGraphController
    {
        private List<NodeException> m_nodeExceptions = new List<NodeException>();
        private Model.ConfigGraph m_targetGraph;

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

        public Model.ConfigGraph TargetGraph
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
        }
        protected virtual void JudgeNodeExceptions(Model.ConfigGraph m_targetGraph, List<NodeException> m_nodeExceptions) { }
        protected virtual void BuildFromGraph(Model.ConfigGraph m_targetGraph) { }
        internal virtual List<KeyValuePair<string, Model.Node>> OnDragAccept(UnityEngine.Object[] objectReferences) { return null; }
        internal virtual void Validate(NodeGUI node) { }
    }
}
