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
    public class ButtonCreater : DefultElementCreater
    {
        protected override string MenuPath
        {
            get
            {
                return "GameObject/UI/Button";
            }
        }
        protected override Type CommponentType
        {
            get
            {
                return typeof(Button);
            }
        }
        protected override void ChargeWidgetInfo(UnityEngine.Component component, WidgetItem info)
        {
            var btn = component as Button;
            var spriteDic = info.spriteDic;
            if (spriteDic.ContainsKey(KeyWord.normal))
            {
                var texture = spriteDic[KeyWord.normal];

                var image = btn.targetGraphic as Image;
                image.sprite = texture;
                image.type = Image.Type.Simple;
                WidgetUtility.SetNativeSize(image);
            }

            foreach (var item in spriteDic)
            {
                if (item.Key == KeyWord.normal) continue;
                btn.transition = Selectable.Transition.SpriteSwap;
                var keyword = item.Key;
                var sprite = item.Value;
                var state = btn.spriteState;
                if (keyword == KeyWord.disabled)
                {
                    state.disabledSprite = sprite;
                }
                else if (keyword == KeyWord.highlighted)
                {
                    state.highlightedSprite = sprite;
                }
                else if (keyword == KeyWord.pressed)
                {
                    state.pressedSprite = sprite;
                }
                btn.spriteState = state;
            }
        }
        protected override Component CreateInstence(WidgetItem info)
        {
            var component = base.CreateInstence(info);
            if(component != null){
                component.GetComponentInChildren<Text>().text = "";
            }
            return component;
        }
        public override List<Sprite> GetPreviewList(WidgetItem info)
        {
            List<Sprite> List = new List<Sprite>();
            var spriteDic = info.spriteDic;
            if (spriteDic.ContainsKey(KeyWord.normal))
            {
                var sprite = spriteDic[KeyWord.normal];
                List.Add(sprite);
            }
            return List;
        }
        protected override List<string> CreateDefultList()
        {
            return new List<string>() { KeyWord.normal, KeyWord.pressed, KeyWord.highlighted, KeyWord.disabled };
        }
    }
}