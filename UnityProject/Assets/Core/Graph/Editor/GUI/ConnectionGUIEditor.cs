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

            DrawToggleFromShowModel(con, ShowMode.Auto,"自动打开");
            DrawToggleFromShowModel(con, ShowMode.Mutex,"同级互斥");
            DrawToggleFromShowModel(con, ShowMode.Cover, "界面遮罩");
            DrawToggleFromShowModel(con, ShowMode.HideBase,"隐藏父级");
            DrawToggleFromShowModel(con, ShowMode.Single, "独立显示");
        }
        private void DrawToggleFromShowModel(ConnectionGUI con,ShowMode model,string tip)
        {
            var on = (con.Data._show & model) == model;
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                var thison = GUILayout.Toggle(on, model.ToString(), EditorStyles.radioButton, GUILayout.Height(60),GUILayout.Width(100));
                EditorGUILayout.LabelField(tip);
                if (thison != on)
                {
                    on = thison;
                    if (on)
                    {
                        con.Data._show |= model;
                    }
                    else
                    {
                        con.Data._show &= ~model;
                    }
                }
            }
           
        }
    }
}