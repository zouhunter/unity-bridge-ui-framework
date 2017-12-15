

namespace NodeGraph.DataModel
{
    /// <summary>
    /// Node.
    /// </summary>
	public abstract class Node {
		public virtual string NodeInputType {
			get {
				return null;
			}
		}
		public virtual string NodeOutputType {
			get {
				return null;
			}
		}
        public abstract void Initialize(NodeData data);
        public virtual bool IsValidInputConnectionPoint(ConnectionPointData point) { return true; }
    }

}
