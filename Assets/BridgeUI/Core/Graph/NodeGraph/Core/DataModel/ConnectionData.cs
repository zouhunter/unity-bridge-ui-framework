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
        [SerializeField] private string m_id;
		[SerializeField] private string m_fromNodeId;
		[SerializeField] private string m_fromNodeConnectionPointId;
		[SerializeField] private string m_toNodeId;
		[SerializeField] private string m_toNodeConnectionPoiontId;
		[SerializeField] private string m_type;
        [SerializeField] private Connection m_connection;

        private ConnectionData() {
			m_id = Guid.NewGuid().ToString();
		}

        public ConnectionData(ConnectionData data)
        {
            m_id = data.Id;
            m_type = data.m_type ;
            m_fromNodeId = data.m_fromNodeId;
            m_fromNodeConnectionPointId = data.m_fromNodeConnectionPointId;
            m_toNodeId = data.m_toNodeId;
            m_toNodeConnectionPoiontId = data.m_toNodeConnectionPoiontId;
            m_connection = Connection.Instantiate(data.Object);
            m_connection.name = data.Object.name;
        }

		public ConnectionData(string type, Connection connection, ConnectionPointData output, ConnectionPointData input) {
			m_id = Guid.NewGuid().ToString();
			m_type = type;
            m_fromNodeId = output.NodeId;
			m_fromNodeConnectionPointId = output.Id;
			m_toNodeId = input.NodeId;
			m_toNodeConnectionPoiontId = input.Id;
            m_connection = connection;
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
		public string ConnectionType {
			get {
				return m_type;
			}

			set {
				m_type = value;
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
        public Connection Object
        {
            get
            {
                return m_connection;
            }
            set
            {
                m_connection = value;
            }
        }
        //public ConnectionInstance Operation
        //{
        //    get
        //    {
        //        return m_connectionInstance;
        //    }
        //}
        public bool Validate()
        {
            //return m_connectionInstance.Object != null;
            return m_connection != null;
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
			newData.m_type = m_type;
			newData.m_fromNodeId = m_fromNodeId;
			newData.m_fromNodeConnectionPointId = m_fromNodeConnectionPointId;
			newData.m_toNodeId = m_toNodeId;
			newData.m_toNodeConnectionPoiontId = m_toNodeConnectionPoiontId;
            //newData.m_connectionInstance = new ConnectionInstance(m_connectionInstance);
            newData.m_connection = UnityEngine.Object.Instantiate<Connection>(m_connection);
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
			if( outputPoint.Type != m_type ) {
				m_type = outputPoint.Type;
			}

			return true;
		}

   
	}
}
