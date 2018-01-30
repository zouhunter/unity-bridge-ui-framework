using UnityEngine;
using System.Collections.Generic;
using System.Collections;
namespace BridgeUI
{
    public class GameObjectPool
    {
        private Dictionary<GameObject, List<GameObject>> poolObjs = new Dictionary<GameObject, List<GameObject>>();
        private Dictionary<GameObject, float> poolObjTimes = new Dictionary<GameObject, float>();
        private Transform transform;

        public GameObjectPool(Transform parent)
        {
            transform = parent;
        }

        /// <summary>
        /// 用于创建静止的物体，指定父级、坐标
        /// </summary>
        /// <returns></returns>
        public GameObject GetPoolObject(GameObject pfb, Transform parent, bool world, bool resetLocalPosition = true, bool resetLocalScale = false, bool activepfb = false)
        {
            pfb.SetActive(true);
            GameObject currGo;
            ////Debug.Log(pfb.name);
            //如果有预制体为名字的对象小池
            if (poolObjs.ContainsKey(pfb))
            {
                List<GameObject> currentList = null;
                currentList = poolObjs[pfb];
                //遍历每数组，得到一个隐藏的对象
                for (int i = 0; i < currentList.Count; i++)
                {
                    //已经被销毁
                    if (currentList[i] == null)
                    {
                        continue;
                    }
                    if (!currentList[i].activeSelf)
                    {
                        currentList[i].SetActive(true);
                        currentList[i].transform.SetParent(parent, world);
                        if (resetLocalPosition)
                        {
                            currentList[i].transform.localPosition = Vector3.zero;
                        }
                        if (resetLocalScale)
                        {
                            currentList[i].transform.localScale = Vector3.one;
                        }
                        pfb.SetActive(activepfb);
                        poolObjTimes.Remove(currentList[i]);
                        return currentList[i];
                    }
                }
                //当没有隐藏对象时，创建一个并返回
                currGo = CreateAGameObject(pfb, parent, world, resetLocalPosition, resetLocalScale);
                currentList.Add(currGo);
                pfb.SetActive(activepfb);
                return currGo;
            }
            currGo = CreateAGameObject(pfb, parent, world, resetLocalPosition, resetLocalScale);
            //如果没有对象小池
            poolObjs.Add(pfb, new List<GameObject>() { currGo });
            pfb.SetActive(activepfb);
            return currGo;
        }

        /// <summary>
        /// 直接创建一个对象
        /// </summary>
        /// <param name="pfb"></param>
        /// <param name="parent"></param>
        /// <param name="world"></param>
        /// <param name="resetLocalPositon"></param>
        /// <param name="resetLocalScale"></param>
        /// <returns></returns>
        public GameObject CreateAGameObject(GameObject pfb, Transform parent, bool world, bool resetLocalPositon = true, bool resetLocalScale = false)
        {
            GameObject currentGo = Object.Instantiate(pfb);
            currentGo.name = pfb.name;
            currentGo.transform.SetParent(parent, world);
            if (resetLocalPositon)
            {
                currentGo.transform.localPosition = Vector3.zero;
            }
            if (resetLocalScale)
            {
                currentGo.transform.localScale = Vector3.one;
            }
            return currentGo;
        }


        /// <summary>
        /// 放回对象池
        /// </summary>
        /// <param name="go"></param>
        /// <param name="world"></param>
        public void SavePoolObject(GameObject go, bool world = false)
        {
            if (go == null || transform == null) return;
            go.transform.SetParent(transform, world);
            go.SetActive(false);
        }

        /// <summary>
        /// 清除所有对象
        /// </summary>
        public void ClearAllObject()
        {
            foreach (Transform item in transform){
                Object.Destroy(item.gameObject);
            }

            if (poolObjs != null)
                poolObjs.Clear();

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }


        /// <summary>
        /// 清除所有目标对象
        /// </summary>
        /// <param name="pfb"></param>
        public void ClearObjectByPrefab(GameObject pfb)
        {
            if (poolObjs == null)
                return;

            if (!poolObjs.ContainsKey(pfb))
                return;

            var currList = poolObjs[pfb];
            foreach (var item in currList)
            {
                Object.Destroy(item);
            }

            poolObjs.Remove(pfb);
            currList.Clear();
        }
    }
}