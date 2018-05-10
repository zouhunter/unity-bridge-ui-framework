using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using BridgeUI;

namespace BridgeUI.Common
{
    public class TextInfoBehiaver : SinglePanel
    {
        public InputField.SubmitEvent onGet;
        [SerializeField]
        private TextBehaiver textBehaiver;
        private GameObject textBody;
        private List<RaycastResult> result = new List<RaycastResult>();
        private PointerEventData eventData;
        private float timer = 3;
        private string timerName;
        private InfoTextShow lastSelected;
        private bool show = false;

        private Color? fontColor;
        private Vector3 position;
        private string textInfo;
        private int? fontSize;

        protected override void Start()
        {
            base.Start();
            textBody = textBehaiver.gameObject;
            eventData = new PointerEventData(EventSystem.current);
            ShowText();
        }
        protected override void HandleData(object data)
        {
            base.HandleData(data);
            var dic = data as Hashtable;
            if (dic.ContainsKey("fontSize"))
            {
                fontSize = (int)dic["fontSize"];
            }
            if (dic.ContainsKey("position"))
            {
                position = (Vector3)dic["position"];
            }

            if (dic.ContainsKey("textInfo"))
            {
                textInfo = dic["textInfo"] as string;
            }

            if (dic.ContainsKey("fontColor"))
            {
                fontColor = (Color)dic["fontColor"];
            }
        }

        public void Update()
        {
            eventData.position = Input.mousePosition;
            show = false;
            EventSystem.current.RaycastAll(eventData, result);
            for (int i = 0; i < result.Count; i++)
            {
                InfoTextShow target = result[i].gameObject.GetComponent<InfoTextShow>();
                if (target != null)
                {
                    show = true;
                    if (target != lastSelected)
                    {
                        lastSelected = target;
                        ShowText();
                    }
                    break;
                }
            }
            if (!show || (timer -= Time.deltaTime) < 0)
            {
                Destroy(gameObject);
            }
        }


        private void ShowText()
        {
            if (!string.IsNullOrEmpty(textInfo))
            {
                show = true;
                textBody.SetActive(true);
                textBehaiver.transform.position = position;
                if (fontSize != null) textBehaiver.text.fontSize = fontSize ?? 0;//?? textBehaiver.text.fontSize;
                if (fontColor != null) textBehaiver.text.color = fontColor ?? Color.clear;// ?? textBehaiver.text.color;
                textBehaiver.text.text = textInfo;
                onGet.Invoke(textBehaiver.text.text);
            }
        }
    }
}