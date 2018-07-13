using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeGraph;

namespace NodeGraph.DataModel
{

    /*
	 * Save data which holds all NodeGraph.DataModel NGSettings and configurations.
	 */
    
    public class NodeGraphObj : ScriptableObject
    {
        /*
		 * Important: 
		 * ABG_FILE_VERSION must be increased by one when any structure change(s) happen
		 */
        public const int ABG_FILE_VERSION = 2;

        [SerializeField]
        private List<NodeData> m_allNodes = new List<NodeData>();
        [SerializeField]
        private List<ConnectionData> m_allConnections = new List<ConnectionData>();
        [SerializeField]
        private string m_lastModified;
        [SerializeField]
        private string m_graphDescription;
        [SerializeField]
        private string m_controllerType;
        public static bool log = false;
        void OnEnable()
        {
            Initialize();
            Validate();
        }

        private string GetFileTimeUtcString()
        {
            return DateTime.Now.ToFileTimeUtc().ToString();
        }

        private void Initialize()
        {
            if (string.IsNullOrEmpty(m_lastModified))
            {
                m_lastModified = GetFileTimeUtcString();
                m_allNodes = new List<NodeData>();
                m_allConnections = new List<ConnectionData>();
                m_graphDescription = String.Empty;
            }
        }

        public string ControllerType
        {
            get
            {
                return m_controllerType;
            }
            set
            {
                m_controllerType = value;
            }
        }
        public DateTime LastModified
        {
            get
            {
                long utcFileTime = long.Parse(m_lastModified);
                DateTime d = DateTime.FromFileTimeUtc(utcFileTime);
                return d;
            }
        }

        public string Descrption
        {
            get
            {
                return m_graphDescription;
            }
            set
            {
                m_graphDescription = value;
            }
        }

        public List<NodeData> Nodes
        {
            get
            {
                return m_allNodes;
            }
        }
        public List<ConnectionData> Connections
        {
            get
            {
                return m_allConnections;
            }
        }

        public List<NodeData> CollectAllLeafNodes()
        {

            var nodesWithChild = new List<NodeData>();
            foreach (var c in m_allConnections)
            {
                NodeData n = m_allNodes.Find(v => v.Id == c.FromNodeId);
                if (n != null)
                {
                    nodesWithChild.Add(n);
                }
            }
            return m_allNodes.Except(nodesWithChild).ToList();
        }

        public List<NodeData> CollectAllRootNodes()
        {
            var nodesNoRoot = new List<NodeData>();
            foreach (var c in Nodes)
            {
                if (c.InputPoints.Count == 0)
                    nodesNoRoot.Add(c);
            }
            return nodesNoRoot;
        }

        //
        // Save/Load to disk
        //

        public void ApplyGraph(List<NodeData> nodes, List<ConnectionData> connections)
        {
            if (!Enumerable.SequenceEqual(nodes.OrderBy(v => v.Id), m_allNodes.OrderBy(v => v.Id)) ||
                !Enumerable.SequenceEqual(connections.OrderBy(v => v.Id), m_allConnections.OrderBy(v => v.Id)))
            {
                if(log) Debug.Log("[ApplyGraph] SaveData updated.");
                m_lastModified = GetFileTimeUtcString();
                m_allNodes = nodes;
                m_allConnections = connections;
            }
            else
            {
                if (log) Debug.Log("[ApplyGraph] SaveData update skipped. graph is equivarent.");
            }
        }

        /*
		 * Checks deserialized SaveData, and make some changes if necessary
		 * return false if any changes are perfomed.
		 */
        private bool Validate()
        {
            var changed = false;

            if (m_allNodes != null)
            {
                List<NodeData> removingNodes = new List<NodeData>();
                foreach (var n in m_allNodes)
                {
                    if (!n.Validate())
                    {
                        removingNodes.Add(n);
                        changed = true;
                    }
                }
                m_allNodes.RemoveAll(n => removingNodes.Contains(n));
            }

            if (m_allConnections != null)
            {
                List<ConnectionData> removingConnections = new List<ConnectionData>();
                foreach (var c in m_allConnections)
                {
                    if (!c.Validate(m_allNodes, m_allConnections))
                    {
                        removingConnections.Add(c);
                        changed = true;
                    }
                }
                m_allConnections.RemoveAll(c => removingConnections.Contains(c));
            }

            if (changed)
            {
                m_lastModified = GetFileTimeUtcString();
            }

            return !changed;
        }
    }
}