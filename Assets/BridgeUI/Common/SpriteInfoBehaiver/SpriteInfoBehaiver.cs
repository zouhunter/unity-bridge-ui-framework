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
    public class SpriteInfoBehaiver : SinglePanel
    {
        public InputField.SubmitEvent onGet;
        [SerializeField]
        private SpriteInfoItem view;
        private GameObject textBody;
        private List<RaycastResult> result = new List<RaycastResult>();
        private PointerEventData eventData;
        private float timer = 3;
        private string timerName;
        private InfoSpriteShow lastSelected;
        private bool show = false;

        protected override void Start()
        {
            base.Start();
            textBody = view.gameObject;
            eventData = new PointerEventData(EventSystem.current);
            ShowText();
        }

        protected override void PropBindings()
        {
            base.PropBindings();
            Binder.RegistMember<int>(view.SetFontSize, "fontSize");
            Binder.RegistMember<Vector3>(SetPosition, "position");
            Binder.RegistMember<string>(view.SetTitle, "textInfo");
            Binder.RegistMember<Color>(view.SetColor, "fontColor");
            Binder.RegistMember<Sprite>(view.SetSprite, "sprite");
        }

        public void Update()
        {
            eventData.position = Input.mousePosition;
            show = false;
            EventSystem.current.RaycastAll(eventData, result);
            for (int i = 0; i < result.Count; i++)
            {
                InfoSpriteShow target = result[i].gameObject.GetComponent<InfoSpriteShow>();
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
            if (!string.IsNullOrEmpty(view.title.text))
            {
                show = true;
                textBody.SetActive(true);
                onGet.Invoke(view.title.text);
            }
        }

        private void SetPosition(Vector3 pos)
        {
            transform.position = pos;
        }
    }
}