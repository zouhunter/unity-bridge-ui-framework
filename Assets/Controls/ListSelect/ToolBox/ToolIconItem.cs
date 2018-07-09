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
        private Image m_image;
        private RawImage m_rawImage;
        private Text m_title;

        public UnityAction<string> onDrag { get; set; }
        public UnityAction<string> onPointDown { get; set; }
        public UnityAction<string> onPointUp { get; set; }

        private void Awake()
        {
            m_title = GetComponentInChildren<Text>();
            m_rawImage = GetComponentsInChildren<RawImage>().Where(x => x.gameObject != gameObject).FirstOrDefault();
            m_image = GetComponentsInChildren<Image>().Where(x => x.gameObject != gameObject).FirstOrDefault();
        }

        internal void Init(string title, Sprite sprite,Texture texture)
        {
             m_title.text = title;
            if (m_image) m_image.sprite = sprite;
            if (m_rawImage) m_rawImage.texture = texture;
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDrag.Invoke(m_title.text);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointDown.Invoke(m_title.text);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointUp.Invoke(m_title.text);
        }


    }

}