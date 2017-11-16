using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Reflection;
using System;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace PrefabGenerate
{
    public static class PGUtility
    {
        public const string Menu_PGWindow = "Window/Prefab-Gen";

        public static void GenPrefabWithBundleName(string path, string assetBundleName, GameObject obj)
        {
            GenPrefab(path, obj);
            SetAssetBundleName(path, assetBundleName);
        }
        public static void SetAssetBundleName(string path, string assetBundleName)
        {
            if (!FileUtility.IsFileExist(path)) return;
            AssetImporter importer = UnityEditor.AssetImporter.GetAtPath(path);
            importer.assetBundleName = assetBundleName;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public static GameObject GenPrefab(string path, GameObject obj)
        {
            FileUtility.InitFileDiractory(path);
            GameObject pfb = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (pfb == null)
            {
                pfb = PrefabUtility.CreatePrefab(path, obj, UnityEditor.ReplacePrefabOptions.ConnectToPrefab);
            }
            else
            {
                pfb = PrefabUtility.ReplacePrefab(obj, pfb, ReplacePrefabOptions.ConnectToPrefab);
            }
            return pfb;
        }
        public static Object TryParent(Object instence)
        {
            if (instence == null) return null;
            var parent = PrefabUtility.GetPrefabParent(instence);
            return parent != null ? parent : instence;
        }
        public static bool ImportAnim(GameObject model, ModelImporterAnimationType type)
        {
            bool haveAnim = true;
            ModelImporter modleImporter = (ModelImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(model));
            modleImporter.animationType = type;
            modleImporter.animationWrapMode = WrapMode.Once;

            if (modleImporter.defaultClipAnimations.Length > 0)
            {
                foreach (var item in modleImporter.defaultClipAnimations)
                {
                    item.wrapMode = WrapMode.Default;
                }
            }

            if (modleImporter.animationType != ModelImporterAnimationType.None)
            {
                if (modleImporter.defaultClipAnimations != null && modleImporter.defaultClipAnimations.Length == 0)
                {
                    modleImporter.animationType = ModelImporterAnimationType.None;
                    haveAnim = false;
                }
            }
            else
            {
                haveAnim = false;
            }
            modleImporter.SaveAndReimport();
            return haveAnim;
        }
        /// <summary>
        /// 将MonoScript上的数据保存到变量记录器
        /// </summary>
        /// <param name="behaiver"></param>
        /// <param name="variables"></param>
        public static void AnalysisVariableFromComponent(Component behaiver, List<Variable> variables)
        {
            variables.Clear();
            var type = behaiver.GetType();
            FieldInfo[] publicFields = type.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
            foreach (var item in publicFields)
            {
                var variable = CreateVariable(item, behaiver);
                variable.isPrivate = false;
                variables.Add(variable);
            }
            FieldInfo[] privateFields = type.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var item in privateFields)
            {
                var attrs = item.GetCustomAttributes(false);
                if (attrs != null && attrs.Length > 0 && Array.Find(attrs, x => x is SerializeField) != null)
                {
                    var variable = CreateVariable(item, behaiver);
                    variable.isPrivate = true;
                    variables.Add(variable);
                }
            }
        }

        /// <summary>
        /// 解析保存的数据信息
        /// </summary>
        /// <param name="variables"></param>
        /// <param name="behaiver"></param>
        public static void InistallVariableToBehaiver(List<Variable> variables, Component behaiver)
        {
            var type = behaiver.GetType();

            foreach (var item in variables)
            {
                var data = LoadVariable(item);
                if (item.isPrivate)
                {
                    type.InvokeMember(item.name, BindingFlags.SetField | BindingFlags.Instance | BindingFlags.NonPublic, null, behaiver, new object[] { data }, null, null, null);
                }
                else
                {
                    type.InvokeMember(item.name, BindingFlags.SetField | BindingFlags.Instance | BindingFlags.Public, null, behaiver, new object[] { data }, null, null, null);
                }
            }
        }

        /// <summary>
        /// 反射生成Variable
        /// </summary>
        /// <param name="item"></param>
        /// <param name="behaiver"></param>
        /// <returns></returns>
        private static Variable CreateVariable(FieldInfo item, Component behaiver)
        {
            var assembleUnity = typeof(Vector2).Assembly;
            var assembleInt = typeof(int).Assembly;

            Variable var = new Variable();
            var.name = item.Name;
            var.type = item.FieldType.ToString();
            var.assemble = item.FieldType.Assembly.ToString();

            if (item.FieldType.IsSubclassOf(typeof(Object)))
            {
                var obj = item.GetValue(behaiver) as Object;
                var path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path))
                {
                    var.value = AssetDatabase.AssetPathToGUID(path);
                }
            }
            else if (item.FieldType.Assembly == assembleUnity)
            {
                var.value = JsonUtility.ToJson(item.GetValue(behaiver));
            }
            else if(item.FieldType.Assembly == assembleInt)
            {
                var.value = Convert.ToString(item.GetValue(behaiver));
            }
            return var;
        }

        private static object LoadVariable(Variable item)
        {
            var assembleUnity = typeof(Vector2).Assembly;
            var assembleInt = typeof(int).Assembly;
            Type dataType = Assembly.Load(item.assemble).GetType(item.type);
            object data = null;
            if (dataType.IsSubclassOf(typeof(Object)))
            {
                if (!string.IsNullOrEmpty(item.value))
                {
                    var path = AssetDatabase.GUIDToAssetPath(item.value);
                    if (!string.IsNullOrEmpty(path))
                    {
                        data = AssetDatabase.LoadAssetAtPath<Object>(path);
                    }
                }
            }
            else if (item.assemble == assembleUnity.ToString())
            {
                data = JsonUtility.FromJson(item.value, dataType);
            }
            else if(item.assemble == assembleInt.ToString())
            {
                data = Convert.ChangeType(item.value, dataType);
            }
            return data;
        }
    }
}
