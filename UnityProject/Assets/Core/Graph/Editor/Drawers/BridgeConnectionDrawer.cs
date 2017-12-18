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
        internal override string Label
        {
            get {
                var str = BridgeUI.Utility.ShowModelToString((target as BridgeConnection).show);
                return str;
            }
        }
        internal override Color LineColor
        {
            get { return Color.yellow; }
        }
        internal override void OnInspectorGUI()
        {
            connecton = target as BridgeConnection;
            var showMode = connecton.show;

            showMode.auto = DrawToggle(showMode.auto, "自动打开");
            showMode.mutex = (MutexRule)DrawEnum(showMode.mutex, "同级互斥");
            showMode.cover = DrawToggle(showMode.cover, "界面遮罩");
            showMode.baseShow = (BaseShow)DrawEnum(showMode.baseShow, "父级演示");
            showMode.single = DrawToggle(showMode.single, "独立显示");

        }
        private bool DrawToggle(bool on, string tip)
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                on = GUILayout.Toggle(on, tip, EditorStyles.radioButton, GUILayout.Height(60), GUILayout.Width(100));
                EditorGUILayout.LabelField(tip);
            }
            return on;
        }

        private Enum DrawEnum(Enum em, string tip)
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                em = EditorGUILayout.EnumPopup(em, EditorStyles.toolbarDropDown, GUILayout.Height(60), GUILayout.Width(100));
                EditorGUILayout.LabelField(tip);
            }
            return em;
        }
    }

}
