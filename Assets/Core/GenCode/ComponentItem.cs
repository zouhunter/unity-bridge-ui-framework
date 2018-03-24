using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using Object = UnityEngine.Object;

namespace BridgeUI.Model
{
    [System.Serializable]
    public class ComponentItem
    {
        public string name;
        public int componentID;
        public TypeRecod[] components;
        public GameObject target;

        public string[] componentStrs { get { return System.Array.ConvertAll<TypeRecod, string>(components, x => x.typeName); } }
        public System.Type componentType
        {
            get
            {
                var type = typeof(GameObject);
                if (components != null && components.Length > componentID)
                {
                    type = components[componentID].type;
                }
                return type;
            }
        }
        public ComponentItem() { }
        public ComponentItem(GameObject target)
        {
            this.name = target.name;
            this.target = target;
        }
    }

    [System.Serializable]
    public struct TypeRecod
    {
        public System.Reflection.Assembly assemble { get { return System.Reflection.Assembly.Load(assembleName); } }
        public System.Type type { get { return assemble.GetType(typeName); } }

        public string assembleName;
        public string typeName;
        public TypeRecod(System.Type type)
        {
            this.assembleName = type.Assembly.ToString();
            this.typeName = type.FullName;
        }
    }
}