
using System.Reflection;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BridgeUI
{
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
    public class Charge : Attribute
    {
        public string key;
        public Charge(string key)
        {
            this.key = key;
        }
        public Charge()
        {
        }
    }
}