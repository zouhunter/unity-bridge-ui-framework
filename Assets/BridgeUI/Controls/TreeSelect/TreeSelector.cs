using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BridgeUI.Control.Tree
{
    public abstract class TreeSelector : BridgeUIControl
    {
        protected TreeNode rootNode;
        public UnityAction<int[]> onSelectID { get; set; }
        public UnityAction<string[]> onSelect { get; set; }

        public virtual void CreateTree(TreeNode nodeBase)
        {
            if (!Initialized) return;

            this.rootNode = nodeBase;
            RebuildRelationship(rootNode);
        }
        protected virtual void RebuildRelationship(TreeNode item)
        {
            if (item == null || item.childern == null) return;

            foreach (var child in item.childern)
            {
                child.ParentItem = item;
                RebuildRelationship(child);
            }
        }
        public abstract void SetSelect(params string[] path);
        public abstract void SetSelect(params int[] path);
        public abstract void AutoSelectFirst();
        protected void OnSelectionChanged(IList<string> path)
        {
            if (onSelect != null)
            {
                onSelect.Invoke(path.ToArray());
            }

            if (onSelectID != null)
            {
                onSelectID(GetIDPath(path));
            }
        }
        protected int[] GetIDPath(IList<string> path)
        {
            var idList = new List<int>();
            var parent = rootNode;
            for (int i = 0; i < path.Count; i++)
            {
                var id = System.Array.FindIndex( parent.childern,x => x.name == path[i]);
                idList.Add(id);
                parent = parent.childern[id];
            }
            return idList.ToArray();
        }

        public abstract void ClearTree();
    }

}