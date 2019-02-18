using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeUI
{
    public abstract class BridgeUIControl : MonoBehaviour, IUIControl
    {
        protected object context;
        private bool initialized;
        public bool Initialized
        {
            get
            {
                return initialized;
            }
        }
        public void Initialize(object context = null)
        {
            if (!initialized)
            {
                this.context = context;
                initialized = true;
                OnInititalize();
            }
        }
        public void Recover()
        {
            if (initialized)
            {
                OnUnInitialize();
                initialized = false;
                context = null;
            }
        }
        protected abstract void OnInititalize();
        protected abstract void OnUnInitialize();
    }
}