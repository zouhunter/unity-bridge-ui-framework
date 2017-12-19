using UnityEngine;
using System;

namespace NodeGraph.DataModel {

	[Serializable]
	public class ConnectionPointData {

		/**
		* In order to support Unity serialization for Undo, cyclic reference need to be avoided.
		* For that reason, we are storing parentId instead of pointer to parent NodeData
		*/

		[SerializeField] private string id;
		[SerializeField] private string label;
		[SerializeField] private string parentId;
		[SerializeField] private bool isInput;
		[SerializeField] private Rect buttonRect;

		public ConnectionPointData(string id, string label, NodeData parent, bool isInput/*, int orderPriority, bool showLabel */) {
			this.id = id;
			this.label = label;
			this.parentId = parent.Id;
			this.isInput = isInput;
		}

		public ConnectionPointData(string label, NodeData parent, bool isInput) {
			this.id = Guid.NewGuid().ToString();
			this.label = label;
			this.parentId = parent.Id;
			this.isInput = isInput;
		}

		public ConnectionPointData(ConnectionPointData p) {
			this.id 		= p.id;
			this.label		= p.label;
			this.parentId 	= p.parentId;
			this.isInput 	= p.isInput;
			this.buttonRect = p.buttonRect;
		}

		public string Id {
			get {
				return id;
			}
		}

		public string Label {
			get {
				return label;
			}
			set {
				label = value;
			}
		}

		public string NodeId {
			get {
				return parentId;
			}
		}

		public bool IsInput {
			get {
				return isInput;
			}
		}

		public bool IsOutput {
			get {
				return !isInput;
			}
		}

		public Rect Region {
			get {
				return buttonRect;
			}
            set
            {
                buttonRect = value;
            }
		}

		// returns rect for outside marker
		public Rect GetGlobalRegion(Rect baseRect) {
			return new Rect(
				baseRect.x + buttonRect.x,
				baseRect.y + buttonRect.y,
				buttonRect.width,
				buttonRect.height
			);
		}

		// returns rect for connection dot
		

		public Vector2 GetGlobalPosition(Rect baseRect) {
			var x = 0f;
			var y = 0f;

			if (IsInput) {
				x = baseRect.x + 8f;
				y = baseRect.y + buttonRect.y + (buttonRect.height / 2f) - 1f;
			}

			if (IsOutput) {
				x = baseRect.x + baseRect.width;
				y = baseRect.y + buttonRect.y + (buttonRect.height / 2f) - 1f;
			}

			return new Vector2(x, y);
		}

		
     
	}
}
