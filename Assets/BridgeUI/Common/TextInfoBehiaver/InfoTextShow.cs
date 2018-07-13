using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
namespace BridgeUI.Common
{
    public class InfoTextShow : MonoBehaviour, IPointerEnterHandler
    {
        public int fontSize = 10;
        public bool specialSize = false;
        public Color fontColor = Color.white;
        public bool specialColor = false;
        public string textshow;
        public Vector2 showPos;
        private bool isWorld;
        private Hashtable sender;
        private float plusOfScreen;
        public void Start()
        {
            if (string.IsNullOrEmpty(textshow))
            {
                textshow = name;
            }
            isWorld = !GetComponent<RectTransform>();
            plusOfScreen = Screen.width / 1600f;
            OnPointerEnter(null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Vector3 position = Vector3.zero;

            if (!isWorld)
            {
                position += transform.position;
            }
            else
            {
                position += Camera.main.WorldToScreenPoint(transform.position);
            }

            position.x += showPos.x * plusOfScreen;
            position.y += showPos.y * plusOfScreen;

            if (sender == null) sender = new Hashtable();

            sender["position"] = position;
            sender["textInfo"] = textshow;

            if (specialColor) sender["fontColor"] = fontColor;
            if (specialSize) sender["fontSize"] = fontSize;
            UIFacade.Instence.Open("TextInfoBehiaver", sender);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            UIFacade.Instence.Close("TextInfoBehiaver");
        }
    }
}