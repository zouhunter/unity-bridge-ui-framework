using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

namespace BridgeUI.Common
{
    public class SpriteInfoItem : MonoBehaviour
    {
        public Text title;
        public Image image;

        internal void SetSprite(Sprite value)
        {
            image.sprite = value;
        }
        internal void SetTitle(string value)
        {
            title.text = value;
        }

        internal void SetColor(Color arg0)
        {
            title.color = arg0;
        }

        internal void SetFontSize(int arg0)
        {
            title.fontSize = arg0;
        }
    }
}