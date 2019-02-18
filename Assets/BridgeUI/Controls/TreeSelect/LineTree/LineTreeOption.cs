using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace BridgeUI.Control.Tree
{
    [System.Serializable]
    public struct LineTreeOption
    {
        public LineTreeItem prefab;
        public GridLayoutGroup.Axis axisType { get; set; }
        public System.Func<int, LineTreeRule> ruleGetter { get; set; }
        public GameObjectPool pool;
    }

}
