using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SpringGUI
{
    public class LongPressButton : Button
    {
        [Serializable]
        public class LongPressEvent : UnityEvent {}

        [SerializeField]
        private LongPressEvent m_onLongPress = null;
        public LongPressEvent onLongPress
        {
            get { return m_onLongPress; }
            set { m_onLongPress = value; }
        }
        [SerializeField]
        private float time = 1;
        private float timer;
        private bool pressed;

        private void Press()
        {
            if( null != onLongPress)
                onLongPress.Invoke();
        }

        private void Update()
        {
            if (!pressed) return;

            if(Time.time - timer > time)
            {
                Press();
                pressed = false;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            timer = Time.time;
            pressed = true;
        }
    }
}