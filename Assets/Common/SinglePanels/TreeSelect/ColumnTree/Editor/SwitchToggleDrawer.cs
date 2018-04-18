using UnityEngine;
using UnityEditor;
namespace BridgeUI.Common.Tree
{

    #region UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SwitchToggle))]
    public class SwitchToggleDrawer : UnityEditor.Editor
    {
        private UnityEditor.SerializedProperty labelProp;
        private UnityEditor.SerializedProperty off_Lable_Color;
        private UnityEditor.SerializedProperty on_Lable_Color;
        private Editor defultDrawer;
        void OnEnable()
        {
            labelProp = serializedObject.FindProperty("label");
            on_Lable_Color = serializedObject.FindProperty("on_Lable_Color");
            off_Lable_Color = serializedObject.FindProperty("off_Lable_Color");
        }
        public override void OnInspectorGUI()
        {
            UnityEditor.EditorGUILayout.PropertyField(labelProp);
            if(labelProp.objectReferenceValue != null)
            {
                UnityEditor.EditorGUILayout.PropertyField(on_Lable_Color);
                UnityEditor.EditorGUILayout.PropertyField(off_Lable_Color);

            }
        }
    }
    #endregion

}