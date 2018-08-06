using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI.Common;
using BridgeUI;

namespace BridgeUI.Control
{
    public class ListItemCreater<T> where T : MonoBehaviour
    {
        public List<T> CreatedItems { get { return createdItems; } }
        Transform parent { get; set; }
        T pfb { get; set; }
        private BridgeUI.GameObjectPool _objectManager;
        private bool isword;
        List<T> createdItems = new List<T>();
        public ListItemCreater(Transform parent, T pfb)
        {
            this.parent = parent;
            this.pfb = pfb;
            pfb.gameObject.SetActive(false);
            _objectManager = new GameObjectPool(parent);
            isword = !parent.GetComponent<RectTransform>();
        }

        public T[] CreateItems(int length)
        {
            ClearOldItems();
            if (length <= 0) return new T[0];

            GameObject go;
            for (int i = 0; i < length; i++)
            {
                go = _objectManager.GetPoolObject(pfb.gameObject, parent, false);
                T scr = go.GetComponent<T>();
                createdItems.Add(scr);
            }
            return createdItems.ToArray();
        }

        public T AddItem()
        {
            if (pfb == null) return null;
            GameObject go;
            go = _objectManager.GetPoolObject(pfb.gameObject, parent, false);
            T scr = go.GetComponent<T>();
            createdItems.Add(scr);
            return scr;
        }

        public void RemoveItem(T item)
        {
            createdItems.Remove(item);
            _objectManager.SavePoolObject(item.gameObject, isword);
        }

        public void ClearOldItems()
        {
            foreach (var item in createdItems)
            {
                _objectManager.SavePoolObject(item.gameObject, isword);
            }

            createdItems.Clear();
        }
    }
}