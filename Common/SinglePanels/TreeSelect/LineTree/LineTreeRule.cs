using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace BridgeUI.Common.Tree
{

    [System.Serializable]
    public class LineTreeRule
    {
        public int deepth = 0;
        public int fontSize = 18;
        public float horizontal = 100;
        public float vertical = 100;
        public float spacing = 2f;
        public Color fontColor_normal = Color.white;
        public Color fontColor_mask = Color.white;
        public Font font;
        public Sprite normal;
        public Sprite mask;
        public bool childCloseAble;
        public bool makeGroup;

        public LineTreeRule()
        {
            fontColor_normal = Color.white;
        }
        public LineTreeRule CreateCopy(int deepth)
        {
            var rule = new LineTreeRule();
            rule.deepth = deepth;
            rule.fontColor_normal = fontColor_normal;
            rule.horizontal = horizontal;
            rule.vertical = vertical;
            rule.spacing = spacing;
            rule.normal = normal;
            rule.mask = mask;
            return rule;
        }
    }
}