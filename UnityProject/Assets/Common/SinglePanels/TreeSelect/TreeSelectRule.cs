using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace BridgeUI.Common
{

    [System.Serializable]
    public class TreeSelectRule
    {
        public int deepth = 0;
        public int fontSize = 18;
        public float horizontal = 100;
        public float vertical = 100;
        public float spacing = 2f;
        public Color fontColor = Color.white;
        public Sprite normal;
        public Sprite mask;
        public bool defultOpen;
        public TreeSelectRule()
        {
            fontColor = Color.white;
        }
        public TreeSelectRule CreateCopy(int deepth)
        {
            var rule = new TreeSelectRule();
            rule.deepth = deepth;
            rule.fontColor = fontColor;
            rule.horizontal = horizontal;
            rule.vertical = vertical;
            rule.spacing = spacing;
            rule.normal = normal;
            rule.mask = mask;
            return rule;
        }
    }
}