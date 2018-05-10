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
        DrawIndex("上一级面板打开本面板的唯一id");
        DrawHead("自动打开");
        connecton.show.auto = DrawToggle(connecton.show.auto,"更随上级同步打开");
        DrawHead("界面遮罩");
        connecton.show.cover = DrawToggle(connecton.show.cover, "阻止触发在此面板下的UI事件");
        DrawHead("独立显示");
        connecton.show.single = DrawToggle(connecton.show.single,"只显示当前界面（关闭其他）");
        DrawHead("界面互斥");
        DrawMutexRules();
        DrawHead("父级变化");
        DrawBaseShow();
    }

    private void DrawIndex(string tip)
    {
        var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight * 1.5f);
        GUI.color = Color.green;
        GUI.Box(rect, "", EditorStyles.miniButton);
        GUI.color = Color.white;
        EditorGUI.LabelField(rect, string.Format("【{0}】", connecton.name), EditorStyles.largeLabel);
        DrawHead("界面索引");
        using (var hor = new EditorGUILayout.VerticalScope())
        {
            connecton.index =(int) EditorGUILayout.Slider(connecton.index, 0, 100);
            EditorGUILayout.SelectableLabel("   --" + tip);
        }
    }

    private void DrawHead(string label)
    {
        EditorGUILayout.LabelField("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
        var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight * 1.1f);
        GUI.color = Color.gray;
        GUI.Box(rect, "", EditorStyles.miniButton);
        GUI.color = Color.white;
        EditorGUI.LabelField(rect, string.Format("【{0}】", label), EditorStyles.largeLabel);
    }

    string[] mutexRules;
    string[] mutexRulesNotice = { "不排斥", "排斥同父级中的同层级", "排斥同层级" };
    int mutexRulesSelected;
    private void DrawMutexRules()
    {
        if (mutexRules == null)
        {
            mutexRules = System.Enum.GetNames(typeof(MutexRule));
        }

        mutexRulesSelected = System.Array.IndexOf(mutexRules, connecton.show.mutex.ToString());

        for (int i = 0; i < mutexRules.Length; i++)
        {
            var isOn = EditorGUILayout.ToggleLeft(string.Format("{0}--{1}", mutexRules[i], mutexRulesNotice[i]), mutexRulesSelected == i);
            if (isOn)
            {
                connecton.show.mutex = (MutexRule)i;
            }
        }
    }


    string[] baseShows;
    string[] baseShowsNotice = { "不改变父级状态", "隐藏父级(在本面板关闭时打开)", "销毁父级(接管因为父级面关闭的面板)" };
    int baseShowsSelected;
    private void DrawBaseShow()
    {
        if (baseShows == null)
        {
            baseShows = System.Enum.GetNames(typeof(BaseShow));
        }

        baseShowsSelected = System.Array.IndexOf(baseShows, connecton.show.baseShow.ToString());

        for (int i = 0; i < mutexRules.Length; i++)
        {
            var isOn = EditorGUILayout.ToggleLeft(string.Format("{0} --{1}", baseShows[i], baseShowsNotice[i]), baseShowsSelected == i);
            if (isOn)
            {
                connecton.show.baseShow = (BaseShow)i;
            }
        }
    }

    private bool DrawToggle(bool on,string tip)
    {
        using (var hor = new EditorGUILayout.HorizontalScope()){
            on = EditorGUILayout.Toggle(on, EditorStyles.radioButton,GUILayout.Width(20));
            EditorGUILayout.SelectableLabel(" --" + tip);
        }
        return on;
    }

}