using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace CommonWidget
{
    public class RawImageCreater : DefultElementCreater
    {
        protected override string MenuPath
        {
            get
            {
                return "GameObject/UI/Raw Image";
            }
        }
        protected override Type CommponentType
        {
            get
            {
                return typeof(RawImage);
            }
        }
        protected override void ChargeWidgetInfo(Component component, WidgetItem info)
        {
            var image = component as RawImage;
            if (info.spriteDic.ContainsKey(KeyWord.sprite))
            {
                WidgetUtility.InitRawImage(image, info.spriteDic[KeyWord.sprite].texture);
            }
        }
     
        public override List<Sprite> GetPreviewList(WidgetItem info)
        {
            List< Sprite > list = new List<Sprite>();
            if (info.spriteDic.ContainsKey(KeyWord.sprite))
            {
                var sprite = info.spriteDic[KeyWord.sprite];
                if (sprite != null)
                {
                    list.Add(sprite);
                }
            }
            return list;
        }
        protected override List<string> CreateDefultList()
        {
            return new List<string>() { KeyWord.sprite };
        }
    }
}
