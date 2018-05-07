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
using System;

namespace BridgeUI.Control.Tree
{

    public class ColumnItem : MonoBehaviour
    {
        [SerializeField]
        private Toggle m_toggle;
        [SerializeField]
        private Text m_text;
        public Toggle Tog { get { return m_toggle; } }
        private UnityAction<bool> onValueChanged { get; set; }
        void Awake()
        {
            m_toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
      
        internal void Init(string v, ToggleGroup group, UnityAction<bool> onValueChanged)
        {
            m_text.text = v;
            m_toggle.group = group;
            this.onValueChanged = onValueChanged;
        }
        internal void Reset()
        {
            m_text.text = "";
            m_toggle.group = null;
            onValueChanged = null;
        }

        private void OnToggleValueChanged(bool isOn)
        {
            if (onValueChanged != null)
                onValueChanged.Invoke(isOn);
        }
    }

}