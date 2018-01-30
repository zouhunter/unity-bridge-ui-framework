using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace BridgeUI.Common
{
    public class TreeNodeCreater
    {
        private int deepth;
        private System.Func<int, TreeSelectRule> ruleGetter;
        private Transform parent;
        private TreeSelectItem prefab;
        private GameObjectPool pool;
        private TreeSelectItem[] created;

        public TreeNodeCreater(System.Func<int, TreeSelectRule> ruleGetter,int deepth, Transform parent, TreeSelectItem prefab)
        {
            this.deepth = deepth;
            this.ruleGetter = ruleGetter;
            this.parent = parent;
            this.prefab = prefab;
            pool = UIFacade.PanelPool;
        }

        public TreeSelectItem[] CreateTreeSelectItems(GridLayoutGroup.Axis axis,TreeNode[] childNodes)
        {
            var rule = ruleGetter(deepth);
            created = new TreeSelectItem[childNodes.Length];
            for (int i = 0; i < childNodes.Length; i++)
            {
                var item = pool.GetPoolObject(prefab.gameObject, parent, false);
                TreeSelectItem tsi = item.GetComponent<TreeSelectItem>();
                tsi.InitTreeSelecter(axis,deepth + 1, ruleGetter, childNodes[i],prefab);
                created[i] = tsi;
            }
            return created;
        }
    }

}