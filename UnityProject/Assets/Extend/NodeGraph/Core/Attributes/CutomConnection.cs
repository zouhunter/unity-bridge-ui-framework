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
using System;
/// <summary>
/// Custom node attribute for custom nodes.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CustomConnection : Attribute
{
    private string m_name;
    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name
    {
        get
        {
            return m_name;
        }
    }


    public CustomConnection(string name)
    {
        m_name = name;
    }
}
