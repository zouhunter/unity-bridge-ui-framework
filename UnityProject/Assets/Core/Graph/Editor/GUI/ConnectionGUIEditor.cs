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

            DrawToggleFromShowModel(con, ShowModel.Auto,"父级界面的开关状态控制连接到的界面开关");
            DrawToggleFromShowModel(con, ShowModel.Mutex,"让拥有相同关键字的其他界面关闭");
            //DrawToggleFromShowModel(con, ShowModel.Cover, "建立遮罩防止点击其他对象");
            DrawToggleFromShowModel(con, ShowModel.HideBase,"连接到的界面控制将父级界面开关");
            DrawToggleFromShowModel(con, ShowModel.Single, "隐藏所有打开的面板");
        }
        private void DrawToggleFromShowModel(ConnectionGUI con,ShowModel model,string toolTip)
        {
            var on = (con.Data._show & model) == model;
            GUIStyle option = on ? EditorStyles.toolbarButton : EditorStyles.toolbarDropDown;
            if (GUILayout.Button(new GUIContent( model.ToString(),toolTip), option))
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