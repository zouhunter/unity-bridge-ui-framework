using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BridgeUI.Binding;

namespace BridgeUI.Extend.XLua
{
    public class LuaPropertyBinder : PropertyBinder
    {
        public LuaPropertyBinder(IBindingContext context) : base(context)
        {
        }
        public override void InvokeEvent<T>(string sourceName, T arg0)
        {
            base.InvokeEvent<object>(sourceName, arg0);
        }
        public override void InvokeEvent<S, T>(string sourceName, S arg0, T arg1)
        {
            base.InvokeEvent<object, object>(sourceName, arg0, arg1);
        }
        public override void InvokeEvent<R, S, T>(string sourceName, R arg0, S arg1, T arg2)
        {
            base.InvokeEvent<object, object, object>(sourceName, arg0, arg1, arg2);
        }
        public override void InvokeEvent<Q, R, S, T>(string sourceName, Q arg0, R arg1, S arg2, T arg3)
        {
            base.InvokeEvent<object, object, object, object>(sourceName, arg0, arg1, arg2, arg3);
        }
        public override void InvokeEvent<P, Q, R, S, T>(string sourceName, P arg0, Q arg1, R arg2, S arg3, T arg4)
        {
            base.InvokeEvent<object, object, object, object, object>(sourceName, arg0, arg1, arg2, arg3, arg4);
        }
    }
}