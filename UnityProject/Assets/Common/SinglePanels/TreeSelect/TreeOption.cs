using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace BridgeUI.Common
{
    [System.Serializable]
    public struct TreeOption
    {
        public TreeSelectItem prefab;
        public GridLayoutGroup.Axis axisType { get; set; }
        public System.Func<int, TreeSelectRule> ruleGetter { get; set; }
    }

}
