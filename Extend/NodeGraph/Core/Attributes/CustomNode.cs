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
using System;
namespace NodeGraph {
    /// <summary>
    /// Custom node attribute for custom nodes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomNode : Attribute
    {
        private string m_name;
        private string m_group;
        private int m_orderPriority;

        public static readonly int kDEFAULT_PRIORITY = 1000;
        public static readonly string kDEFAULT_GROUP = "Empty";
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
        public string Group
        {
            get
            {
                return m_group;
            }
        }

        /// <summary>
        /// Gets the order priority.
        /// </summary>
        /// <value>The order priority.</value>
        public int OrderPriority
        {
            get
            {
                return m_orderPriority;
            }
        }

        public CustomNode(string name)
        {
            m_name = name;
            m_orderPriority = kDEFAULT_PRIORITY;
            m_group = kDEFAULT_GROUP;
        }

        public CustomNode(string name, int orderPriority)
        {
            m_name = name;
            m_orderPriority = orderPriority;
            m_group = kDEFAULT_GROUP;
        }
        public CustomNode(string name, int orderPriority, string group)
        {
            m_name = name;
            m_orderPriority = orderPriority;
            m_group = group;
        }
    } }
