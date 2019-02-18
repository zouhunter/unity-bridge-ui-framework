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

namespace BridgeUI.Control
{
    /// <summary>
    /// /// <summary>
    /// 这是一个列表创建器（用于快速创建一组对象）
    /// 建议数量100
    /// </summary>
    public class ListViewer : BridgeUIControl
    {
        public List<GameObject> CreatedItems { get { return createdItems; } }
        [SerializeField]
        private Transform parent;
        [SerializeField]
        private GameObject pfb;
        protected GameObjectPool objectPool;
        protected bool isword;
        protected List<GameObject> createdItems = new List<GameObject>();

        public  UnityAction<GameObject> onGetFrom { get; set; }
        public  UnityAction<GameObject> onSaveBack { get; set; }
        private GameObject itemPool;

        public GameObject[] CreateItems(int length,bool clean = true)
        {
            if (!Initialized) return null;

            if(clean){
                ClearOldItems();
            }

            GameObject temp;
            for (int i = 0; i < length; i++)
            {
                temp = objectPool.GetPoolObject(pfb.gameObject, parent, isword);
                createdItems.Add(temp);
            }

            return createdItems.ToArray();
        }

        public GameObject AddItem()
        {
            if (!Initialized) return null;

            if (pfb == null) return null;
            GameObject go;
            go = objectPool.GetPoolObject(pfb.gameObject, parent, isword);
            if(onGetFrom != null){
                onGetFrom.Invoke(go);
            }
            createdItems.Add(go);
            return go;
        }

        public void RemoveItem(GameObject item)
        {
            if (!Initialized) return;

            createdItems.Remove(item);
            objectPool.SavePoolObject(item.gameObject, isword);
        }

        public void ClearOldItems()
        {
            if (!Initialized) return;

            foreach (var item in createdItems)
            {
               if(onSaveBack != null) onSaveBack.Invoke(item);
                objectPool.SavePoolObject(item.gameObject, isword);
            }

            createdItems.Clear();
        }

        protected override void OnInititalize()
        {
            objectPool = Utility.CreatePool(transform, out itemPool);
            pfb.gameObject.SetActive(false);
            isword = !parent.GetComponent<RectTransform>();
        }

        protected override void OnUnInitialize()
        {
            ClearOldItems();
            objectPool = null;
            if (itemPool)
                Destroy(itemPool);
        }
    }

}