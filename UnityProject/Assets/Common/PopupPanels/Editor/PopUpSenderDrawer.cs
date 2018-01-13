using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(PopUpSender))]
public class PopUpSenderDrawer : Editor
{
    private const string guid = "642c7f4e1966a1446a29f67498cd94eb";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Select",EditorStyles.toolbarButton))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Assert(!string.IsNullOrEmpty((path)));
            var obj = AssetDatabase.LoadAssetAtPath<PopUpObj>(path);
            Debug.Assert(obj != null);
            EditorGUIUtility.PingObject(obj);
        }
    }
}
