using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
namespace BridgeUI.Common
{
    [CustomPropertyDrawer(typeof(PopUpObj.PopData))]
    public class PopUpObjPopDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 4 * EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typePorp = property.FindPropertyRelative("type");
            var titlePorp = property.FindPropertyRelative("title");
            var infoPorp = property.FindPropertyRelative("info");
            var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            GUI.color = Color.green;
            EditorGUI.SelectableLabel(rect, ((PopUpType)typePorp.intValue).ToString(), EditorStyles.boldLabel);
            GUI.color = Color.white;
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, titlePorp);

            rect.y += EditorGUIUtility.singleLineHeight;
            rect.height += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, infoPorp);

        }
    }
    [CustomEditor(typeof(PopUpObj))]
    public class PopUpObjDrawer : Editor
    {
        SerializedProperty popDatasProp;
        SerializedProperty scriptProp;
        SerializedProperty popTypeBehaiverProp;
        ReorderableList reorderList;
        private void OnEnable()
        {
            popDatasProp = serializedObject.FindProperty("popDatas");
            scriptProp = serializedObject.FindProperty("m_Script");
            popTypeBehaiverProp = serializedObject.FindProperty("popTypeBehaiver");
            (target as PopUpObj).Reset();
            reorderList = new ReorderableList(serializedObject, popDatasProp, true, true, false, false);
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(scriptProp);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(popTypeBehaiverProp);


            //ReorderableListGUI.Title("信息列表");
            //ReorderableListGUI.ListField(popDatasProp, null, ReorderableListFlags.HideAddButton | ReorderableListFlags.HideRemoveButtons);
            reorderList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

    }
}