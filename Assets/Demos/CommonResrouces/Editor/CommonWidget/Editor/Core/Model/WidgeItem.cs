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

namespace CommonWidget
{
    //用于记录一条信息
    public class WidgetItem
    {
        public WidgetType type;
        public string name;
        public Dictionary<string, Sprite> spriteDic;
        public Dictionary<string, Sprite> catchDic;//防止误操作
        public WidgetItem() { }

        public WidgetItem(WidgetType type)
        {
            ChangeType(type);
        }
        
        public void ChangeType(WidgetType type)
        {
            this.type = type;
            InitSpriteDic();
        }
        public WidgetItem(string name, Dictionary<string, Sprite> spriteDic)
        {
            this.name = name;
            this.spriteDic = spriteDic;
        }
        public JSONClass ToJson(string rootPath)//json文件的asset路径的文件夹/
        {
            JSONClass json = new JSONClass();
            json[KeyWord.type] = type.ToString();
            json[KeyWord.name] =string.IsNullOrEmpty(name)?type.ToString():name;
            if (spriteDic != null)
            {
                var imagedic = json[KeyWord.image] = new JSONClass();
                foreach (var item in spriteDic)
                {
                    if(item.Value != null){
                        imagedic[item.Key] = GetSpritePath(rootPath, item.Value);
                    }
                }
            }
            return json;
        }

        private JSONNode GetSpritePath(string root, Sprite sprite)
        {
            var texture = sprite.texture;
            var path = AssetDatabase.GetAssetPath(texture);
            if (path.Length < root.Length){
                Debug.LogError("请确保图片在文档的同文件夹或子文件夹下!");
            }
            var textureImporter = TextureImporter.GetAtPath(path) as TextureImporter;
            var texturePath = path.Replace(root, "");
            if (textureImporter.spriteImportMode == SpriteImportMode.Multiple)
            {
                var jsonArray = new JSONArray();
                jsonArray.Add(texturePath);
                jsonArray.Add(sprite.name);
                return jsonArray;
            }
            else
            {
                return texturePath;
            }
        }

        void InitSpriteDic()
        {
            var keys = WidgetUtility.GetKeys(type);

            if (spriteDic == null)
            {
                spriteDic = new Dictionary<string, Sprite>();
            }
            else
            {
                a: foreach (var item in spriteDic)
                {
                    if (!keys.Contains(item.Key))
                    {
                        Catch(item.Key, item.Value);
                        spriteDic.Remove(item.Key);
                        goto a;
                    }
                }
            }

            foreach (var item in keys)
            {
                if (!spriteDic.ContainsKey(item))
                {
                    spriteDic.Add(item, TryLoad(item));
                }
            }
        }

        void Catch(string key,Sprite sprite)
        {
            if (sprite == null) return;
            if(catchDic == null)
            {
                catchDic = new Dictionary<string, Sprite>();
            }
            catchDic[key] = sprite;
        }
        Sprite TryLoad(string key)
        {
            if(catchDic != null && catchDic.ContainsKey(key))
            {
                return catchDic[key];
            }
            return null;
        }
    }
}