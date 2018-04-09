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

namespace BridgeUI.Binding
{

    public class PanelBaseBinder : PropertyBinder
    {
        public PanelBaseBinder(PanelBase panel) : base(panel) { }
        public void AddText(Text text, string name)
        {
            AddToModel<string>(name, (value) => { text.text = value; });
        }
        public void AddButton(Button button, string methodName)
        {
            UnityAction action = () => { Invoke(methodName, button, new RoutedEventArgs(Context)); };

            binders += viewModel =>
            {
                button.onClick.AddListener(action);
            };

            unbinders += viewModel =>
            {
                button.onClick.RemoveListener(action);
            };
        }
    }

}