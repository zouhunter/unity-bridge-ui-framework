using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using UnityEngine;
using UnityEditor.UI;
using NodeEditorFramework;
using NodeEditorFramework.Standard;
using System;
using System.Reflection;
namespace PrefabGenerate
{
    [CustomEditor(typeof(ObjectNode), true)]
    public class ObjectNodeDrawer : NodeInspector
    {
        SerializedProperty sHoldProp;
        SerializedProperty modifyProp;
        ObjectNode _node;
        GameObject _scriptTempObj;
        MonoScript textasset;
        Dictionary<MonoScript, SerializedObject> monoTemp = new Dictionary<MonoScript, SerializedObject>();
        public new void OnEnable()
        {
            base.OnEnable();
            _node = (ObjectNode)node;
            foreach (var item in _node.sHolds)
            {
                item.InitVariables();
            }
            modifyProp = serializedObject.FindProperty("modify");
            sHoldProp = serializedObject.FindProperty("sHolds");
            CreateHoldAndLoadInformation();
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
            DrawScriptInfo();
            DrawModifyInfo();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawModifyInfo()
        {
            if (GUILayout.Button("添加修饰脚本", EditorStyles.toolbarButton))
            {
                var modifyTempGUID = "884bbfff294ca6945891de6e938eb83d";
                var path = AssetDatabase.GUIDToAssetPath(modifyTempGUID);
                if (!string.IsNullOrEmpty(path))
                {
                    CreateScriptUtil("newModifyer.cs", path);
                }
                else
                {
                    Debug.LogError("模板文件[ModifyTemp.txt].meta文件发生变化");
                }
            }

            if (textasset == null && modifyProp.objectReferenceValue != null)
            {
                textasset = MonoScript.FromScriptableObject(modifyProp.objectReferenceValue as PrefabModify);
            }
            EditorGUI.BeginChangeCheck();

            if(textasset == null)
            {
                textasset = EditorGUILayout.ObjectField(textasset, typeof(MonoScript), false) as MonoScript;
            }

            bool change = EditorGUI.EndChangeCheck();

            if (change)
            {
                if (textasset.GetClass().IsSubclassOf(typeof(PrefabModify)))
                {
                    modifyProp.objectReferenceValue = ScriptableObject.CreateInstance(textasset.GetClass());
                }
                else
                {
                    if (modifyProp.objectReferenceValue != null)
                    {
                        textasset = MonoScript.FromScriptableObject(modifyProp.objectReferenceValue as PrefabModify);
                    }
                    else
                    {
                        textasset = null;
                    }
                }
            }

            if (modifyProp.objectReferenceValue != null)
            {
                SerializedObject obj = new SerializedObject(modifyProp.objectReferenceValue as PrefabModify);
                obj.Update();
                SerializedProperty iterator = obj.GetIterator();
                bool enterChildren = true;
                while (iterator.NextVisible(enterChildren))
                {
                    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                    enterChildren = false;
                }
                obj.ApplyModifiedProperties();
            }
        }

        public static void CreateScriptUtil(string path, string templete)
        {
            MethodInfo method = typeof(ProjectWindowUtil).GetMethod("CreateScriptAsset",
                BindingFlags.Static | BindingFlags.NonPublic);
            if (method != null)
                method.Invoke(null, new object[] { templete, path });
        }

        private void DrawScriptInfo()
        {
            if (GUILayout.Button("添加关联脚本", EditorStyles.toolbarButton))
            {
                sHoldProp.InsertArrayElementAtIndex(0);
                sHoldProp.GetArrayElementAtIndex(0).FindPropertyRelative("monoScript").objectReferenceValue = null;
            }

            if (sHoldProp.arraySize > 0 && _scriptTempObj != null)
            {
                for (int i = 0; i < sHoldProp.arraySize; i++)
                {
                    var sholdItem = sHoldProp.GetArrayElementAtIndex(i);
                    var monoScript = sholdItem.FindPropertyRelative("monoScript");
                    if (monoScript.objectReferenceValue == null)
                    {
                        using (var hor = new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PropertyField(monoScript);
                            if (GUILayout.Button("-", GUILayout.Width(20)))
                            {
                                sHoldProp.DeleteArrayElementAtIndex(i);
                                return;
                            }
                        }
                    }
                    else
                    {
                        var script = monoScript.objectReferenceValue as MonoScript;
                        SerializedObject obj = null;

                        if (!monoTemp.ContainsKey(script))
                        {
                            Type type = script.GetClass();
                            Component monoObj = null;
                            if (_node.obj.item != null)
                            {
                                monoObj = _scriptTempObj.GetComponent(type);
                                if (monoObj == null)
                                {
                                    monoObj = _scriptTempObj.AddComponent(type);
                                }
                            }
                            obj = new SerializedObject(monoObj as MonoBehaviour);
                            monoTemp.Add(script, obj);
                        }
                        else
                        {
                            obj = monoTemp[script];
                        }

                        obj.Update();
                        SerializedProperty iterator = obj.GetIterator();
                        bool enterChildren = true;
                        while (iterator.NextVisible(enterChildren))
                        {
                            if ("m_Script" == iterator.propertyPath)
                            {
                                using (var hor = new EditorGUILayout.HorizontalScope())
                                {
                                    EditorGUI.BeginDisabledGroup(true);
                                    EditorGUILayout.PropertyField(iterator);
                                    EditorGUI.EndDisabledGroup();
                                    if (GUILayout.Button("-", GUILayout.Width(20)))
                                    {
                                        sHoldProp.DeleteArrayElementAtIndex(i);
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                            }

                            enterChildren = false;
                        }
                        obj.ApplyModifiedProperties();
                    }
                    EditorGUILayout.SelectableLabel("----------------------------------------------------------------------");
                }
            }

        }
        private void CreateHoldAndLoadInformation()
        {
            monoTemp.Clear();
            _scriptTempObj = new GameObject();
            _scriptTempObj.hideFlags = HideFlags.HideInHierarchy;
            foreach (var item in _node.sHolds)
            {
                var monoType = item.monoScript.GetClass();
                var script = _scriptTempObj.GetComponent(monoType);
                if (script == null)
                {
                    script = _scriptTempObj.AddComponent(monoType);
                }
                PGUtility.InistallVariableToBehaiver(item.variables, script);
                monoTemp.Add(item.monoScript, new SerializedObject(script));
            }
        }
        private void OnDisable()
        {
            if (_scriptTempObj != null)
            {
                //保存信息
                foreach (var item in monoTemp)
                {
                    var classType = item.Key.GetClass();
                    var component = _scriptTempObj.GetComponent(classType);
                    var holder = _node.sHolds.Find(x => x.monoScript == item.Key);
                    if (component != null && holder != null)
                    {
                        PGUtility.AnalysisVariableFromComponent(component, holder.variables);
                    }
                }
                DestroyImmediate(_scriptTempObj);
            }
        }
    }
}
