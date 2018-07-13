using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using NodeGraph;

namespace BridgeUI
{
    [CustomNodeView(typeof(BridgeConnection))]
    public class BridgeConnectionView:ConnectionView
    {
        private BridgeConnection connecton;
        public static BridgeConnection copyed;

        private static GUIContent _endContent;
        private static GUIContent endContent
        {
            get
            {
                if (_endContent == null)
                {
#if UNITY_5_6
                    _endContent = EditorGUIUtility.IconContent("varpin tooltip@2x");
#elif UNITY_5_3_4
                    _endContent = EditorGUIUtility.IconContent("ChannelStripAttenuationMarker3");
#else
                    _endContent = EditorGUIUtility.IconContent("WarningMessageOverlay");
#endif
                }
                return _endContent;
            }
        }
        private static Vector2 contentPos = new Vector2(-endContent.image.width, -endContent.image.height) * 0.5f;
        protected string Label
        {
            get
            {
                if (connecton == null && target != null)
                    connecton = target as BridgeConnection;
                var str = connecton != null ? string.Format("{0} {1}", connecton.index, BridgeEditorUtility.ShowModelToString(connecton.show)) : "";
                return str;
            }
        }
        internal override Color LineColor
        {
            get { return Color.yellow; }
        }
        internal override void OnDrawLabel(Vector2 centerPos, string label)
        {
            base.OnDrawLabel(centerPos, Label);
        }
        internal override void OnConnectionGUI(Vector2 startV3, Vector2 endV3, Vector2 startTan, Vector2 endTan)
        {
            base.OnConnectionGUI(startV3, endV3, startTan, endTan);
            var quater = Quaternion.Euler(endTan);
            quater.SetLookRotation(startTan);
            Handles.Label(endV3 + contentPos, endContent);//
        }
        internal override void OnContextMenuGUI(GenericMenu menu, ConnectionGUI connectionGUI)
        {
            base.OnContextMenuGUI(menu, connectionGUI);
            menu.AddItem(
                      new GUIContent("Copy"),
                      false,
                      () =>
                      {
                          copyed = connecton;
                      }
                  );
            menu.AddItem(
                    new GUIContent("Paste"),
                    false,
                    () =>
                    {
                        if (copyed != null)
                        {
                            connecton.show = copyed.show;
                        }
                    }
                );
        }
    }

}
