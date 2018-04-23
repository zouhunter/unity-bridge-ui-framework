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


//namespace BridgeUI.Binding
//{
    public delegate void RoutedEventHandler(object sender, RoutedEventArgs e);
    public class EventArgs
    {
        public static readonly EventArgs Empty;
        public EventArgs() { }
    }
    public class RoutedEventArgs : EventArgs
    {
        public bool Handled { get; set; }
        public object Source { get; set; }
        public object OriginalSource { get; set; }

        public RoutedEventArgs(object originalSource)
        {
            this.OriginalSource = originalSource;
        }
        public RoutedEventArgs(object originalSource, object source)
        {
            this.Source = source;
            this.OriginalSource = originalSource;
        }
    }
//}
