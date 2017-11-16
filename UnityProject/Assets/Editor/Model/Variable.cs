using System.Reflection;
using UnityEngine;

namespace PrefabGenerate
{
    [System.Serializable]
    public class Variable
    {
        public string name;
        public string value;
        public string type;
        public bool isPrivate;
        public string assemble;
    }
}