using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace CommonWidget
{
    public class SliderCreater : DefultElementCreater
    {
        protected override string MenuPath
        {
            get
            {
                return "GameObject/UI/Slider";
            }
        }
        protected override Type CommponentType
        {
            get
            {
                return typeof(Slider);
            }
        }
        protected override void ChargeWidgetInfo(Component component, WidgetItem info)
        {
            var slider =  component as Slider;
            var dic = info.spriteDic;
            if (dic.ContainsKey(KeyWord.background))
            {
                var image = slider.transform.Find("Background").GetComponent<Image>();
                image.sprite = dic[KeyWord.background];

                if (image.sprite.rect.width > image.sprite.rect.height)
                {
                    slider.SetDirection(Slider.Direction.LeftToRight, true);
                }
                else
                {
                    slider.SetDirection(Slider.Direction.BottomToTop, true);
                }

                var sliderRect = slider.GetComponent<RectTransform>();
                sliderRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, image.sprite.rect.width);
                sliderRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, image.sprite.rect.height);
                var backgroundRect = image.GetComponent<RectTransform>();
                backgroundRect.anchorMin = Vector2.zero;
                backgroundRect.anchorMax = Vector2.one;
            }

            if (dic.ContainsKey(KeyWord.fill))
            {
                slider.fillRect.gameObject.SetActive(true);
                slider.fillRect.GetComponent<Image>().sprite = dic[KeyWord.fill];
            }
            else
            {
                slider.fillRect.gameObject.SetActive(false);
            }

            if(slider.fillRect != null)
            {
                var fileRectParent = slider.fillRect.parent as RectTransform;
                fileRectParent.anchoredPosition = Vector2.zero;
                fileRectParent.sizeDelta = Vector2.zero;
            }

            if (dic.ContainsKey(KeyWord.handle))
            {
                slider.handleRect.gameObject.SetActive(true);
                var handleImage = slider.handleRect.GetComponent<Image>();
                handleImage.sprite = dic[KeyWord.handle];
            }
            else
            {
                slider.handleRect.gameObject.SetActive(false);
            }

            if(slider.handleRect != null)
            {
                var handleRectParent = slider.handleRect.parent as RectTransform;
                handleRectParent.sizeDelta = Vector2.zero;
            }
        }

        public override List<Sprite> GetPreviewList(WidgetItem info)
        {
            var list = new List<Sprite>();
            var dic = info.spriteDic;
            if (dic != null)
            {
                if (dic.ContainsKey(KeyWord.background))
                {
                    list.Add(dic[KeyWord.background]);
                }

                if (dic.ContainsKey(KeyWord.fill))
                {
                    list.Add(dic[KeyWord.fill]);
                }
            }
            return list;
        }

        protected override List<string> CreateDefultList()
        {
            return new List<string>() { KeyWord.background, KeyWord.fill, KeyWord.handle };
        }
    }

}
