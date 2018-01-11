using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using NodeGraph;

namespace BridgeUI
{
    [CustomNodeGraphDrawer(typeof(BridgeConnection))]
    public class BridgeConnectionDrawer : ConnectionDrawer
    {
        private BridgeConnection connecton;
        public static BridgeConnection copyed;
        protected string Label
        {
            get
            {
                if (connecton == null && target != null) connecton = target as BridgeConnection;
                var str = connecton != null ? string.Format("{0} {1}", connecton.index, Utility.ShowModelToString(connecton.show)) : "";
                return str;
            }
        }
        internal override Color LineColor
        {
            get { return Color.yellow; }
        }
        internal override void OnDrawLabel(Vector3 centerPos, string label)
        {
            base.OnDrawLabel(centerPos, Label);
        }
        internal override void OnInspectorGUI()
        {
            connecton = target as BridgeConnection;
            DrawIndex();
            connecton.show.auto = DrawToggle(connecton.show.auto, "自动打开");
            GUILayout.Space(10);
            connecton.show.mutex = (MutexRule)DrawEnum(connecton.show.mutex, "同级互斥");
            GUILayout.Space(10);
            connecton.show.cover = DrawToggle(connecton.show.cover, "界面遮罩");
            GUILayout.Space(10);
            connecton.show.baseShow = (BaseShow)DrawEnum(connecton.show.baseShow, "上级状态");
            GUILayout.Space(10);
            connecton.show.single = DrawToggle(connecton.show.single, "独立显示");
        }
        private void DrawIndex()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Index");
                connecton.index = EditorGUILayout.IntField(connecton.index);
            }
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
        private bool DrawToggle(bool on, string tip)
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(tip, GUILayout.Width(100));
                on = EditorGUILayout.Toggle(on, EditorStyles.radioButton);
            }
            return on;
        }

        private Enum DrawEnum(Enum em, string tip)
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(tip, GUILayout.Width(100));
                em = EditorGUILayout.EnumPopup(em);
            }
            return em;
        }
    }

}
