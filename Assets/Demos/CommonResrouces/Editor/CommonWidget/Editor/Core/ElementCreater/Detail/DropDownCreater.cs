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
using UnityEditor;
using System;

namespace CommonWidget
{
    public class DropdownCreater : DefultElementCreater
    {
        private string backgroundbase { get { return "backgroundbase"; } }
        private string backgroundgroup { get { return "backgroundgroup"; } }
        private string backgrounditem { get { return "backgrounditem"; } }
        private string scrollbarbackground { get { return "scrollbarbackground"; } }
        private string scrollbarfill { get { return "scrollbarfill"; } }
        protected override string MenuPath
        {
            get
            {
                return "GameObject/UI/Dropdown";
            }
        }
        protected override Type CommponentType
        {
            get
            {
                return typeof(Dropdown);
            }
        }
        protected override void ChargeWidgetInfo(Component component, WidgetItem info)
        {
            Dropdown dropdown = component as Dropdown;
            var dic = info.spriteDic;
            if (dic.ContainsKey(backgroundbase))
            {
                WidgetUtility.InitImage(dropdown.targetGraphic as Image, dic[backgroundbase], Image.Type.Sliced);
            }

            if (dic.ContainsKey(backgroundgroup))
            {
                dropdown.template.GetComponent<Image>().sprite = dic[backgroundgroup];
            }

            if (dic.ContainsKey(backgrounditem))
            {
                var itemgraph = dropdown.template.GetComponentInChildren<Toggle>().targetGraphic as Image;
                itemgraph.sprite = dic[backgrounditem];
            }

            var scrollbar = dropdown.template.GetComponentInChildren<Scrollbar>();

            if (dic.ContainsKey(scrollbarbackground))
            {
                (scrollbar.targetGraphic as Image).sprite = dic[scrollbarbackground];
            }
            if (dic.ContainsKey(scrollbarfill))
            {
                scrollbar.handleRect.GetComponent<Image>().sprite = dic[scrollbarfill];
            }
        }

        public override List<Sprite> GetPreviewList(WidgetItem info)
        {
            var list = new List<Sprite>();
            if(info.spriteDic.ContainsKey(backgroundbase))
            {
                list.Add(info.spriteDic[backgroundbase]);
            }
            return list;
        }

        protected override List<string> CreateDefultList()
        {
            return new List<string>() { backgroundbase, backgroundgroup, backgrounditem, scrollbarbackground, scrollbarfill };
        }
    }

}
