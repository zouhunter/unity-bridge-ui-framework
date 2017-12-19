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
            object o = type.Assembly.CreateInstance(type.FullName);
            return (Node)o;
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