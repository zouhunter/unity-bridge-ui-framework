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
using UnityEditorInternal;
using System;

namespace CommonWidget
{
    public class ConfigWidow : EditorWindow
    {
        private TextAsset jsonFile;//用于加载和保存widge信息
        private List<WidgetItem> widgetItems = new List<WidgetItem>();
        private ReorderableList reorderableList;
        private const string prefer_lastwidgetconfig = "prefer_lastwidgetconfig";
        private Vector2 scrollPos;
        private void OnEnable()
        {
            LoadLastConfig();
            InitReorderableList();
        }
        private void OnGUI()
        {
            DrawJsonFile();
            if (jsonFile != null)
            {
                using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = scroll.scrollPosition;
                    reorderableList.DoLayoutList();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("请填入或创建配制文档", MessageType.Info);
            }
            DrawToolButtons();
        }
      
        private void DrawToolButtons()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                if (jsonFile != null && widgetItems.Count > 0)
                {
                    if (GUILayout.Button("save", EditorStyles.miniButtonMid))
                    {
                        Undo.RecordObject(this, "save window");
                        SaveToJsonFile();
                    }
                }
            }
        }

        private void SaveToJsonFile()
        {
            var filePath = AssetDatabase.GetAssetPath(jsonFile);
            string dir = filePath.Replace(System.IO.Path.GetFileName(filePath), "");

            var jsonArray = new JSONArray();
            foreach (var item in widgetItems)
            {
                jsonArray.Add(item.ToJson(dir));
            }
            System.IO.File.WriteAllText(filePath, jsonArray.ToString());
            AssetDatabase.Refresh();
        }

        private void DrawJsonFile()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("配制文档:", GUILayout.Width(100));
                EditorGUI.BeginChangeCheck();
                jsonFile = EditorGUILayout.ObjectField(jsonFile, typeof(TextAsset), false) as TextAsset;
                if (EditorGUI.EndChangeCheck())
                {
                    SaveCurrentConfig();
                    InportFromJson();
                }
                if (GUILayout.Button("create", EditorStyles.miniButtonRight))
                {
                    var path = EditorUtility.SaveFilePanelInProject("保存路径", "config", "json", "请确保配制文件路径在资源路径之上");
                    if (!string.IsNullOrEmpty(path))
                    {
                        System.IO.File.WriteAllText(path, "");
                        AssetDatabase.Refresh();
                        EditorApplication.delayCall = () =>
                        {
                            jsonFile = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                        };
                    }

                }
            }

        }

        private void InitReorderableList()
        {
            reorderableList = new ReorderableList(widgetItems, typeof(WidgetItem));
            reorderableList.onAddCallback += OnAdd;
            reorderableList.drawHeaderCallback += DrawHead;
            reorderableList.drawElementCallback += DrawWidgeItem;
            reorderableList.elementHeightCallback += GetWidgeItemHeight;
        }

        private void OnAdd(ReorderableList list)
        {
            widgetItems.Add(new WidgetItem(WidgetType.Button));
        }

        private float GetWidgeItemHeight(int index)
        {
            var height = 1;
            var item = widgetItems[index];
            if (item.spriteDic != null)
            {
                height += item.spriteDic.Count;
            }
            return height * EditorGUIUtility.singleLineHeight + 10;
        }

        private void DrawWidgeItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            var boxRect = new Rect(rect.x + 2, rect.y + 2, rect.width - 4, rect.height - 4);
            WidgetUtility.DrawContentColor(boxRect, Color.green);

            rect = new Rect(rect.x + 5, rect.y + 5, rect.width - 10, rect.height - 10);

            var item = widgetItems[index];

            var typeRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginChangeCheck();
            item.type = (WidgetType)EditorGUI.EnumPopup(typeRect, item.type);
            if (EditorGUI.EndChangeCheck())
            {
                item.ChangeType(item.type);
            }

            var nameRect = new Rect(rect.x + rect.width * 0.3f, rect.y, rect.width * 0.7f, EditorGUIUtility.singleLineHeight);
            item.name = EditorGUI.TextField(nameRect, item.name);

            if (item.spriteDic == null) return;

            var itemnameRect = new Rect(rect.x + 0.1f * rect.width, rect.y + EditorGUIUtility.singleLineHeight, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
            var spriteRect = new Rect(rect.x + 0.4f * rect.width, rect.y + +EditorGUIUtility.singleLineHeight, rect.width * 0.6f, EditorGUIUtility.singleLineHeight);
            var keys = item.spriteDic.Keys;
            var array = new List<string>(keys);
            foreach (var key in array)
            {
                EditorGUI.LabelField(itemnameRect, key);
                item.spriteDic[key] = EditorGUI.ObjectField(spriteRect, item.spriteDic[key], typeof(Sprite), false) as Sprite;
                itemnameRect.y += EditorGUIUtility.singleLineHeight;
                spriteRect.y += EditorGUIUtility.singleLineHeight;
            }

        }

        private void DrawHead(Rect rect)
        {
            EditorGUI.LabelField(rect, "配制列表(请确保图片在文档的同文件夹或子文件夹下)");
        }
        private void InportFromJson()
        {
            if (jsonFile == null) return;
            var filePath = AssetDatabase.GetAssetPath(jsonFile);
            string dir = filePath.Replace(System.IO.Path.GetFileName(filePath), "");
            var json = jsonFile.text;
            var loaded = WidgetUtility.LoadWidgeItems(json, dir);
            if (loaded != null)
            {
                widgetItems.Clear();
                widgetItems.AddRange(loaded);
            }
        }

        private void LoadLastConfig()
        {
            if (PlayerPrefs.HasKey(prefer_lastwidgetconfig))
            {
                var guid = PlayerPrefs.GetString(prefer_lastwidgetconfig);
                if (!string.IsNullOrEmpty(guid))
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (!string.IsNullOrEmpty(path))
                    {
                        jsonFile = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                        if (jsonFile != null)
                        {
                            InportFromJson();
                        }
                    }
                }
            }
        }
        private void SaveCurrentConfig()
        {
            if (jsonFile != null)
            {
                var path = AssetDatabase.GetAssetPath(jsonFile);
                var guid = AssetDatabase.AssetPathToGUID(path);
                PlayerPrefs.SetString(prefer_lastwidgetconfig, guid);
                PlayerPrefs.Save();
            }
        }
    }
}