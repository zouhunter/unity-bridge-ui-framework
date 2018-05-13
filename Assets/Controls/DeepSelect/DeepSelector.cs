using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;

namespace BridgeUI.Control
{
    public class DeepSelector : MonoBehaviour
    {
        [SerializeField]
        private ListViewer result;
        [SerializeField]
        private ListViewer creater;
        private Tree.TreeNode _root;
        private int currentdeep = 0;
        private List<string> currentSelection = new List<string>();
        private Tree.TreeNode active;

        public UnityAction<string[]> onSelect { get; set; }
        public UnityAction<int[]> onSelectID { get; set; }
        public Tree.TreeNode Option
        {
            get
            {
                return _root;
            }
            set
            {
                active = _root = value;
                OnHeadSelect(0);
            }
        }
        
        public void ChargeSelection(string[] keys)
        {
            result.ClearOldItems();

            if (keys.Length > 0)
            {
                var created = result.CreateItems(keys.Length);
                for (int i = 0; i < created.Length; i++)
                {
                    var deep = i;
                    var item = created[i].GetComponent<DeepSelectItem>();
                    item.Init(keys[i], (x) => { OnHeadSelect(deep); });
                }
            }
          
        }

        //private void UpdateOption()
        //{
        //    Debug.Assert(Option != null, "option is Empty");

        //    if (Option.childern == null || Option.childern.Count == 0)
        //    {
        //        if (onSelect != null)
        //            onSelect.Invoke(active.FullPath);
        //    }
        //    else
        //    {

        //        foreach (var item in Option.childern)
        //        {
        //            item.ParentItem = Option;
        //        }

        //        currentSelection.Clear();
        //        ChargeSelection(currentSelection.ToArray());
        //        OnSelect();
        //    }
        //}

        private void CreateSelectList(string[] selectItems)
        {
            Debug.Assert(selectItems != null);
            creater.ClearOldItems();
            var created = creater.CreateItems(selectItems.Length);
            for (int i = 0; i < created.Length; i++)
            {
                var item = created[i].GetComponent<DeepSelectItem>();
                item.Init(selectItems[i], OnSelect);
            }
        }
        private void OnHeadSelect(int dp)
        {
            active = _root;
            for (int i = 0; i < dp; i++){
                var key = currentSelection[i];
                active = active.childern.Find(x => x.name == key);
            }

            foreach (var item in active.childern){
                item.ParentItem = active;
            }

            currentSelection.RemoveRange(dp, currentSelection.Count - dp);
            ChargeSelection(currentSelection.ToArray());
            currentdeep = dp;
            OnSelect(null);
        }

        private void OnSelect(string key)
        {
            bool enterChild = false;

            if (!string.IsNullOrEmpty(key))
            {
                //先找子级
                var child = active.GetChildItem(key);
                if (child != null)
                {
                    enterChild = true;
                    active = child;
                    foreach (var item in active.childern)
                    {
                        item.ParentItem = active;
                    }
                }
                else//再找兄弟级
                {
                    var borther = active.ParentItem.GetChildItem(key);
                    if (borther != null)
                    {
                        active = borther;
                        enterChild = true;
                    }
                }
            }
            else
            {
                enterChild = true;
            }
            if (active.childern != null && active.childern.Count > 0)
            {
                if (enterChild)
                {
                    currentdeep++;
                    CreateSelectList(active.childern.ConvertAll<string>(x => x.name).ToArray());
                    if (!string.IsNullOrEmpty(key))
                    {
                        currentSelection.Add(key);
                        ChargeSelection(currentSelection.ToArray());
                    }
                }
            }
            else
            {
                Debug.Assert(active != null, "active :Null", gameObject);
                OnSelectionChanged();
            }
        }

        private void OnSelectionChanged()
        {
            if (onSelect != null)
                onSelect.Invoke(active.FullPath);
        }
    }
}