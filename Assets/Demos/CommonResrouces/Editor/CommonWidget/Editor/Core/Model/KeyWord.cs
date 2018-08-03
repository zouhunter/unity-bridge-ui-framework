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
using System.Collections.Generic;
namespace CommonWidget
{
    public static class KeyWord
    {
        public static string name { get; private set; }
        public static string type { get; private set; }
        public static string normal { get; private set; }
        public static string highlighted { get; private set; }
        public static string pressed { get; private set; }
        public static string disabled { get; private set; }
        public static string background { get; private set; }
        public static string mask { get; private set; }
        public static string sprite { get; internal set; }
        public static string image { get; internal set; }
        public static string fill { get; internal set; }
        public static string handle { get; internal set; }

        static KeyWord()
        {
            var keys = typeof(KeyWord).GetProperties(System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach (var item in keys)
            {
                item.SetValue(null, item.Name, null);
            }
        }

    }
}