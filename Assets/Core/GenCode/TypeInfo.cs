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

namespace BridgeUI.CodeGen
{

    [System.Serializable]
    public struct TypeInfo
    {
        public System.Reflection.Assembly assemble { get { return System.Reflection.Assembly.Load(assembleName); } }
        public System.Type type { get { return assemble.GetType(typeName); } }

        public string assembleName;
        public string typeName;

        public TypeInfo(System.Type type)
        {
            this.assembleName = type.Assembly.ToString();
            this.typeName = type.FullName;
        }
        public void Update(System.Type type)
        {
            this.assembleName = type.Assembly.ToString();
            this.typeName = type.FullName;
        }
    }
}