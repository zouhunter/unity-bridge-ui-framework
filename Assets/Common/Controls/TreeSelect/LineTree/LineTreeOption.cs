using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace BridgeUI.Common.Tree
{
    [System.Serializable]
    public struct LineTreeOption
    {
        public LineTreeItem prefab;
        public GridLayoutGroup.Axis axisType { get; set; }
        public System.Func<int, LineTreeRule> ruleGetter { get; set; }
    }

}
