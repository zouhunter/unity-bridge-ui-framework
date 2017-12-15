using UnityEngine;
using System;
using System.Collections.Generic;
using NodeGraph;

namespace NodeGraph.DataModel {
    /// <summary>
    /// Connection data.
    /// </summary>
	[Serializable]
	public class ConnectionData
    {
        [System.Serializable]
        public class ConnectionInstance : SerializedInstance<Connection>
        {
            public ConnectionInstance() : base() { }
            public ConnectionInstance(ConnectionInstance instance) : base(instance) { }
            public ConnectionInstance(Connection obj) : base(obj) { }
        }

        [SerializeField] private string m_id;
		[SerializeField] private string m_fromNodeId;
		[SerializeField] private string m_fromNodeConnectionPointId;
		[SerializeField] private string m_toNodeId;
		[SerializeField] private string m_toNodeConnectionPoiontId;
		[SerializeField] private string m_label;
        [SerializeField] private ConnectionInstance m_connectionInstance;
        
        private ConnectionData() {
			m_id = Guid.NewGuid().ToString();
		}

		public ConnectionData(string label, Connection connection, ConnectionPointData output, ConnectionPointData input) {
			m_id = Guid.NewGuid().ToString();
			m_label = label;
            m_fromNodeId = output.NodeId;
			m_fromNodeConnectionPointId = output.Id;
			m_toNodeId = input.NodeId;
			m_toNodeConnectionPoiontId = input.Id;

            m_connectionInstance = new ConnectionInstance(connection);
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
		public string Id {
			get {
				return m_id;
			}
		}

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
		public string Label {
			get {
				return m_label;
			}

			set {
				m_label = value;
			}
		}

        /// <summary>
        /// Gets from node identifier.
        /// </summary>
        /// <value>From node identifier.</value>
		public string FromNodeId {
			get {
				return m_fromNodeId;
			}
		}

        public ConnectionInstance Operation
        {
            get
            {
                return m_connectionInstance;
            }
        }
        public bool Validate()
        {
            return m_connectionInstance.Object != null;
        }

        /// <summary>
        /// Gets from node connection point identifier.
        /// </summary>
        /// <value>From node connection point identifier.</value>
		public string FromNodeConnectionPointId {
			get {
				return m_fromNodeConnectionPointId;
			}
		}

        /// <summary>
        /// Gets to node identifier.
        /// </summary>
        /// <value>To node identifier.</value>
		public string ToNodeId {
			get {
				return m_toNodeId;
			}
		}

        /// <summary>
        /// Gets to node connection point identifier.
        /// </summary>
        /// <value>To node connection point identifier.</value>
		public string ToNodeConnectionPointId {
			get {
				return m_toNodeConnectionPoiontId;
			}
		}

		public ConnectionData Duplicate(bool keepGuid = false) {
			ConnectionData newData = new ConnectionData();
			if(keepGuid) {
				newData.m_id = m_id;
			}
			newData.m_label = m_label;
			newData.m_fromNodeId = m_fromNodeId;
			newData.m_fromNodeConnectionPointId = m_fromNodeConnectionPointId;
			newData.m_toNodeId = m_toNodeId;
			newData.m_toNodeConnectionPoiontId = m_toNodeConnectionPoiontId;
            newData.m_connectionInstance = new ConnectionInstance(m_connectionInstance);

            return newData;
		}

		public bool Validate (List<NodeData> allNodes, List<ConnectionData> allConnections) {

			var fromNode = allNodes.Find(n => n.Id == this.FromNodeId);
			var toNode   = allNodes.Find(n => n.Id == this.ToNodeId);

			if(fromNode == null) {
				return false;
			}

			if(toNode == null) {
				return false;
			}

			var outputPoint = fromNode.FindOutputPoint(this.FromNodeConnectionPointId);
			var inputPoint  = toNode.FindInputPoint(this.ToNodeConnectionPointId);

			if(null == outputPoint) {
				return false;
			}

			if(null == inputPoint) {
				return false;
			}

			// update connection label if not matching with outputPoint label
			if( outputPoint.Label != m_label ) {
				m_label = outputPoint.Label;
			}

			return true;
		}

        /// <summary>
        /// Determines if can connect the specified from to.
        /// </summary>
        /// <returns><c>true</c> if can connect the specified from to; otherwise, <c>false</c>.</returns>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
		public static string GetConnectType(NodeData from, NodeData to) {

            var inType = to.Operation.Object.NodeInputType;
            var outType = from.Operation.Object.NodeOutputType;

            if (string.Equals(inType, outType)){
                return to.Operation.Object.NodeInputType;
            }
            else
            {
                return null;
            }
		}
	}
}
