using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

namespace BridgeUI.Control
{
    public class ScrollChildern : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private float speed = 30;
        [SerializeField]
        private RectTransform current;

        private RectTransform[] childern;
        private bool enter;
        private float scollValue;

        private void Awake()
        {
            current = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (enter)
            {
                scollValue = Input.GetAxis("Mouse ScrollWheel");
                if (scollValue != 0)
                {
                    ApplyChanged(scollValue);
                }
            }
        }

        private void ApplyChanged(float scollValue)
        {
            var chenged = Vector3.one * scollValue * speed * Time.deltaTime;
            for (int i = 0; i < current.childCount; i++)
            {
                var child = current.GetChild(i);
                child.localScale += chenged;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            enter = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            enter = false;
        }
    }
}