using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace NodeGraph
{

    public class UserDefineUtility
    {
        private static List<Type> _controllerTypes;
        public static List<Type> CustomControllerTypes
        {
            get
            {
                if (_controllerTypes == null)
                {
                    _controllerTypes = BuildControllerTypeList();
                }
                return _controllerTypes;
            }
        }
        private static List<Type> BuildControllerTypeList()
        {
            var list = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var nodes = assembly.GetTypes()
                    .Where(t => t != typeof(NodeGraphController))
                    .Where(t => typeof(NodeGraphController).IsAssignableFrom(t));
                list.AddRange(nodes);
            }
            return list;
        }

        internal static NodeGraphController CreateController(NodeGraph.DataModel.ConfigGraph graph)
        {
            var type = CustomControllerTypes.Find(x => x.Name == graph.ControllerType);
            if (type != null)
            {
                var ctrl = System.Activator.CreateInstance(type);
                var gctrl =  ctrl as NodeGraphController;
                gctrl.TargetGraph = graph;
                return gctrl;
            }
            else
            {
                return null;
            }
        }
        private static Dictionary<Type, Type> userDrawer;
        internal static object GetUserDrawer(Type type)
        {
            InitDrawerTypes();
            if (userDrawer.ContainsKey(type))
            {
                var drawer = Activator.CreateInstance(userDrawer[type]);
                return drawer;
            }
            return null;
        }
        private static void InitDrawerTypes()
        {
            if (userDrawer == null)
            {
                userDrawer = new Dictionary<Type, Type>();
                var allDrawer = new List<Type>();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var nodes = assembly.GetTypes()
                        .Where(t => t != typeof(NodeDrawer) && t != typeof(ConnectionDrawer))
                        .Where(t => typeof(NodeDrawer).IsAssignableFrom(t) || typeof(ConnectionDrawer).IsAssignableFrom(t));
                    allDrawer.AddRange(nodes);
                }
                foreach (var type in allDrawer)
                {
                    CustomNodeGraphDrawer attr = type.GetCustomAttributes(typeof(CustomNodeGraphDrawer), false).FirstOrDefault() as CustomNodeGraphDrawer;

                    if (attr != null)
                    {
                        userDrawer.Add(attr.targetType, type);
                    }
                }
            }
        }

        /// <summary>
        /// 分析类中的参数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldInfos"></param>
        public static void GetNeedSerializeField(object instence, Dictionary<object, bool> toggleDic, Dictionary<object, List<FieldInfo>> fieldDic)
        {
            toggleDic.Add(instence, true);
            var type = instence.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var item in fields)
            {
                if (!IsFieldNeed(item)) continue;

                if (item.FieldType.IsValueType || item.FieldType.IsEnum || item.FieldType.IsClass)
                {
                    if (fieldDic.ContainsKey(instence))
                    {
                        fieldDic[instence].Add(item);
                    }
                    else
                    {
                        fieldDic[instence] = new List<FieldInfo>() { item };
                    }
                }

                if (item.FieldType.IsClass)
                {
                    GetNeedSerializeField(item.GetValue(instence), toggleDic, fieldDic);
                }
            }
        }


        public static object DrawClassObject(object classItem, Dictionary<object, bool> toggleDic, Dictionary<object, List<FieldInfo>> fieldDic)
        {
            EditorGUI.indentLevel++;
            if (GUILayout.Button(classItem.GetType().Name, EditorStyles.boldLabel))
            {
                toggleDic[classItem] = !toggleDic[classItem];
            }
            if (toggleDic[classItem])
            {
                foreach (var item in fieldDic[classItem])
                {
                    if (item.FieldType.IsClass && (item.FieldType != typeof(string)))
                    {
                        DrawClassObject(item.GetValue(classItem), toggleDic, fieldDic);
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(item.Name, GUILayout.Width(100));
                        item.SetValue(classItem, DrawField(item.GetValue(classItem)));
                        EditorGUILayout.EndHorizontal();
                    }

                }
            }

            return classItem;
        }

        public static object DrawField(object data)
        {
            if (data is int)
            {
                data = EditorGUILayout.IntField(Convert.ToInt32(data));
            }
            else if (data is bool)
            {
                data = EditorGUILayout.Toggle(Convert.ToBoolean(data));
            }
            else if (data is float || data is double)
            {
                data = EditorGUILayout.FloatField(float.Parse(data.ToString()));
            }
            else if (data is string)
            {
                data = EditorGUILayout.TextField(data.ToString());
            }
            else if (data is Color)
            {
                data = EditorGUILayout.ColorField((Color)data);
            }
            else if (data is Enum)
            {
                data = EditorGUILayout.EnumPopup((Enum)data);
            }
            else if (data is Vector2)
            {
                data = EditorGUILayout.Vector2Field("", (Vector2)data);
            }
            else if (data is Vector3)
            {
                data = EditorGUILayout.Vector3Field("", (Vector3)data);
            }
            else if (data is Vector4)
            {
                data = EditorGUILayout.Vector4Field("", (Vector4)data);
            }
            else if (data is Rect)
            {
                data = EditorGUILayout.RectField("", (Rect)data);
            }
            return data;
        }

        protected void DrawSerializedObject(SerializedObject serializedObject)
        {
            serializedObject.Update();
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                EditorGUI.BeginDisabledGroup("m_Script" == iterator.propertyPath);
                EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                EditorGUI.EndDisabledGroup();
                enterChildren = false;
            }
            serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 判断寡字段能否序列化
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static bool IsFieldNeed(FieldInfo fieldInfo)
        {
            var type = fieldInfo.FieldType;

            //排除字典
            if (type.IsGenericType && type.Name.Contains("Dictionary`"))
            {
                return false;
            }

            //排除非公有变量
            if (fieldInfo.Attributes != FieldAttributes.Public)
            {
                var attrs = fieldInfo.GetCustomAttributes(false);
                if (attrs.Length == 0 || (attrs.Length > 0 && Array.Find(attrs, x => x is SerializeField) == null))
                {
                    return false;
                }
            }

            //排出接口
            if (type.IsInterface)
            {
                return false;
            }

            //修正type
            if (type.IsArray || type.IsGenericType)
            {
                if (type.IsGenericType)
                {
                    type = type.GetGenericArguments()[0];
                }
                else
                {
                    type = type.GetElementType();
                }
            }

            //排出修正后的接口
            if (type.IsInterface)
            {
                return false;
            }

            //排除不能序列化的类
            if (type.IsClass)
            {
                if (!type.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    var atts = type.GetCustomAttributes(false);
                    var seri = Array.Find(atts, x => x is System.SerializableAttribute);
                    if (seri == null)
                    {
                        return false;
                    }
                }
            }

            //排除内置变量
            if (fieldInfo.Name.Contains("k__BackingField"))
            {
                return false;
            }

            return true;
        }

    }

}