using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Model = NodeGraph.DataModel.Version2;

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

            DrawToggleFromShowModel(con, ShowModel.Auto);
            DrawToggleFromShowModel(con, ShowModel.Mutex);
            DrawToggleFromShowModel(con, ShowModel.Cover);
            DrawToggleFromShowModel(con, ShowModel.HideBase);
            DrawToggleFromShowModel(con, ShowModel.Single);
        }
        private void DrawToggleFromShowModel(ConnectionGUI con,ShowModel model)
        {
            var on = (con.Data._show & model) == model;
            GUIStyle option = on ? EditorStyles.toolbarButton : EditorStyles.toolbarDropDown;
            if (GUILayout.Button(model.ToString(), option))
            {
                on = !on;
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