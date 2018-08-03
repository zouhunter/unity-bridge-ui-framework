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
    public class CercalSliderCreater : ElementCreater
    {
        public override GameObject CreateOrCharge(WidgetItem info)
        {
            var ok = EditorApplication.ExecuteMenuItem("GameObject/UI/Image");
            if(ok)
            {
                var backgroundImage = Selection.activeGameObject;
                if(info.spriteDic.ContainsKey(KeyWord.background))
                {
                    WidgetUtility.InitImage( backgroundImage.GetComponent<Image>(), info.spriteDic[KeyWord.background]);
                }

                ok = EditorApplication.ExecuteMenuItem("GameObject/UI/Image");
                if(ok)
                {
                    var fillImage = Selection.activeGameObject.GetComponent<Image>();
                    fillImage.name = KeyWord.fill;
                  
                    fillImage.type = Image.Type.Filled;
                    fillImage.fillMethod = Image.FillMethod.Radial360;
                    if(info.spriteDic.ContainsKey(KeyWord.fill))
                    {
                        fillImage.sprite = info.spriteDic[KeyWord.fill];
                    }

                    fillImage.transform.SetParent(backgroundImage.transform, false);
                    (fillImage.transform as RectTransform).anchorMin = Vector2.zero;
                    (fillImage.transform as RectTransform).anchorMax = Vector2.one;
                    fillImage.transform.localPosition = Vector3.zero;
                    (fillImage.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (backgroundImage.transform as RectTransform).sizeDelta.x);
                    (fillImage.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (backgroundImage.transform as RectTransform).sizeDelta.y);

                }
                return backgroundImage;
            }
            return null;
        }

        public override List<Sprite> GetPreviewList(WidgetItem info)
        {
            var list = new List<Sprite>();
            if(info.spriteDic.ContainsKey(KeyWord.fill))
            {
                list.Add(info.spriteDic[KeyWord.fill]);
            }
            else if(info.spriteDic.ContainsKey(KeyWord.background))
            {
                list.Add(info.spriteDic[KeyWord.background]);
            }
            return list;
        }

        protected override List<string> CreateDefultList()
        {
            return new List<string>() { KeyWord.background, KeyWord.fill };
        }
    }
}