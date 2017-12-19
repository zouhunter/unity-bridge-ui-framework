using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NodeGraph.DataModel {

	/*
	 * Save data which holds all NodeGraph.DataModel NGSettings and configurations.
	 */ 
	[CreateAssetMenu(fileName = "NodeGraph", menuName = "ConfigGraph", order = 650 )]
	public class ConfigGraph : ScriptableObject {

		/*
		 * Important: 
		 * ABG_FILE_VERSION must be increased by one when any structure change(s) happen
		 */ 
		public const int ABG_FILE_VERSION = 2;

		[SerializeField] private List<NodeData> m_allNodes;
		[SerializeField] private List<ConnectionData> m_allConnections;
		[SerializeField] private string m_lastModified;
		[SerializeField] private int m_version;
		[SerializeField] private string m_graphDescription;
		[SerializeField] private bool m_useAsAssetPostprocessor;
        [SerializeField] private string m_controllerType;

		void OnEnable() {
			Initialize();
			Validate();
		}

		private string GetFileTimeUtcString() {
			return DateTime.UtcNow.ToFileTimeUtc().ToString();
		}

		private void Initialize() {
			if(string.IsNullOrEmpty(m_lastModified)) {
				m_lastModified = GetFileTimeUtcString();
				m_allNodes = new List<NodeData>();
				m_allConnections = new List<ConnectionData>();
				m_version = ABG_FILE_VERSION;
				m_graphDescription = String.Empty;
                SetGraphDirty();
            }
        }

		public bool UseAsAssetPostprocessor {
			get {  
				return m_useAsAssetPostprocessor;
			}
			set {
				m_useAsAssetPostprocessor = value;
				SetGraphDirty();
			}
		}

        public string ControllerType
        {
            get {
                return m_controllerType;
            }
            set
            {
                m_controllerType = value;
            }
        }
		public DateTime LastModified {
			get {
				long utcFileTime = long.Parse(m_lastModified);
				DateTime d = DateTime.FromFileTimeUtc(utcFileTime);

				return d;
			}
		}

		public string Descrption {
			get{
				return m_graphDescription;
			}
			set {
				m_graphDescription = value;
				SetGraphDirty();
			}
		}

		public int Version {
			get {
				return m_version;
			}
		}

		public List<NodeData> Nodes {
			get{ 
				return m_allNodes;
			}
		}

		public List<ConnectionData> Connections {
			get{ 
				return m_allConnections;
			}
		}

		public List<NodeData> CollectAllLeafNodes() {

			var nodesWithChild = new List<NodeData>();
			foreach (var c in m_allConnections) {
				NodeData n = m_allNodes.Find(v => v.Id == c.FromNodeId);
				if(n != null) {
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
                if(c.InputPoints.Count == 0)
                nodesNoRoot.Add(c);
            }
            return nodesNoRoot;
        }

		public void Save() {
			m_allNodes.ForEach(n => n.Operation.Save());
			SetGraphDirty();
		}

		public void SetGraphDirty() {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
		}

		//
		// Save/Load to disk
		//

		public void ApplyGraph(List<NodeData> nodes, List<ConnectionData> connections) {
			if( !Enumerable.SequenceEqual(nodes.OrderBy(v => v.Id), m_allNodes.OrderBy(v => v.Id)) ||
				!Enumerable.SequenceEqual(connections.OrderBy(v => v.Id), m_allConnections.OrderBy(v => v.Id)) ) 
			{
				Debug.Log("[ApplyGraph] SaveData updated.");

				m_version = ABG_FILE_VERSION;
				m_lastModified = GetFileTimeUtcString();
				m_allNodes = nodes;
				m_allConnections = connections;
				Save();
			} else {
                Debug.Log("[ApplyGraph] SaveData update skipped. graph is equivarent.");
			}
		}


		public static ConfigGraph CreateNewGraph(string controllerType) {
			var data = ScriptableObject.CreateInstance<ConfigGraph>();
            data.m_controllerType = controllerType;
			return data;
		}

		/*
		 * Checks deserialized SaveData, and make some changes if necessary
		 * return false if any changes are perfomed.
		 */
		private bool Validate () {
			var changed = false;

			if(m_allNodes != null) {
				List<NodeData> removingNodes = new List<NodeData>();
				foreach (var n in m_allNodes) {
					if(!n.Validate()) {
						removingNodes.Add(n);
						changed = true;
					}
				}
				m_allNodes.RemoveAll(n => removingNodes.Contains(n));
			}

			if(m_allConnections != null) {
				List<ConnectionData> removingConnections = new List<ConnectionData>();
				foreach (var c in m_allConnections) {
					if(!c.Validate(m_allNodes, m_allConnections)) {
						removingConnections.Add(c);
						changed = true;
					}
				}
				m_allConnections.RemoveAll(c => removingConnections.Contains(c));
			}

			if(changed) {
				m_lastModified = GetFileTimeUtcString();
			}

			return !changed;
		}
	}
}