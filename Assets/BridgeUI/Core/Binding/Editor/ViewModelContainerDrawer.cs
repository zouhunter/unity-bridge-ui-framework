using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BridgeUI.Binding;
using UnityEditor;
using System;
using System.Linq;
using UnityEditorInternal;

namespace BridgeUI.Drawer
{
    [CustomEditor(typeof(ViewModelContainer))]
    public class ViewModelContainerDrawer : Editor
    {
        private GUIContent[] options;
        private Type[] viewModelTypes;
        private SerializedProperty prop_instences;
        private ReorderableList instencesList;
        private List<string> ignoreProps = new List<string> { "m_Script" };
        public const float middleButtonWidth = 45f;
      
        private void OnEnable()
        {
            InitPropertys();
            InitTypeOptions();
            InitInstenceList();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            TryDrawViewModelList();
            serializedObject.ApplyModifiedProperties();
        }

        private void InitInstenceList()
        {
            instencesList = new ReorderableList(prop_instences.serializedObject, prop_instences);
            instencesList.onAddCallback = OnAddElement;
            instencesList.onRemoveCallback = OnRemoveElement;
            instencesList.drawHeaderCallback = OnDrawHeader;
            instencesList.drawElementCallback = DrawElementCallBack;
            instencesList.elementHeightCallback = ElementHeightCallback;
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "视图模型列表(--优先绑定第一个ViewModel--)");
        }

        protected float ElementHeightCallback(int index)
        {
            var prop = prop_instences.GetArrayElementAtIndex(index);
            var height = EditorGUIUtility.singleLineHeight + BridgeEditorUtility.padding * 2;
            if (prop.objectReferenceValue != null && prop.isExpanded)
            {
                var se = BridgeUI.Drawer.BridgeEditorUtility. CreateCachedSerializedObject(prop.objectReferenceValue);
                height += BridgeEditorUtility.GetSerializedObjectHeight(se, ignoreProps) + BridgeEditorUtility.padding * 2;
            }
            return height;
        }

        protected void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect = BridgeUI.Drawer.BridgeEditorUtility.DrawBoxRect(rect, index.ToString());
            var prop = prop_instences.GetArrayElementAtIndex(index);
            var content = prop.objectReferenceValue == null ? new GUIContent("Null") : new GUIContent(prop.objectReferenceValue.name);

            var btnRect = new Rect(rect.x, rect.y, rect.width - middleButtonWidth, EditorGUIUtility.singleLineHeight);
            var objRect = new Rect(rect.x + rect.width - middleButtonWidth, rect.y, middleButtonWidth, EditorGUIUtility.singleLineHeight);

            if (GUI.Button(btnRect, content, EditorStyles.toolbarDropDown))
            {
                prop.isExpanded = !prop.isExpanded;
            }

            if (prop.objectReferenceValue != null)
            {
                if (GUI.Button(objRect, "", EditorStyles.objectFieldMiniThumb))
                {
                    EditorGUIUtility.PingObject(prop.objectReferenceValue);
                }
            }
            else
            {
                EditorGUI.LabelField(objRect, "Lost!!!");
            }

            if (prop.isExpanded && prop.objectReferenceValue != null)
            {
                DrawObjectDetail(prop.objectReferenceValue, rect);
            }
        }
        private void OnRemoveElement(ReorderableList list)
        {
            if(list.index >=0 && list.index < list.count)
            {
                var prop = prop_instences.GetArrayElementAtIndex(list.index);

                if (prop.objectReferenceValue)
                {
                    DestroyImmediate(prop.objectReferenceValue,true);
                    Debug.Log("删除："+ prop.objectReferenceValue);
                }

                prop_instences.DeleteArrayElementAtIndex(list.index);
                prop_instences.serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnAddElement(ReorderableList list)
        {
            EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.zero), options, -1, (x, y, z) =>
            {
                var type = viewModelTypes[z];
                var instence = GetAssetByType(type);
                prop_instences.InsertArrayElementAtIndex(0);
                var prop = prop_instences.GetArrayElementAtIndex(0);
                prop.objectReferenceValue = instence;
                prop_instences.serializedObject.ApplyModifiedProperties();
            }, null);
        }

        private ScriptableObject GetAssetByType(Type type)
        {
            var path = AssetDatabase.GetAssetPath(target);
            //var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            var instence = ScriptableObject.CreateInstance(type);
            instence.name = type.Name;
            instence.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(instence, path);
            return instence as ScriptableObject;
        }

        private void TryDrawViewModelList()
        {
            instencesList.DoLayoutList();
        }

        private void InitPropertys()
        {
            prop_instences = serializedObject.FindProperty("instences");
        }

        private void InitTypeOptions()
        {
            viewModelTypes = BridgeUI.Drawer.MvvmUtil.GetSubInstenceTypes(typeof(ViewModel)).ToArray();
            options = viewModelTypes.Select(x => new GUIContent(x.FullName)).ToArray();
        }

        protected void DrawObjectDetail(UnityEngine.Object obj, Rect rect)
        {
            if (obj != null)
            {
                var serializedObj = BridgeEditorUtility.CreateCachedSerializedObject(obj);
                rect.y += EditorGUIUtility.singleLineHeight + 5;
                serializedObj.Update();
                BridgeEditorUtility.DrawChildInContent(serializedObj.GetIterator(), rect, ignoreProps, null, 1);
                serializedObj.ApplyModifiedProperties();
            }
        }
        
    }
}