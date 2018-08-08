using System;
using System.Collections.Generic;

namespace BridgeUI.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PanelParentAttribute : System.Attribute
    {
        public int sortIndex;
        public PanelParentAttribute(int sortIndex = 0)
        {
            
        }
    }

}
