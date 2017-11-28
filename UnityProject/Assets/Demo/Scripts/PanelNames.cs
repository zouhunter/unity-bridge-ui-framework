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
using System.Reflection;
public class PanelNames  {
    
    static PanelNames()
    {
        var type = typeof(PanelNames);
        var prop = type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Static);
        foreach (var item in prop)
        {
            item.SetValue(null, item.Name,new object[] { });
        }
    }
    public static string MainPanel { get; set; }
    public static string Panel01 { get; internal set; }
    public static string Panel02 { get; internal set; }
    public static string Panel03 { get; internal set; }
}
