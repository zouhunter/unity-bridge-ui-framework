using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace AssetBundles
{
    public class Tuple<T, S, U>
    {
        public T Item1;
        public S Item2;
        public U Item3;
        public Tuple(T item1, S item2, U item3)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
        }
    }
}