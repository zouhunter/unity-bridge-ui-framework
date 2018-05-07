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
using UnityEditorInternal;
using System.Reflection;
using System.Linq;
namespace EditorTools
{

    public class ScriptableObjectWindow : EditorWindow
    {
        #region InfoItem
        public class InfoItem
        {
            public bool isExand;
            public Assembly assemble;
            public Type type;
            public InfoItem parent;
            public List<InfoItem> childs = new List<InfoItem>();
            public ReorderableList listDrawer;

            public InfoItem()
            {
                InitDrawer();
            }
            public InfoItem(Assembly assemble, Type type, InfoItem parent) : this()
            {
                this.assemble = assemble;
                this.parent = parent;
                this.type = type;
            }
            public void OnGUI()
            {
                if (isExand)
                {
                    listDrawer.DoLayoutList();
                }
                else
                {
                    if (GUILayout.Button(assemble.FullName, EditorStyles.toolbarDropDown))
                    {
                        isExand = true;
                    }
                    GUILayout.Space(10);
                }
            }

            public float GetHeight()
            {
                return (isExand ? GetChildsHeight() : 0) + EditorGUIUtility.singleLineHeight;
            }

            private float GetChildsHeight()
            {
                float height = 0;
                foreach (var item in childs)
                {
                    height += item.GetHeight();
                }
                return height;
            }

            private void InitDrawer()
            {
                listDrawer = new ReorderableList(childs, typeof(InfoItem), true, false, false, false);
                listDrawer.drawHeaderCallback += DrawHeader;
                listDrawer.drawElementCallback += DrawElement;
                listDrawer.elementHeightCallback += ElementHeight;
            }

            private void DrawHeader(Rect rect)
            {
                if (GUI.Button(rect, assemble.FullName, EditorStyles.label))
                {
                    isExand = false;
                }
            }

            private float ElementHeight(int index)
            {
                return childs[index].GetHeight();
            }

            private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                var type = childs[index].type;
                if (type == null)
                {
                    //Debug.Log(childs[index].GetFullName());
                    return;
                }
                var left = new Rect(rect.x, rect.y, rect.width * 0.8f, EditorGUIUtility.singleLineHeight);
                var right = new Rect(rect.width * 0.9f, rect.y, rect.width * 0.1f, EditorGUIUtility.singleLineHeight);

                EditorGUI.LabelField(left, type.Name, EditorStyles.miniLabel);
                if (GUI.Button(right, new GUIContent("create", "create a " + type.FullName)))
                {
                    var instence = ScriptableObject.CreateInstance(type);
                    ProjectWindowUtil.CreateAsset(instence, type.Name + ".asset");
                }
            }

            internal void AddChild(InfoItem item)
            {
                if (!childs.Contains(item))
                {
                    childs.Add(item);
                }
            }
        }
        #endregion
        private Dictionary<string, InfoItem> assembleDic = new Dictionary<string, InfoItem>();
        private Vector2 pos;
        private SerializedProperty scriptProp;
        [MenuItem("Tools/ScriptObject")]
        public static void InitWindow()
        {
            GetWindow<ScriptableObjectWindow>();
        }

        private void OnEnable()
        {
            scriptProp = new SerializedObject(this).FindProperty("m_Script");
            LoadTypes();
        }

        //private void InitStyle()
        //{
        //    if (titleStyle == null)
        //    {
        //        titleStyle = new GUIStyle(GUI.skin.label);
        //        titleStyle.fontStyle = FontStyle.Bold;
        //        titleStyle.alignment = TextAnchor.MiddleCenter;
        //        titleStyle.fontSize = 30;
        //    }
        //}

        private void OnGUI()
        {
            EditorGUILayout.PropertyField(scriptProp);
            using (var scr = new EditorGUILayout.ScrollViewScope(pos))
            {
                pos = scr.scrollPosition;
                foreach (var item in assembleDic)
                {
                    if (item.Value.childs.Count > 0)
                    {
                        item.Value.OnGUI();
                    }
                }
            }
        }

        private void LoadTypes()
        {
            assembleDic.Clear();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var rootItem = new InfoItem(assembly, null, null);
                CreateInfoItem(assembly, rootItem);
                assembleDic.Add(assembly.FullName, rootItem);
            }
        }

        private void CreateInfoItem(Assembly assembly, InfoItem root)
        {
            var types = assembly.GetTypes()
                .Where(t => t != typeof(ScriptableObject))
                .Where(t => typeof(ScriptableObject).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract);

            foreach (var type in types)
            {
                var infoItem = new InfoItem(assembly, type, root);
                root.AddChild(infoItem);
            }
        }
    }
}
