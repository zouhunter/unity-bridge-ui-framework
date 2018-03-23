
using System.Reflection;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BridgeUI
{
    /// <summary>
    /// 在属性上添加本Attribute,
    /// 在传递字典类参数时，可自动匹配填充数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false,Inherited =true)]
    public class Charge : Attribute
    {
        public object key;
        public Charge(object key)
        {
            this.key = key;
        }
        public Charge()
        {
        }
    }
}