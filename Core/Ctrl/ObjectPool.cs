using System;
using System.Collections;
namespace BridgeUI
{
    /// <summary>
    /// Represents a pool of objects that we can pull from in order
    /// to prevent constantly reallocating new objects. This collection
    /// is meant to be fast, so we limit the "lock" that we use and do not
    /// track the instances that we hand out.
    /// </summary>
    /// <typeparam name="T">The type of object in the pool.</typeparam>
    public class ObjectPool<T> 
    {
        /// <summary>
        /// 创建方法
        /// </summary>
        Func<T> CreateFunc;
        /// <summary>
        /// Number of items to grow the array by if needed
        /// </summary>
        private int mGrowSize = 5;

        /// <summary>
        /// Pool objects
        /// </summary>
        private T[] mPool;

        /// <summary>
        /// Index into the pool
        /// </summary>
        private int mNextIndex = 0;

        /// <summary>
        /// Initializes a new instance of the ObjectPool class.
        /// </summary>
        /// <param name="size">The size of the object pool.</param>
        public ObjectPool(int rSize, Func<T> createFunc)
        {
            this.CreateFunc = createFunc;
            // Initialize the pool
            Resize(rSize, false);
        }

        /// <summary>
        /// Initializes a new instance of the ObjectPool class.
        /// </summary>
        /// <param name="rSize">The initial size of the object pool.</param>
        /// <param name="rGrowize">Increment to grow the pool by when needed</param>
        public ObjectPool(int rSize, int rGrowSize)
        {
            mGrowSize = rGrowSize;

            // Initialize the pool
            Resize(rSize, false);
        }

        /// <summary>
        /// The total size of the pool
        /// </summary>
        /// <value>The length.</value>
        public virtual int Length
        {
            get { return mPool.Length; }
        }

        /// <summary>
        /// The number of items available in the pool
        /// </summary>
        public virtual int Available
        {
            get { return mPool.Length - mNextIndex; }
        }

        /// <summary>
        /// The number of items that have been allocated
        /// </summary>
        public virtual int Allocated
        {
            get { return mNextIndex; }
        }

        /// <summary>
        /// Pulls an item from the object pool or creates more
        /// if needed.
        /// </summary>
        /// <returns>Object of the specified type</returns>
        public virtual T Allocate()
        {
            T lItem = default(T);
            if (mPool.Length > 30)
            {
                UnityEngine.Debug.Log("mPool.Length:" + mPool.Length + "Reseted");
                Reset();
            }
            // Creates extra items if needed
            if (mNextIndex >= mPool.Length)
            {
                if (mGrowSize > 0)
                {
                    Resize(mPool.Length + mGrowSize, true);
                }
                else
                {
                    return lItem;
                }
            }

            // Returns the item. For performance, we'll use an if
            // statement instead of a try-catch block.
            if (mNextIndex >= 0 && mNextIndex < mPool.Length)
            {
                lItem = mPool[mNextIndex];
                mNextIndex++;
            }

            return lItem;
        }

        /// <summary>
        /// Sends an item back to the pool.
        /// </summary>
        /// <param name="rInstance">Object to return</param>
        public virtual void Release(T rInstance)
        {
            if (mNextIndex > 0)
            {
                mNextIndex--;
                mPool[mNextIndex] = rInstance;
            }
        }

        /// <summary>
        /// Rebuilds the pool with new instances
        /// 
        /// Note:
        /// This is a fast pool so we don't track the instances
        /// that are handed out. Releasing an instance also overwrites
        /// what was there. That means we can't have a "ReleaseAll"
        /// function that allows the array to be used again. The best
        /// we can do is abandon what we have given out and rebuild all our instances.
        /// </summary>
        /// <param name="rInstance">Object to return</param>
        public virtual void Reset()
        {
            // Determine the length to initialize
            int lLength = mGrowSize;
            //if (mPool != null) { lLength = mPool.Length; }

            // Rebuild our elements
            Resize(lLength, false);

            // Reset the pool stats
            mNextIndex = 0;
        }

        /// <summary>
        /// Resize the pool array
        /// </summary>
        /// <param name="rSize">New size of the pool</param>
        /// <param name="rCopyExisting">Determines if we copy contents from the old pool</param>
        public virtual void Resize(int rSize, bool rCopyExisting)
        {
            lock (this)
            {
                int lCount = 0;

                // Build the new array and copy the contents
                T[] lNewPool = new T[rSize];

                if (mPool != null && rCopyExisting)
                {
                    lCount = mPool.Length;
                    Array.Copy(mPool, lNewPool, Math.Min(lCount, rSize));
                }

                // Allocate items in the new array
                for (int i = lCount; i < rSize; i++)
                {
                    lNewPool[i] = CreateFunc.Invoke();
                }

                // Replace the old array
                mPool = lNewPool;
            }
        }
    }
}