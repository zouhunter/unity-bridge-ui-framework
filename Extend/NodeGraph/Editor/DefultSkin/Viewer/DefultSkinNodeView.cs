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
using NodeGraph;
using NodeGraph.DataModel;
using BridgeUI;
using UnityEditor;


namespace NodeGraph.DefultSkin
{
    public abstract class DefultSkinNodeView : NodeView
    {
        public virtual int Style { get { return 0; } }
        private GUISkin _skin;
        protected GUISkin skin
        {
            get
            {
                if (_skin == null)
                {
                    var path = AssetDatabase.GUIDToAssetPath("75ce4a2b9ce8e45f9bcb12d38ed95952");
                    _skin = AssetDatabase.LoadAssetAtPath<GUISkin>(path);
                    Debug.Assert(_skin != null, "the guid of the skin is changed!");
                }
                return _skin;
            }
        }

        public override GUIStyle ActiveStyle
        {
            get
            {
               return skin.FindStyle(string.Format("node {0} on", Style));
            }
        }
        public override GUIStyle InactiveStyle
        {
            get
            {
                return skin.FindStyle(string.Format("node {0}", Style));
            }
        }
    }
}