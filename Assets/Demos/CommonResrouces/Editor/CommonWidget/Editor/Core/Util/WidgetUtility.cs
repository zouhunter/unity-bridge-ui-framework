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
using System.IO;
using System;
using System.Linq;

namespace CommonWidget
{
    public static class WidgetUtility
    {
        //保存预制体的文件夹自动生成的guid（如果发生了变化请重写）
        private const string prefer_sprites_folder_guid = "prefer_sprites_folder_guid";
        //打开窗口的猜到，可自定义
        public const string Menu_widgetWindow = "Window/CommonWidget";
        private static Dictionary<WidgetType, IElementCreater> createrDic;
        private static string _defultSpritePath;
        public static string defultSpritePath
        {
            get
            {
                if (string.IsNullOrEmpty(_defultSpritePath))
                {
                    if (PlayerPrefs.HasKey(prefer_sprites_folder_guid))
                    {
                        var guid = PlayerPrefs.GetString(prefer_sprites_folder_guid);
                        _defultSpritePath = AssetDatabase.GUIDToAssetPath(guid);
                    }
                }
                return _defultSpritePath;
            }
            set
            {
                if(_defultSpritePath != value)
                {
                    _defultSpritePath = value;
                    var guid = AssetDatabase.AssetPathToGUID(value);
                    PlayerPrefs.SetString(prefer_sprites_folder_guid, guid);
                }
            }
        }

        internal static void SetNativeSize(Image image)
        {
            var sprite = image.sprite;
            if(sprite != null)
            {
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sprite.textureRect.width);
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sprite.textureRect.height);
            }
        }

        internal static void SetNativeSize(RawImage image)
        {
            var texture = image.texture;
            if (texture != null)
            {
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, texture.width);
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, texture.height);
            }
        }

        /// <summary>
        /// 从PrefabPathGUID加载所有的预制体
        /// </summary>
        /// <returns></returns>
        public static ObjectHolder[] LoadAllGameObject(string spritePath)
        {
            var holders = new List<ObjectHolder>();
           
            if (!string.IsNullOrEmpty(spritePath))
            {
                var fullPath = Path.GetFullPath(spritePath);
                var userDefines = LoadAllUserDefine(fullPath);
                var sprites = LoadAllSprites(fullPath);
                holders.AddRange(userDefines);
                foreach (var item in sprites)
                {
                    if(holders.Find(x=>x.name == item.name) == null)
                    {
                        holders.Add(item);
                    }
                }
                return holders.ToArray();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 从json文件中加载出配制
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static WidgetItem[] LoadWidgeItems(string json, string assetDir)
        {
            if (string.IsNullOrEmpty(json)) return null;

            var items = new List<WidgetItem>();

            var jsonarray = JSONNode.Parse(json).AsArray;
            foreach (var nodeItem in jsonarray)
            {
                var jsonClass = nodeItem as JSONClass;
                if (nodeItem != null && jsonClass != null)
                {
                    var item = new WidgetItem();
                    item.type = (WidgetType)Enum.Parse(typeof(WidgetType), jsonClass[KeyWord.type].Value);
                    item.name = jsonClass[KeyWord.name].Value;
                    item.spriteDic =  LoadTextures(jsonClass, assetDir);
                    foreach (var key in WidgetUtility.GetKeys(item.type))
                    {
                        if(!item.spriteDic.ContainsKey(key))
                        {
                            item.spriteDic.Add(key, null);
                        }
                    }
                    items.Add(item);
                }
            }
            return items.ToArray();
        }


        private static List<ObjectHolder> LoadAllSprites(string fullpath)
        {
            var holders = new List<ObjectHolder>();
            string[] spritepaths = Directory.GetFiles(fullpath, "*.png", SearchOption.AllDirectories);
            
            foreach (var spritepath in spritepaths)
            {
                var assetpath = spritepath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetpath);
                if (sprite != null)
                {
                    var holder = new ObjectHolder(sprite);
                    holders.Add(holder);
                }
            }
            return holders;
        }

        private static List<ObjectHolder> LoadAllUserDefine(string fullPath)
        {
            var holders = new List<ObjectHolder>();
            string[] rules = Directory.GetFiles(fullPath, "*.json", SearchOption.AllDirectories);
            foreach (var rulepath in rules)
            {
                var assetpath = rulepath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                var jsonname = System.IO.Path.GetFileName(assetpath);
                var assetDir = assetpath.Replace(jsonname, "");

                var jsonString = AssetDatabase.LoadAssetAtPath<TextAsset>(assetpath).text;
                if (string.IsNullOrEmpty(jsonString))
                {
                    continue;
                }

                var jsonarray = JSONArray.Parse(jsonString).AsArray;
                if (jsonarray == null) continue;

                foreach (var nodeItem in jsonarray)
                {
                    if (nodeItem != null && nodeItem is JSONClass)
                    {
                        var holder = new ObjectHolder(assetDir, (JSONClass)nodeItem);
                        holders.Add(holder);
                    }
                }

            }
            return holders;
        }
        public static void MakeTextureReadable(Texture texture)
        {
            var path = AssetDatabase.GetAssetPath(texture);
            var textureImporter = TextureImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter.textureType != TextureImporterType.Default || !textureImporter.isReadable)
            {
                textureImporter.isReadable = true;
                textureImporter.textureType = TextureImporterType.Default;
                textureImporter.SaveAndReimport();
            }
        }
        public static void MakeSpriteAsUISprite(Sprite sprite)
        {
            var path = AssetDatabase.GetAssetPath(sprite.texture);
            var textureImporter = TextureImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter.textureType != TextureImporterType.Sprite || textureImporter.isReadable)
            {
                textureImporter.isReadable = false;
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.SaveAndReimport();
            }
        }
        internal static void InitImage(Image image, Sprite sprite, Image.Type simple = Image.Type.Simple)
        {
            image.sprite = sprite;
            image.type = Image.Type.Simple;
            WidgetUtility.SetNativeSize(image);
        }

        internal static void InitRawImage(RawImage image, Texture texture)
        {
            image.texture = texture;
            WidgetUtility.SetNativeSize(image);
        }

        public static GameObject CreateOrCharge(WidgetType type, WidgetItem info)
        {
            var creater = GetCreater(type);
            return creater.CreateOrCharge(info);
        }
        public static Texture CreatePreview(WidgetType type, WidgetItem info)
        {
            var creater = GetCreater(type);
            var list = creater.GetPreviewList(info);
            return GetPreviewFromSpriteList(list);
        }
        internal static List<string> GetKeys(WidgetType type)
        {
            var creater = GetCreater(type);
            return creater.Keys;
        }
        private static IElementCreater GetCreater(WidgetType type)
        {
            if (createrDic == null)
            {
                createrDic = new Dictionary<WidgetType, IElementCreater>();
            }

            if (!createrDic.ContainsKey(type))
            {
                var typeName = "CommonWidget." + type.ToString() + "Creater";
                var createrType = typeof(IElementCreater).Assembly.GetType(typeName);
                if (createrType == null)
                {
                    Debug.LogError("请编写:" + typeName);
                    return null;
                }
                createrDic.Add(type, System.Activator.CreateInstance(createrType) as IElementCreater);
            }

            return createrDic[type];

        }

        public static Dictionary<string, Sprite> LoadTextures(JSONClass json, string assetDir)
        {
            var spriteDic = new Dictionary<string, Sprite>();
            if (json[KeyWord.image] != null && json[KeyWord.image].AsObject != null)
            {
                var obj = json[KeyWord.image].AsObject;
                foreach (var item in obj)
                {
                    var keyValue = JSONArray.Parse(item.ToString());
                    if (keyValue.Count < 2 || string.IsNullOrEmpty(keyValue[0])) continue;

                    Sprite sprite = null;
                    if (!string.IsNullOrEmpty(keyValue[1].Value))
                    {
                        var texturePath = assetDir + keyValue[1];
                        sprite = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
                    }
                    else if (keyValue[1].AsArray != null)
                    {
                        var texturesPath = assetDir + keyValue[1].AsArray[0].Value;
                        var spriteName = keyValue[1].AsArray[1].Value;
                        var sprites = AssetDatabase.LoadAllAssetsAtPath(texturesPath);

                        sprite = sprites.Where(x => x != null && x.name == spriteName).FirstOrDefault() as Sprite;
                    }

                    if (sprite != null)
                    {
                        spriteDic.Add(keyValue[0], sprite);
                    }
                }
              
            }

            return spriteDic;
        }

        /// <summary>
        /// 利用一组sprite.创建preview
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Texture GetPreviewFromSpriteList(List<Sprite> list)
        {
            if (list == null || list.Count == 0) return null;
            Texture2D texture = null;

            foreach (var sprite in list)
            {
                if (sprite == null) continue;
                if (sprite.texture == null) continue;

                var path = AssetDatabase.GetAssetPath(sprite.texture);
                var textureImporter = TextureImporter.GetAtPath(path) as TextureImporter;
                Texture2D current = null;

                if (textureImporter.spriteImportMode == SpriteImportMode.Multiple)
                {
                    current = ConventSpriteToTexture(sprite);
                }
                else
                {
                    current = sprite.texture;
                }

                if (texture == null)
                {
                    texture = current;
                }
                else
                {
                    MakeTextureReadable(texture);
                    MakeTextureReadable(current);
                    CoverTexture(texture, current);
                }
            }
            return texture;
        }
        private static Texture2D ConventSpriteToTexture(Sprite sprite)
        {
            MakeTextureReadable(sprite.texture);

            int width = (int)sprite.rect.width;
            int height = (int)sprite.rect.height;
            var rect = sprite.textureRect;
            var croppedTexture = new Texture2D(width, height);
            //var pixels = sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var i0 = (int)rect.x + i;
                    var j0 = (int)rect.y + j;
                    if (i0 < sprite.texture.width && j0 < sprite.texture.height)
                    {
                        croppedTexture.SetPixel(i,j,sprite.texture.GetPixel(i0,j0));
                    }
                }
            }
            croppedTexture.Apply();
            return croppedTexture;
        }

        private static Texture2D CoverTexture(Texture2D down, Texture2D up)
        {
            var newTexture = new Texture2D(down.width, down.height);
            var xSpan = (int)((down.width - up.width) / 2);
            var ySpan = (int)((down.height - up.height) / 2);

            for (int i = 0; i < down.width; i++)
            {
                for (int j = 0; j < down.height; j++)
                {
                    if (i > xSpan && i < down.width - xSpan)
                    {
                        var pix = up.GetPixel(i - xSpan, j - ySpan);
                        newTexture.SetPixel(i, j, pix);
                    }
                    else
                    {
                        newTexture.SetPixel(i, j, down.GetPixel(i, j));
                    }
                }
            }
            return down;
        }
        public static void DrawContentColor(Rect rect, Color color)
        {
            Color oringal = GUI.backgroundColor;
            GUI.backgroundColor = color;
            GUI.Box(rect, "");
            GUI.backgroundColor = oringal;
        }

    }
}