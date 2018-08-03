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
using UnityEditor;
using System.Collections.Generic;

namespace CommonWidget
{
    public class ObjectHolder
    {
        private Texture _preview;
        private Dictionary<string, Sprite> _textures;
        public bool effective { get; private set; }
        public string name { get; private set; }
        public WidgetType widgetType { get; private set; }
        public Texture Preview
        {
            get
            {
                if (_preview == null)
                    _preview = WidgetUtility.CreatePreview(widgetType, new WidgetItem(name, spriteDic));
                return _preview;
            }
        }
        public string menuName { get { return widgetType.ToString(); } }
        public Dictionary<string, Sprite> spriteDic { get { if (_textures == null) _textures = WidgetUtility.LoadTextures(json, assetDir); return _textures; } }

        private JSONClass json;
        private string assetDir;

        public ObjectHolder(string dir, JSONClass json)
        {
            if (json[KeyWord.name] != null)
            {
                this.name = json[KeyWord.name];
            }
            else
            {
                this.name = json[KeyWord.type].Value;
            }
            this.assetDir = dir;
            effective = true;

            this.json = json;

            if (string.IsNullOrEmpty(json.ToString()) || string.IsNullOrEmpty(json[KeyWord.type].Value))
            {
                effective = false;
                return;
            }

            var type = System.Enum.Parse(typeof(WidgetType), json[KeyWord.type]);
            if (type == null)
            {
                effective = false;
                return;
            }

            widgetType = (WidgetType)type;
            effective = true;
        }

        public ObjectHolder(Sprite sprite)
        {
            widgetType = WidgetType.RawImage;
            name = sprite.name;
            _textures = new Dictionary<string, Sprite>();
            _textures.Add(KeyWord.sprite, sprite);
        }

        public GameObject CreateInstence()
        {
            var lastTrans = Selection.activeTransform;
            if (lastTrans == null || !(lastTrans is RectTransform))
            {
                var canvas = GameObject.FindObjectOfType<Canvas>();
                if (canvas == null){
                    EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
                }
            }

            var info = new WidgetItem(name, spriteDic);
            var created = WidgetUtility.CreateOrCharge(widgetType, info);
            return created;
        }
    }
}

