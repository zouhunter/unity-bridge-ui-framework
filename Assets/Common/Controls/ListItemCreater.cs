#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 11:27:06
    * 说    明：       
                       1.调用了内部的对象池
                       2.只创建少量的列表对象时使用
                       3.对于会发生变化的对象,在Revert中重置
                       4.按父级是否有Recttransform判断是否是ui
* ************************************************************************************/
#endregion
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Internal;

namespace BridgeUI.Common
{
    public interface IListItem {
        void Revert();
    }
    /// <summary>
    /// 这是一个列表创建器（用于快速创建一组对象）
    /// 建议数量 < 100
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListItemCreater<T>  : MonoBehaviour where T : MonoBehaviour, IListItem
    {
        public Transform parent;
        public T pfb;
        public List<T> CreatedItems { get { return createdItems; } }


        private GameObjectPool objectPool;
        private bool isword;
        List<T> createdItems = new List<T>();

        private void Awake()
        {
            pfb.gameObject.SetActive(false);
            objectPool = UIFacade.PanelPool;
            isword = !parent.GetComponent<RectTransform>();
        }

        public T[] CreateItems(int length)
        {
            ClearOldItems();
            if (length <= 0) return new T[0];

            GameObject go;
            for (int i = 0; i < length; i++)
            {
                go = objectPool.GetPoolObject(pfb.gameObject, parent, isword);
                T scr = go.GetComponent<T>();
                createdItems.Add(scr);
            }
            return createdItems.ToArray();
        }

        public T AddItem()
        {
            if (pfb == null) return null;
            GameObject go;
            go = objectPool.GetPoolObject(pfb.gameObject, parent, isword);
            T scr = go.GetComponent<T>();
            createdItems.Add(scr);
            return scr;
        }

        public void RemoveItem(T item)
        {
            createdItems.Remove(item);
            objectPool.SavePoolObject(item.gameObject, isword);
        }

        public void ClearOldItems()
        {
            foreach (var item in createdItems)
            {
                objectPool.SavePoolObject(item.gameObject, isword);
            }

            createdItems.Clear();
        }
    }

}