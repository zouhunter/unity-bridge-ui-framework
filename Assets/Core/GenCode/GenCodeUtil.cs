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
using System;
using UnityEditor;
using System.CodeDom;
using System.Linq;
using BridgeUI.Model;
using System.Reflection;


namespace BridgeUI
{
    public static class GenCodeUtil
    {
        /// <summary>
        /// 按顺序加载组件
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Model.TypeRecod[] SortComponent(GameObject target)
        {
            var components = new List<Component>();
            components.AddRange(target.GetComponents<Component>());
            var list = new List<Model.TypeRecod>();
            var types = new System.Type[] {
                typeof(ScrollRect),
                typeof(InputField),
                typeof(Dropdown),
                typeof(Button),
                typeof(Toggle),
                typeof(Image),
                typeof(Text),
                typeof(Slider),
                typeof(HorizontalLayoutGroup),
                typeof(VerticalLayoutGroup),
                typeof(LayoutElement),
                typeof(RectTransform),
                typeof(GameObject),
            };
            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (components.Find(x => x.GetType() == type))
                {
                    list.Add(new Model.TypeRecod(type));
                }
            }
            return list.ToArray();
        }


        public static UICoder LoadUICoder(GameObject prefab)
        {
            UICoder coder = new UICoder();
            var oldScript = prefab.GetComponent<PanelBase>();
            if (oldScript != null)
            {
                var path = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(oldScript));
                coder.Load(System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8));
            }
            var nameSpace = TryAddNameSpace(coder.unit);
            CodeTypeDeclaration classItem = TryInitClass(nameSpace, prefab);
            return coder;
        }

        private static CodeTypeDeclaration TryInitClass(CodeNamespace nameSpace, GameObject prefab)
        {
            var className = prefab.name;
            if (nameSpace.Types.Count == 0)
            {
                var type = new CodeTypeDeclaration(prefab.name);
                type.IsPartial = true;
                type.BaseTypes.Add(new CodeTypeReference("PanelBase"));
                nameSpace.Types.Add(type);
            }
            return nameSpace.Types[0];
        }

        private static CodeNamespace TryAddNameSpace(CodeCompileUnit unit)
        {
            if (unit.Namespaces.Count == 0)
            {
                unit.Namespaces.Add(new CodeNamespace(""));
            }
            else
            {
                unit.Namespaces[0].Name = "";
            }

            string[] assebles = {
                "UnityEngine",
                "UnityEngine.UI",
                "System.Collections",
                "System.Collections.Generic",
                "BridgeUI"
            };

            foreach (var item in assebles)
            {
                unit.Namespaces[0].Imports.Add(new CodeNamespaceImport(item));
            }

            return unit.Namespaces[0];

        }
        public static void CreateComponent(GameObject go, List<ComponentItem> components, UICoder uiCoder)
        {
            TryAddInfoToUnit(uiCoder.unit.Namespaces[0].Types[0], components);
            var path = AssetDatabase.GetAssetPath(go);
            var scriptPath = path.Replace(".prefab", ".cs");
            System.IO.File.WriteAllText(scriptPath, uiCoder.Compile());
            AssetDatabase.Refresh();
            string className = go.name;
            var type = typeof(BridgeUI.PanelBase).Assembly.GetType(className);
            if (type != null)
            {
                if (go.GetComponent(type) == null)
                {
                    go.AddComponent(type);
                }
            }
        }

        private static void TryAddInfoToUnit(CodeTypeDeclaration codeClass, List<ComponentItem> components)
        {
            foreach (var item in components)
            {
                var fieldName = string.Format("m_" + item.name);
                var field = SuarchField(codeClass.Members, fieldName);
                if (field == null)
                {
                    field = new CodeMemberField();
                    codeClass.Members.Add(field);
                }
                field.Type = new CodeTypeReference(item.componentType, CodeTypeReferenceOptions.GenericTypeParameter);
                field.Attributes = MemberAttributes.Private;
                if (field.CustomAttributes.SuarchAttribute("SerializeField") == null) {
                    field.CustomAttributes.Add(new CodeAttributeDeclaration("SerializeField"));
                }
                field.Name = fieldName;
            }
        }
        /// <summary>
        /// 查看字段
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        private static CodeMemberField SuarchField(this CodeTypeMemberCollection collection, string Name)
        {
            foreach (var item in collection)
            {
                if (item is CodeMemberField)
                {
                    if ((item as CodeMemberField).Name == Name)
                    {
                        return (item as CodeMemberField);
                    }
                }
            }
            return null;
        }

        public static void AnalysisComponent(GameObject prefab, List<ComponentItem> components)
        {
            var component = prefab.GetComponent<PanelBase>();
            if(component == null)
            {
                EditorApplication.Beep();
                return;
            }

            var fields = component.GetType().GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if(typeof(MonoBehaviour).IsAssignableFrom(field.FieldType))
                {
                    var compItem = components.Find(x => "m_" + x.name == field.Name || x.name == field.Name);

                    if (compItem == null) {
                        compItem = new ComponentItem();
                        compItem.name = field.Name.Replace("m_","");
                        components.Add(compItem);
                    }
                    var value = field.GetValue(component);
                    if (value != null)
                    {
                        if(field.FieldType == typeof(GameObject))
                        {
                            compItem.target = value as GameObject;
                        }
                        else
                        {
                            compItem.target = (value as MonoBehaviour).gameObject;
                        }
                        compItem.Update();
                    }
                }
            }
        }

        /// <summary>
        /// 查找属性
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        private static CodeAttributeDeclaration SuarchAttribute(this CodeAttributeDeclarationCollection collection, string Name)
        {
            foreach (var item in collection)
            {
                if (item is CodeAttributeDeclaration)
                {
                    if ((item as CodeAttributeDeclaration).Name == Name)
                    {
                        return (item as CodeAttributeDeclaration);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 快速进行控件绑定
        /// </summary>
        /// <param name="go"></param>
        /// <param name="components"></param>
        public static void BindingUI(GameObject go, List<ComponentItem> components)
        {
            if(go == null || go.GetComponent<PanelBase>() == null)
            {
                EditorApplication.Beep();
                return;
            }
            var behaiver = go.GetComponent<PanelBase>();
            foreach (var item in components)
            {
                var filedName = "m_" + item.name;
                UnityEngine.Object obj = item.target;
                if(item.componentType != typeof(GameObject))
                {
                    obj = item.target.GetComponent(item.componentType);
                }
                behaiver.GetType().InvokeMember(filedName,
                                BindingFlags.SetField |
                                BindingFlags.Instance |
                                BindingFlags.NonPublic,
                                null, behaiver, new object[] { obj }, null, null, null);
            }
        }


        [InitializeOnLoadMethod]
        public static void HandleOnReImport()
        {

        }

    }
}