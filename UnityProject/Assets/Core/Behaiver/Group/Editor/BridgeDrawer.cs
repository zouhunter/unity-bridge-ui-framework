using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
[CustomPropertyDrawer(typeof(Bridge))]
public class BridgeDrawer : PropertyDrawer {
    SerializedProperty inNodeProp;
    SerializedProperty outNodeProp;
    SerializedProperty showModelProp;
    private string inNodeName;
    string showKey = null;
       
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        inNodeProp = property.FindPropertyRelative("inNode");
        outNodeProp = property.FindPropertyRelative("outNode");
        showModelProp = property.FindPropertyRelative("showModel");
        inNodeName = inNodeProp.stringValue;
        if(string.IsNullOrEmpty(inNodeName)){
            inNodeName = "[Any]";
        }

        ShowModel show = (ShowModel)showModelProp.intValue;
        showKey = Utility.ShowModelToString(show);
        var rect_L = new Rect(position.x, position.y, position.width * 0.3f, position.height);
        EditorGUI.LabelField(rect_L, inNodeName);
        var rect_C = new Rect(position.x + position.width * 0.4f, position.y, position.width * 0.3f, position.height);
        EditorGUI.LabelField(rect_C, showKey);
        var rect_R = new Rect(position.x + position.width * 0.7f, position.y, position.width * 0.3f, position.height);
        EditorGUI.LabelField(rect_R, outNodeProp.stringValue);
    }
}
