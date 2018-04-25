using UnityEngine;
using System.Collections.Generic;
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

namespace BridgeUI.CodeGen
{
    [System.Serializable]
    public class ComponentItem
    {
        public bool open;
        public string name;
        public int componentID;
        public TypeInfo[] components;
        public List<BindingShow> viewItems = new List<BindingShow>();
        public List<BindingEvent> eventItems = new List<BindingEvent>();
        public GameObject target;

        public string[] componentStrs { get { return System.Array.ConvertAll<TypeInfo, string>(components, x => x.typeName); } }
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

  
}