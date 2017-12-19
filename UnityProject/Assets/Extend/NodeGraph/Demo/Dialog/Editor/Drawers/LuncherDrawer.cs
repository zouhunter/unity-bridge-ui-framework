//using UnityEngine;
//using UnityEditor;

//using System;
//using System.Linq;
//using System.Collections.Generic;
//using NodeGraph.DataModel;
//using NodeGraph;

//[CustomNodeGraphDrawer(typeof(Luncher))]
//public class LuncherDrawer : NodeDrawer
//{
//    public override string ActiveStyle
//    {
//        get
//        {
//            return "node 0 on";
//        }
//    }

//    public override string InactiveStyle
//    {
//        get
//        {
//            return "node 0";
//        }
//    }

//    public override string Category
//    {
//        get
//        {
//            return "empty";
//        }
//    }
   
//    public override void OnInspectorGUI()
//    {
//        EditorGUILayout.HelpBox("Any Lunch: Lunch SubPanels From Any State", MessageType.Info);
//        base.OnInspectorGUI();
//    }
//    public override void OnContextMenuGUI(GenericMenu menu)
//    {
//        base.OnContextMenuGUI(menu);
//    }
//}