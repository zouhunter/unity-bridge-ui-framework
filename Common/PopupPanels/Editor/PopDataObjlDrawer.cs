using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
namespace BridgeUI.Common
{
    [CustomPropertyDrawer(typeof(PopDataObj.PopData))]
    public class PopUpObjPopDataDrawer : PropertyDrawer
    {
        private static Color bgColor = new Color(0.1f, 0.1f, 0.1f, 0.3f);
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 4 * EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var padding = 3;
            var innerRect = new Rect(position.x + padding, position.y + padding, position.width - 2 * padding, position.height - 2 * padding);
            GUI.color = bgColor;
            GUI.Box(innerRect,"");
            GUI.color = Color.white;

            var typePorp = property.FindPropertyRelative("typeName");
            var titlePorp = property.FindPropertyRelative("title");
            var infoPorp = property.FindPropertyRelative("info");
            var donthideProp = property.FindPropertyRelative("donthide");
            var titleRect = new Rect(innerRect.x, innerRect.y, innerRect.width - 90, EditorGUIUtility.singleLineHeight);
            GUI.color = Color.green;
            EditorGUI.SelectableLabel(titleRect, typePorp.stringValue, EditorStyles.boldLabel);
            GUI.color = Color.white;

            var rect = new Rect(innerRect.x, innerRect.y + EditorGUIUtility.singleLineHeight, innerRect.width, EditorGUIUtility.singleLineHeight);
            titlePorp.stringValue = EditorGUI.TextArea(rect, titlePorp.stringValue);
            rect.y += EditorGUIUtility.singleLineHeight;
            rect.height += 8;
            infoPorp.stringValue = EditorGUI.TextArea(rect, infoPorp.stringValue);

            var donthideRect = new Rect(innerRect.x + innerRect.width - 30, innerRect.y, 20, innerRect.height);
            var dontHideLabelRect = new Rect(innerRect.x + innerRect.width - 90, innerRect.y, 60, innerRect.height);
            EditorGUI.LabelField(dontHideLabelRect, "dontHide");
            donthideProp.boolValue = EditorGUI.Toggle(donthideRect,donthideProp.boolValue);
        }
    }
    [CustomEditor(typeof(PopDataObj))]
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
            var targetObj = target as PopDataObj;
            var popDatas = targetObj.popDatas;
            var popEnum = targetObj.popEnum;
            if (popEnum == null) return;
            var enumType = popEnum.GetClass();
            if (!enumType.IsEnum)
            {
                Debug.LogError("请放置枚举类型！");
                return;
            }
            targetObj.enumType = enumType.FullName;
            var types = System.Enum.GetNames(enumType);

            var activeList = new List<PopDataObj.PopData>();
            foreach (var element in types)
            {
                var intValue = (int)System.Convert.ChangeType(System.Enum.Parse(enumType, element), typeof(int));
                var pitem = popDatas.Find(x => x.typeInt == intValue);
                if (pitem == null)
                {
                    pitem = new PopDataObj.PopData(intValue, element);
                    popDatas.Add(pitem);
                }
                else
                {
                    pitem.typeName = element;
                }
                activeList.Add(pitem);
            }
            popDatas.RemoveAll(x => !activeList.Contains(x));
            (target as PopDataObj).popDatas = popDatas;
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