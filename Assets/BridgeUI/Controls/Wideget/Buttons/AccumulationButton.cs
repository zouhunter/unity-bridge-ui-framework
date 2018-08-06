using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BridgeUI.Control
{
    public class AccumulationButton : Button
    {
        [System. Serializable]
        public class AccumulationEvent : UnityEvent<float> { }

        [SerializeField]
        private AccumulationEvent m_LongPressDown = new AccumulationEvent();
        public AccumulationEvent LongPressDown
        {
            get { return m_LongPressDown; }
            set { m_LongPressDown = value; }
        }

        // 延迟时间  
        private float delay = 0.2f;

        // 按钮是否是按下状态  
        private bool isDown = false;

        // 按钮最后一次是被按住状态时候的时间  
        private float lastIsDownTime;
        
        private float _gasoline;


        void Update()
        {
            // 如果按钮是被按下状态  
            if (isDown)
            {
                // 当前时间 -  按钮最后一次被按下的时间 > 延迟时间0.2秒  
                if (Time.time - lastIsDownTime > delay)
                {
                    // 触发长按方法  
                    if (LongPressDown != null)
                    {
                        _gasoline += 0.1f;
                        LongPressDown.Invoke(_gasoline);
                    }
                    // 记录按钮最后一次被按下的时间  
                    lastIsDownTime = Time.time;

                }
            }
            else
            {
                if (_gasoline > 0)
                {
                    _gasoline -= 0.05f;
                    LongPressDown.Invoke(_gasoline);

                }
            }
        }

        // 当按钮被按下后系统自动调用此方法  
        public override void OnPointerDown(PointerEventData eventData)
        {
            isDown = true;
            lastIsDownTime = Time.time;
        }

        // 当按钮抬起的时候自动调用此方法  
        public override void OnPointerUp(PointerEventData eventData)
        {
            isDown = false;
        }

        // 当鼠标从按钮上离开的时候自动调用此方法  
        public override void OnPointerExit(PointerEventData eventData)
        {
            isDown = false;
        }
    }
}