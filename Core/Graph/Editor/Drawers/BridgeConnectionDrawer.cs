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
using BridgeUI;
using System;

[CustomEditor(typeof(BridgeConnection))]
public class BridgeConnectionDrawer : Editor
{
    private BridgeConnection connecton;

    public override void OnInspectorGUI()
    {
        connecton = target as BridgeConnection;
        DrawIndex();
        connecton.show.auto = DrawToggle(connecton.show.auto, "自动打开");
        GUILayout.Space(10);
        connecton.show.mutex = (MutexRule)DrawEnum(connecton.show.mutex, "同级互斥");
        GUILayout.Space(10);
        connecton.show.cover = DrawToggle(connecton.show.cover, "界面遮罩");
        GUILayout.Space(10);
        connecton.show.baseShow = (BaseShow)DrawEnum(connecton.show.baseShow, "上级状态");
        GUILayout.Space(10);
        connecton.show.single = DrawToggle(connecton.show.single, "独立显示");
    }

    private void DrawIndex()
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Index");
            connecton.index = EditorGUILayout.IntField(connecton.index);
        }
    }

    private bool DrawToggle(bool on, string tip)
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField(tip, GUILayout.Width(100));
            on = EditorGUILayout.Toggle(on, EditorStyles.radioButton);
        }
        return on;
    }

    private Enum DrawEnum(Enum em, string tip)
    {
        using (var hor = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField(tip, GUILayout.Width(100));
            em = EditorGUILayout.EnumPopup(em);
        }
        return em;
    }
}