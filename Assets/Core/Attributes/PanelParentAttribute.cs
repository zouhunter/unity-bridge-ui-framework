using System;
using System.Collections.Generic;

namespace BridgeUI
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PanelParentAttribute : System.Attribute
    {
        public int sortIndex;
        public PanelParentAttribute(int sortIndex = 0)
        {
            
        }
    }

    public class ComparePanelParentType : IComparer<Type>
    {
        int IComparer<Type>.Compare(Type x, Type y)
        {
            var att_x = GetAttribute(x);
            var att_y = GetAttribute(y);
            if (att_x.sortIndex > att_y.sortIndex)
            {
                return 1;
            }
            else if (att_x.sortIndex < att_y.sortIndex)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private PanelParentAttribute GetAttribute(Type type)
        {
            var attributes = type.GetCustomAttributes(false);
            var attribute = Array.Find(attributes, x => x is PanelParentAttribute);
            return attribute as PanelParentAttribute;
        }
    }
}
