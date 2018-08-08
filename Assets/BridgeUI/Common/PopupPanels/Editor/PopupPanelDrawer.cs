using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
using BridgeUI.Common;
using UnityEditor;

namespace BridgeUI.Drawer
{

    [CustomEditor(typeof(PopupPanel))]
    public class PopupPanelDrawer : Editor
    {
        private SerializedProperty popDatas_prop;
        private PopDataObjListDrawer listDrawer = new PopDataObjListDrawer("模块提示列表");

        private void OnEnable()
        {
            popDatas_prop = serializedObject.FindProperty("popDatas");
            listDrawer.InitReorderList(popDatas_prop);
        }
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();
            var literator = serializedObject.GetIterator();
            var enterChild = true;
            while (literator.NextVisible(enterChild))
            {
                if(literator.name == "popDatas")
                {
                    listDrawer.DoLayoutList();
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(literator.name == "m_Script");
                    EditorGUILayout.PropertyField(literator, true);
                    EditorGUI.EndDisabledGroup();
                    enterChild = false;
                }
            }
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}