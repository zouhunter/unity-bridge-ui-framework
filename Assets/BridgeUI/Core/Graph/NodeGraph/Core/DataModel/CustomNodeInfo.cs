using System;

namespace NodeGraph.DataModel
{
    public struct CustomNodeInfo : IComparable
    {
        public CustomNode node;
        public Type type;

        public CustomNodeInfo(Type t, CustomNode n)
        {
            node = n;
            type = t;
        }

        public Node CreateInstance()
        {
            Node o = UnityEngine.ScriptableObject.CreateInstance(type) as Node;
            o.name = type.FullName;
            return o;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            CustomNodeInfo rhs = (CustomNodeInfo)obj;
            return node.OrderPriority - rhs.node.OrderPriority;
        }
    }
}