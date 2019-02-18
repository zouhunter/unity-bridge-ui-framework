using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BridgeUI;
using BridgeUI.Binding;
using BridgeUI.Events;
public class ImguiEditorTestWindow : EditorWindow {
    private UIPanelBinder Binder;
    public const byte title_binding = 1;
    public const byte label_binding = 2;
    public const byte on_button_click_binding = 3;

    private string m_title;
    private string label;
    private IntEvent clickEvent = new IntEvent();
    private string lastText;

    public IViewModel ViewModel { get; set; }

    [MenuItem("Test/TestImgui")]
    private static void OpenImguiEditorTestWindow()
    {
        GetWindow<ImguiEditorTestWindow>("IMGUIBINDING");
    }

    private void OnEnable()
    {
        //Binder = new UIPanelBinder(this);
        //ViewModel = ScriptableObject.CreateInstance<VMIMGUIPanel_ViewModel1>();

        PropBindings();
        Binder.Bind(ViewModel);
    }

    protected void PropBindings()
    {
        Binder.RegistValueChange<string>(x => m_title = x, title_binding);
        Binder.RegistValueChange<string>(x => label = x, label_binding);
        Binder.RegistEvent(clickEvent, on_button_click_binding);
    }

    private void OnGUI()
    {
        GUILayout.Label(m_title);
        label = GUILayout.TextField(label);
        if (lastText != label)
        {
            lastText = label;
            Binder.SetValue(label, label_binding);
        }
        if (GUILayout.Button("click!"))
        {
            clickEvent.Invoke(Random.Range(0, 1000));
        }
    }

    public void OnViewModelChanged(IViewModel newValue)
    {
        Debug.Log(newValue);
    }
}
