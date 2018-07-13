using UnityEngine;
using System;
using System.Collections.Generic;

namespace NodeGraph.DataModel
{

    /// <summary>
    /// Node data.
    /// </summary>
	[Serializable]
    public class NodeData
    {
        [SerializeField]
        private string m_name;
        [SerializeField]
        private string m_id;
        [SerializeField]
        private float m_x;
        [SerializeField]
        private float m_y;
        [SerializeField]
        private Node m_node;
        [SerializeField]
        private List<ConnectionPointData> m_inputPoints;
        [SerializeField]
        private List<ConnectionPointData> m_outputPoints;

        private bool m_nodeNeedsRevisit;

        /*
		 * Properties
		 */

        public bool NeedsRevisit
        {
            get
            {
                return m_nodeNeedsRevisit;
            }
            set
            {
                m_nodeNeedsRevisit = value;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        public string Id
        {
            get
            {
                return m_id;
            }
        }
        public Node Object
        {
            get
            {
                return m_node;
            }
            set
            {
                m_node = value;
            }
        }

        public float X
        {
            get
            {
                return m_x;
            }
            set
            {
                m_x = value;
            }
        }

        public float Y
        {
            get
            {
                return m_y;
            }
            set
            {
                m_y = value;
            }
        }

        public List<ConnectionPointData> InputPoints
        {
            get
            {
                return m_inputPoints;
            }
        }

        public List<ConnectionPointData> OutputPoints
        {
            get
            {
                return m_outputPoints;
            }
        }


        /// <summary>
        /// Create new node from GUI.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="node">Node.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
		public NodeData(string name, Node node, float x, float y)
        {
            m_id = Guid.NewGuid().ToString();
            m_name = name;
            m_x = x;
            m_y = y;
            m_node = node;
            m_nodeNeedsRevisit = false;

            m_inputPoints = new List<ConnectionPointData>();
            m_outputPoints = new List<ConnectionPointData>();
        }

        public NodeData(NodeData node, bool keepId = false)
        {
            m_name = node.m_name;
            m_x = node.m_x;
            m_y = node.m_y;
            m_nodeNeedsRevisit = false;
            m_inputPoints = new List<ConnectionPointData>();
            m_outputPoints = new List<ConnectionPointData>();

            if (keepId)
            {
                m_id = node.m_id;
                node.InputPoints.ForEach(p => m_inputPoints.Add(new ConnectionPointData(p)));
                node.OutputPoints.ForEach(p => m_outputPoints.Add(new ConnectionPointData(p)));
            }
            else
            {
                m_id = Guid.NewGuid().ToString();
            }
            m_node = UnityEngine.Object.Instantiate((UnityEngine.Object)node.m_node) as Node;
            m_node.name = node.m_node.name;
        }

        public NodeData Duplicate(bool keepId = false)
        {
            return new NodeData(this, keepId);
        }

        public ConnectionPointData AddInputPoint(string label, string type, int max = 1)
        {
            var p = new ConnectionPointData(label, type, max, this, true);
            m_inputPoints.Add(p);
            return p;
        }

        public ConnectionPointData AddOutputPoint(string label, string type, int max = 1)
        {
            var p = new ConnectionPointData(label, type, max, this, false);
            m_outputPoints.Add(p);
            return p;
        }

        public ConnectionPointData FindInputPoint(string id)
        {
            return m_inputPoints.Find(p => p.Id == id);
        }

        public ConnectionPointData FindOutputPoint(string id)
        {
            return m_outputPoints.Find(p => p.Id == id);
        }

        public ConnectionPointData FindConnectionPoint(string id)
        {
            var v = FindInputPoint(id);
            if (v != null)
            {
                return v;
            }
            return FindOutputPoint(id);
        }

        public bool Validate()
        {
            return m_node != null;
        }

        public bool CompareIgnoreGUIChanges(NodeData rhs)
        {

            if (m_node == null && rhs.m_node != null ||
                m_node != null && rhs.m_node == null)
            {
                LogUtility.Logger.LogFormat(LogType.Log, "{0} and {1} was different: {2}", Name, rhs.Name, "Node Type");
                return false;
            }

            //if (m_node.ClassName != rhs.m_node.ClassName)
            //{
            //    LogUtility.Logger.LogFormat(LogType.Log, "{0} and {1} was different: {2}", Name, rhs.Name, "Node Type");
            //    return false;
            //}

            //if (m_node.Data != rhs.m_node.Data)
            //{
            //    LogUtility.Logger.LogFormat(LogType.Log, "{0} and {1} was different: {2}", Name, rhs.Name, "Node Variable");
            //    return false;
            //}

            if (m_inputPoints.Count != rhs.m_inputPoints.Count)
            {
                LogUtility.Logger.LogFormat(LogType.Log, "{0} and {1} was different: {2}", Name, rhs.Name, "Input Count");
                return false;
            }

            if (m_outputPoints.Count != rhs.m_outputPoints.Count)
            {
                LogUtility.Logger.LogFormat(LogType.Log, "{0} and {1} was different: {2}", Name, rhs.Name, "Output Count");
                return false;
            }

            foreach (var pin in m_inputPoints)
            {
                if (rhs.m_inputPoints.Find(x => pin.Id == x.Id) == null)
                {
                    LogUtility.Logger.LogFormat(LogType.Log, "{0} and {1} was different: {2}", Name, rhs.Name, "Input point not found");
                    return false;
                }
            }

            foreach (var pout in m_outputPoints)
            {
                if (rhs.m_outputPoints.Find(x => pout.Id == x.Id) == null)
                {
                    LogUtility.Logger.LogFormat(LogType.Log, "{0} and {1} was different: {2}", Name, rhs.Name, "Output point not found");
                    return false;
                }
            }


            return true;
        }

    }
}
