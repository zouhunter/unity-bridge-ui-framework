using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BridgeUI.Control
{
    public class DoubleClickButton : Button 
    {
        [Serializable]
        public class DoubleClickedEvent : UnityEvent {}

        [SerializeField]
        private DoubleClickedEvent m_onDoubleClick = new DoubleClickedEvent();
        public DoubleClickedEvent onDoubleClick
        {
            get { return m_onDoubleClick; }
            set { m_onDoubleClick = value; }
        }

        [SerializeField]
        private float time = 0.5f;
        private float timer;

        private void Press()
        {
            if (null != onDoubleClick )
                onDoubleClick.Invoke();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if(Time.time - timer < time)
            {
                Press();
                timer = 0;
            }
            else
            {
                timer = Time.time;
            }
        }
       
    }
}