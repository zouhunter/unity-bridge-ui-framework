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
using System.Text.RegularExpressions;
using System.Linq;

namespace CommonWidget
{
    public class WidgetWindow : EditorWindow
    {
        [MenuItem(WidgetUtility.Menu_widgetWindow)]
        static void OpenWidow()
        {
            GetWindow<WidgetWindow>();
        }

        private SerializedObject serializeObj;
        private SerializedProperty scriptProp;
        private int currToolbar;
        private string[] menus = null;
        private ObjectHolder[] allobjhs;
        private ObjectHolder[] currobjhs;
        private Vector3 scrollpos;
        private string userPath
        {
            get
            {
                return WidgetUtility.defultSpritePath;
            }
            set
            {
                WidgetUtility.defultSpritePath = value;
            }
        }
        private string match;
        private Vector2 scroll_left;

        private void OnEnable()
        {
            serializeObj = new SerializedObject(this);
            scriptProp = serializeObj.FindProperty("m_Script");
            UpdateObjectHolders();
        }
        private void OnGUI()
        {
            DrawHeader();
            DrawBodyContent();
        }

        private void OnDisable()
        {
            MakeUISpriteTypeBack();
        }

        private void DrawBodyContent()
        {
            var height = this.position.height - 3 * EditorGUIUtility.singleLineHeight;
            var width = this.position.width - 1 * EditorGUIUtility.singleLineHeight;

            using (var hor = new EditorGUILayout.HorizontalScope(GUILayout.Width(width), GUILayout.Height(height)))
            {
                using (var ver = new EditorGUILayout.VerticalScope(GUILayout.Width(width * 0.3f), GUILayout.Height(height)))
                {
                    WidgetUtility.DrawContentColor(ver.rect, Color.green);

                    using (var scroll = new EditorGUILayout.ScrollViewScope(scroll_left, false, true))
                    {
                        scroll_left = scroll.scrollPosition;
                        DrawWidghtsOptions(width * 0.3f);
                    }
                }

                using (var ver = new EditorGUILayout.VerticalScope(GUILayout.Width(width * 0.7f), GUILayout.Height(height)))
                {
                    WidgetUtility.DrawContentColor(ver.rect, Color.green);

                    DrawScrollViewObjs(width * 0.7f, height * 0.9f);

                    using (var hor0 = new EditorGUILayout.HorizontalScope(GUILayout.Width(width * 0.7f), GUILayout.Height(height * 0.1f)))
                    {
                        WidgetUtility.DrawContentColor(hor0.rect, Color.green);

                        DrawToolButtons();
                    }
                }
            }
        }


        private void DrawWidghtsOptions(float width)
        {
            if (menus == null) return;
            using (var ver = new EditorGUILayout.VerticalScope(GUILayout.Width(width)))
            {
                for (int i = 0; i < menus.Length; i++)
                {
                    var menuName = menus[i];
                    Color color = GUI.contentColor;
                    GUI.contentColor = i == currToolbar ? Color.green : color;
                    if (GUILayout.Button(menuName, EditorStyles.toolbarButton, GUILayout.Width(width - 20)))
                    {
                        currToolbar = i;
                        LoadCurrObjects();
                    }
                    GUI.contentColor = color;
                }
            }
        }

        private void MakeUISpriteTypeBack()
        {
            if (allobjhs == null) return;

            foreach (var item in allobjhs)
            {
                if (item.spriteDic == null) continue;
                float current = 0;
                float all = item.spriteDic.Count;
                foreach (var spriteItem in item.spriteDic)
                {
                    EditorUtility.DisplayProgressBar("wait", "还原图片状态", current++ / all);

                    if (spriteItem.Value != null)
                    {
                        WidgetUtility.MakeSpriteAsUISprite(spriteItem.Value);
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }
        private void UpdateObjectHolders()
        {
            allobjhs = WidgetUtility.LoadAllGameObject(userPath);
            if (allobjhs != null)
            {
                List<string> menus = new List<string>();
                for (int i = 0; i < allobjhs.Length; i++)
                {
                    if (!menus.Contains(allobjhs[i].menuName))
                    {
                        menus.Add(allobjhs[i].menuName);
                    }
                }
                this.menus = menus.ToArray();
                if (menus.Count > 0)
                {
                    LoadCurrObjects();
                }
            }
        }



        private void DrawSurchOption()
        {
            EditorGUI.BeginChangeCheck();
            match = EditorGUILayout.TextField(match);
            if (EditorGUI.EndChangeCheck())
            {
                LoadCurrObjects();
                var containsKey = new List<ObjectHolder>(currobjhs);
                Regex regex = new Regex(match, RegexOptions.IgnoreCase);
                if (!string.IsNullOrEmpty(match))
                {
                    currobjhs = containsKey.FindAll(x => regex.Match(x.name).Length > 0).ToArray();
                }
            }
        }

        private void DrawHeader()
        {
            using (var ver = new EditorGUILayout.VerticalScope(GUILayout.Height(3 * EditorGUIUtility.singleLineHeight)))
            {
                WidgetUtility.DrawContentColor(ver.rect, Color.yellow);

                EditorGUILayout.PropertyField(scriptProp);

                GUILayout.Space(5);

                using (var hor = new EditorGUILayout.HorizontalScope())
                {

                    EditorGUILayout.LabelField("自定义加载路径:", GUILayout.Width(100));
                    if (GUILayout.Button(userPath, EditorStyles.label))
                    {
                        var folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(userPath);
                        Selection.activeObject = folder;
                    }

                    if (GUILayout.Button("选择", EditorStyles.miniButtonRight, GUILayout.Width(40)))
                    {
                        if (Selection.activeObject != null && ProjectWindowUtil.IsFolder(Selection.activeInstanceID))
                        {
                            var path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
                            userPath = path;
                            currobjhs = null;
                            UpdateObjectHolders();
                        }
                        else
                        {
                            var folder = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
                            folder = folder.Replace(Application.dataPath, "Assets");
                            if (!string.IsNullOrEmpty(folder))
                            {
                                userPath = folder;
                                currobjhs = null;
                                UpdateObjectHolders();
                            }
                        }
                    }
                }
            }

        }

        private void DrawToolButtons()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Tools:", GUILayout.Width(40));
                if (GUILayout.Button("Config"))
                {
                    OpenConfigWindow();
                }
                GUILayout.FlexibleSpace();
                if(GUILayout.Button("ALINE", GUILayout.Width(60)))
                {
                    AnchorsUtil.AnchorsToCorners();
                }
                if (GUILayout.Button("Expand", GUILayout.Width(60)))
                {
                    AnchorsUtil.CornersToAnchors();
                }
            }
        }

        private void OpenConfigWindow()
        {
            ConfigWidow.GetWindow<ConfigWidow>("配制面板", true);
        }
        
        private void DrawScrollViewObjs(float scrollwidth, float scrollheight)
        {
            using (var scro = new EditorGUILayout.ScrollViewScope(scrollpos, GUILayout.Width(scrollwidth), GUILayout.Height(scrollheight)))
            {
                scrollpos = scro.scrollPosition;

                if (currobjhs != null)
                {
                    int horCount = 4;
                    var width = (scrollwidth * 0.9f) / horCount;
                    EditorGUILayout.BeginHorizontal();
                    for (int i = 0; i < currobjhs.Length; i++)
                    {
                        if ((i + horCount) % horCount == 0)
                        {
                            EditorGUILayout.EndHorizontal();
                        }
                        if (i % horCount == 0)
                        {
                            EditorGUILayout.BeginHorizontal();
                        }
                        var item = currobjhs[i];
                        var click = GUILayout.Button(new GUIContent(item.Preview), GUILayout.MinWidth(width), GUILayout.MinHeight(width * 0.6f));
                        var lastRect = GUILayoutUtility.GetLastRect();
                        GUI.Label(lastRect, item.name);
                        if (click)
                        {
                            item.CreateInstence();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void LoadCurrObjects()
        {
            string menu = menus[currToolbar];
            var targetItems = new List<ObjectHolder>(allobjhs);
            var items = targetItems.FindAll(x => x.menuName == menu);
            if (items != null)
            {
                currobjhs = items.ToArray();
            }
            else
            {
                Debug.Log("找不到" + menu);
            }
        }

        
    }
}
