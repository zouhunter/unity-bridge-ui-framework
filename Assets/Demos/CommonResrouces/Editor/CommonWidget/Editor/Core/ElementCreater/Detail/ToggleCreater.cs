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
    public class ToggleCreater : DefultElementCreater
    {
        protected override string MenuPath
        {
            get
            {
                return "GameObject/UI/Toggle";
            }
        }
        protected override Type CommponentType
        {
            get
            {
                return typeof(Toggle);
            }
        }
        protected override void ChargeWidgetInfo(Component component, WidgetItem info)
        {
            var spriteDic = info.spriteDic;
            var toggle = component as Toggle;
            var background = toggle.targetGraphic as Image;
            var mask = toggle.graphic as Image;
            if (spriteDic.ContainsKey(KeyWord.background))
            {
                background.sprite = spriteDic[KeyWord.background];
                background.type = Image.Type.Simple;
                WidgetUtility.SetNativeSize(background);
            }
            if (spriteDic.ContainsKey(KeyWord.mask))
            {
                mask.sprite = spriteDic[KeyWord.mask];
                background.type = Image.Type.Simple;
                WidgetUtility.SetNativeSize(background);
            }
        }
  
        public override List<Sprite> GetPreviewList(WidgetItem info)
        {
            List<Sprite> list = new List<Sprite>();
            var spriteDic = info.spriteDic;
            if (spriteDic.ContainsKey(KeyWord.background)){
                var sprite = spriteDic[KeyWord.background];
                list.Add(sprite);
            }
            return list;
        }
        protected override List<string> CreateDefultList()
        {
            return new List<string>() { KeyWord.background, KeyWord.mask};
        }
    }
}