using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace BridgeUI.Control.Tree
{
    public class LineTreeItemCreater
    {
        private int deepth;
        private Transform parent;
        private LineTreeItem[] created;
        private LineTreeOption option;
        private ToggleGroup group;
        public ToggleGroup Group { get { return group; } }
        public LineTreeItem[] CreatedItems { get { return created; } }
        private GameObjectPool pool { get { return option.pool; } }

        public LineTreeItemCreater(int deepth, Transform parent, LineTreeOption option)
        {
            this.deepth = deepth;
            this.parent = parent;
            this.option = option;
        }

        public LineTreeItem[] CreateTreeSelectItems(TreeNode[] childNodes)
        {
            var prefab = option.prefab;
            var rule = option.ruleGetter(deepth + 1);

            if(rule.makeGroup) {
                group = InitGroup();
            }

            created = new LineTreeItem[childNodes.Length];
            for (int i = 0; i < childNodes.Length; i++)
            {
                var item = pool.GetPoolObject(prefab.gameObject, parent, false);
                LineTreeItem tsi = item.GetComponent<LineTreeItem>();
                tsi.InitTreeSelecter(deepth + 1, childNodes[i],option);
                if (rule.makeGroup) tsi.SetGroup(group);
                created[i] = tsi;
            }
            return created;
        }

        internal void SetChildActive(List<int> list)
        {
            if (list == null || list.Count == 0) return;

            var key = list[0];

            if (created.Length <= key) return;

            var item = created[key];

            if (item == null) return;
            item.SetToggle(true);

            if (item.Creater == null) return;
            list.RemoveAt(0);
            item.Creater.SetChildActive(list);
        }

        private ToggleGroup InitGroup()
        {
            group = parent.GetComponent<ToggleGroup>();
            if (group == null)
            {
                group = parent.gameObject.AddComponent<ToggleGroup>();
            }
            //group.allowSwitchOff = true;
            return group;
        }

        internal void Clear()
        {
            if (created != null)
            {
                foreach (var item in created)
                {
                    if (item.Creater != null)
                        item.Creater.Clear();

                    if(pool!= null)  pool.SavePoolObject(item.gameObject, false);

                    if (item.childContent != null)
                        UnityEngine.Object.DestroyImmediate(item.childContent.gameObject);
                }
            }
        }
    }

}