using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace BridgeUI.Control
{
    public class ToolIconItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField]
        private RectTransform m_imageRect;
        [SerializeField]
        private Text m_title;
        private string title;
        public string itemName { get { return title; } }
        public UnityAction<string> onDrag { get; set; }
        public UnityAction<string> onPointDown { get; set; }
        public UnityAction<string> onPointUp { get; set; }

        public Graphic GraphicBehaiver
        {
            get
            {
                return m_imageRect.GetComponent<UnityEngine.UI.Graphic>();
            }
        }

        public void Init(string title, Sprite sprite, Texture texture)
        {
            this.title = title;
            if (m_title) m_title.text = title;

            if(sprite)
            {
                SwitchToGraphic<Image>().sprite = sprite;
            }
            else if (texture)
            {
                SwitchToGraphic<RawImage>().texture = texture;
            }
        }

        private T SwitchToGraphic<T>() where T:Graphic
        {
            var graphic = m_imageRect.GetComponent<Graphic>();

            if (graphic != null && !(graphic is T))
            {
                GameObject.Destroy(graphic);
            }

            if(graphic == null)
            {
                graphic = m_imageRect.gameObject.AddComponent<T>();
            }
            return graphic as T;
        }

        internal void Init(string title, Texture texture)
        {
            if (m_title) m_title.text = title;
            if (texture)
            {
                SwitchToGraphic<RawImage>().texture = texture;
            }
        }
        internal void Init(string title, Sprite sprite)
        {
            if (m_title) m_title.text = title;
            if (sprite)
            {
                SwitchToGraphic<Image>().sprite = sprite;
            }
        }


        public void OnDrag(PointerEventData eventData)
        {
            if (onDrag != null) onDrag.Invoke(title);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (onPointDown != null) onPointDown.Invoke(title);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (onPointUp != null) onPointUp.Invoke(title);
        }


    }

}