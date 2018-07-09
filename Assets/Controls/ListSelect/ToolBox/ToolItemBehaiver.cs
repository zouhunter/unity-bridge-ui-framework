using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace BridgeUI.Control
{
    public abstract class ToolItemBehaiver : MonoBehaviour, IUseAble,ICustomMove
    {
        public Vector3 oringalPos { get; set; }
        public virtual bool saveAble { get { return true; } }
        public virtual bool hideOnUse { get { return true; } }
        public virtual void OnReset() { }
        public abstract bool TryUse(UnityAction onUse);
        public virtual void SetPosition(Vector3 pos)
        {
            transform.position = pos;
        }
    }
}