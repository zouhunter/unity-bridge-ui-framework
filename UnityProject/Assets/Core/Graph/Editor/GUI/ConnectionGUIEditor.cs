using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Model = NodeGraph.DataModel.Version2;
using BridgeUI;

namespace NodeGraph
{
    [CustomEditor(typeof(ConnectionGUIInspectorHelper))]
    public class ConnectionGUIEditor : Editor
    {

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            ConnectionGUIInspectorHelper helper = target as ConnectionGUIInspectorHelper;

            var con = helper.connectionGUI;

            if (con == null)
            {
                return;
            }
            EditorGUILayout.HelpBox("界面切换规则:", MessageType.Info);

            con.Data._show.auto = DrawToggle(con.Data._show.auto, "自动打开");
            con.Data._show.mutex =(MutexRule) DrawEnum(con.Data._show.mutex,"同级互斥");
            con.Data._show.cover = DrawToggle( con.Data._show.cover, "界面遮罩");
            con.Data._show.baseShow = (BaseShow)DrawEnum(con.Data._show.baseShow, "父级演示");
            con.Data._show.single = DrawToggle( con.Data._show.single, "独立显示");
        }

        private bool DrawToggle(bool on, string tip)
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                 on = GUILayout.Toggle(on, tip, EditorStyles.radioButton, GUILayout.Height(60),GUILayout.Width(100));
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