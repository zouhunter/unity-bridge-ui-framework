using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using BridgeUI;

namespace BridgeUI.Common
{
    public class SpriteInfoBehaiver : ViewBaseComponent
    {
        public InputField.SubmitEvent onGet;
        [SerializeField]
        private SpriteInfoItem spriteInfoItem;
        private GameObject textBody;
        private List<RaycastResult> result = new List<RaycastResult>();
        private PointerEventData eventData;
        private float timer = 3;
        private string timerName;
        private InfoSpriteShow lastSelected;
        private bool show = false;


        //protected override void PropBindings()
        //{
        //    base.PropBindings();
        //    Binder.RegistValueChange<int>(view.SetFontSize, 1);
        //    Binder.RegistValueChange<Vector3>(SetPosition, 2);
        //    Binder.RegistValueChange<string>(view.SetTitle, 3);
        //    Binder.RegistValueChange<Color>(view.SetColor, 4);
        //    Binder.RegistValueChange<Sprite>(view.SetSprite, 5);
        //}

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
                Object.Destroy(gameObject);
            }
        }


        private void ShowText()
        {
            if (!string.IsNullOrEmpty(spriteInfoItem.title.text))
            {
                show = true;
                textBody.SetActive(true);
                onGet.Invoke(spriteInfoItem.title.text);
            }
        }

        private void SetPosition(Vector3 pos)
        {
           transform.position = pos;
        }

        protected override void OnInitialize()
        {
            textBody = spriteInfoItem.gameObject;
            eventData = new PointerEventData(EventSystem.current);
            ShowText();
        }

        protected override void OnRecover()
        {
        }
    }
}