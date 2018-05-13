using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace BridgeUI.Control
{
    public class DeepSelectItem : MonoBehaviour
    {
        [SerializeField]
        private Text m_text;
        [SerializeField]
        private Button m_btn;
        private UnityAction<string> action { get; set; }
        private void Awake()
        {
            if(m_text ==null) m_text = gameObject.GetComponentInChildren<Text>();
            if (m_btn == null) m_btn = gameObject.GetComponentInChildren<Button>();
            if (m_btn)
            {
                m_btn.onClick.AddListener(OnClickedItem);
            }
        }
        private void OnClickedItem()
        {
            if (action != null)
            {
                action.Invoke(m_text.text);
            }
        }
        internal void Init(string v, UnityAction<string> onSelect)
        {
            if (v != null) m_text.text = v;
            this.action = onSelect;
        }
    }
}