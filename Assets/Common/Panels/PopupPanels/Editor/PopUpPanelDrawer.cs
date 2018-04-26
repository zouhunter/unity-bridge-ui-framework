using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
namespace BridgeUI.Common
{
    [CustomPropertyDrawer(typeof(PopupPanel.PopData))]
    public class PopUpObjPopDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 4 * EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typePorp = property.FindPropertyRelative("typeName");
            var titlePorp = property.FindPropertyRelative("title");
            var infoPorp = property.FindPropertyRelative("info");
            var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            GUI.color = Color.green;
            EditorGUI.SelectableLabel(rect, typePorp.stringValue, EditorStyles.boldLabel);
            GUI.color = Color.white;
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, titlePorp);

            rect.y += EditorGUIUtility.singleLineHeight;
            rect.height += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, infoPorp);

        }
    }
    [CustomEditor(typeof(PopupPanel))]
    public class PopUpPanelDrawer : Editor
    {
        SerializedProperty popDatasProp;
        ReorderableList reorderList;
        SerializedProperty popEnumProp;
        private void OnEnable()
        {
            popEnumProp = serializedObject.FindProperty("popEnum");
            popDatasProp = serializedObject.FindProperty("popDatas");
            InitListDrawer();
        }

        private void UpdatePopUpData()
        {
            var popDatas = (target as PopupPanel).popDatas;
            var popEnum = (target as PopupPanel).popEnum;
            if (popEnum == null) return;
            var enumType = popEnum.GetClass();
            if (!enumType.IsEnum)
            {
                Debug.LogError("请放置枚举类型！");
                return;
            }
            var types = System.Enum.GetNames(enumType);

            var activeList = new List<PopupPanel.PopData>();
            foreach (var element in types)
            {
                var intValue = (int)System.Convert.ChangeType(System.Enum.Parse(enumType, element), typeof(int));
                var pitem = popDatas.Find(x => x.typeInt == intValue);
                if (pitem == null)
                {
                    pitem = new PopupPanel.PopData(intValue, element);
                    popDatas.Add(pitem);
                }
                else
                {
                    pitem.typeName = element;
                }
                activeList.Add(pitem);
            }
            popDatas.RemoveAll(x => !activeList.Contains(x));
            (target as PopupPanel).popDatas = popDatas;
            EditorUtility.SetDirty(target);
        }

        private void InitListDrawer()
        {
            reorderList = new ReorderableList(serializedObject, popDatasProp, true, true, false, false);
            reorderList.elementHeight = 4 * EditorGUIUtility.singleLineHeight;
            reorderList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "枚举信息列表");
            };
            reorderList.drawElementCallback = (rect, index, actived, focused) =>
            {
                var prop = popDatasProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, prop);
            };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(popEnumProp);
            if (EditorGUI.EndChangeCheck())
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    UpdatePopUpData();
                };
            }
            reorderList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

    }
}