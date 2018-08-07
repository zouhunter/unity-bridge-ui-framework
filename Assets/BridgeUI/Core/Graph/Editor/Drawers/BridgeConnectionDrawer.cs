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
    protected const float buttonWidth = 20;

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
        DrawHead("数据模型");
        DrawViewModel();
    }

 
    private void DrawIndex(string tip)
    {
        var position = GUILayoutUtility.GetRect(BridgeUIEditor.BridgeEditorUtility.currentViewWidth, EditorGUIUtility.singleLineHeight * 1.5f);
        GUI.color = Color.green;
        GUI.Box(position, "", EditorStyles.miniButton);
        GUI.color = Color.white;
        EditorGUI.LabelField(position, string.Format("【{0}】", connecton.name), EditorStyles.largeLabel);
        DrawHead("界面索引");

        position = GUILayoutUtility.GetRect(BridgeUIEditor.BridgeEditorUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
        var fieldRect = new Rect(position.x, position.y, position.width - 2 * buttonWidth, position.height);
        var btnRect = new Rect(position.x + position.width - 2 * buttonWidth, position.y, buttonWidth, position.height);

        connecton.index = EditorGUI.IntField(fieldRect, connecton.index);
        btnRect.x -= 1;

        if (GUI.Button(btnRect, "-", EditorStyles.miniButtonLeft))
        {
            connecton.index--;
        }
        btnRect.x += buttonWidth + 1;
        if (GUI.Button(btnRect, "+", EditorStyles.miniButtonRight))
        {
            connecton.index++;
        }
        if (connecton.index < 0)
        {
            connecton.index = 0;
        }
        if (connecton.index > 100)
        {
            connecton.index = 100;
        }
    }

    private void DrawHead(string label)
    {
        EditorGUILayout.LabelField("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
        var rect = GUILayoutUtility.GetRect(BridgeUIEditor.BridgeEditorUtility.currentViewWidth, EditorGUIUtility.singleLineHeight * 1.1f);
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
                mutexRulesSelected = i;
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
                baseShowsSelected = i;
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

    private void DrawViewModel()
    {
        connecton.viewModel = EditorGUILayout.ObjectField(connecton.viewModel, typeof(BridgeUI.Binding.ViewModel), false) as BridgeUI.Binding.ViewModel;
    }

}