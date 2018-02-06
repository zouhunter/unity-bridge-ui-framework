#region statement
/************************************************************************************* 
    * 作    者：       zouhunter
    * 时    间：       2018-02-02 
    * 详    细：       1.支持枚举、模型、结构和继承等类的模板。
                       2.支持快速创建通用型UI界面脚本
                       3.支持自定义模板类
                       4.自动生成作者、创建时间、描述等功能
                       5.支持工程同步（EditorPrefer）
   *************************************************************************************/
#endregion

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.CodeDom;
using System.IO;
using System.Text;
using NUnit.Framework.Constraints;
using NUnit.Framework;

namespace EditorTools
{
    #region Window
    /// <summary>
    /// 一个创建脚本模板的窗口
    /// </summary>
    public class TempScriptWindow : EditorWindow
    {
        [MenuItem("Window/TempScriptWindow")]
        static void Open()
        {
            var window = TempScriptHelper.GetWindow();
            window.wantsMouseMove = true;
        }


        [SerializeField]
        public List<ScriptTemplate> templates;
        ScriptTemplate currentTemplates { get { if (templates != null && templates.Count > currentIndex) return templates[currentIndex]; return null; } }
        private MonoScript script;
        [SerializeField]
        private bool isSetting;
        [SerializeField]
        private string authorName;
        [SerializeField]
        private string[] templateNames;
        [SerializeField]
        private int currentIndex;
        private Vector2 scrollPos;
        [SerializeField]
        private string[] templateType;
        private bool showRule;
        private string codeRule;
        private void OnEnable()
        {
            InitEnviroment();
        }

        private void OnDisable()
        {
            if (templates != null)
            {
                foreach (var item in templates)
                {
                    item.SaveToJson();
                }
            }
            EditorUtility.SetDirty(this);
            TempScriptHelper.SaveWindow(this);
        }

        private void OnGUI()
        {
            DrawHead();
            if (isSetting)
            {
                //绘制设置信息
                DrawSettings();
            }
            else
            {
                if (templates == null)
                {
                    Debug.Log("template == null");
                    templates = new List<ScriptTemplate>();
                }

                if (templates.Count == 0)
                {
                    Debug.Log("AddTemplates");
                    AddTemplates();
                }

                currentIndex = GUILayout.Toolbar(currentIndex, templateNames);
                using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = scroll.scrollPosition;

                    if (currentTemplates != null)
                    {
                        if (currentTemplates.GetType().FullName != currentTemplates.type)
                        {
                            templates[currentIndex] = TempScriptHelper.LoadFromJson(currentTemplates);
                        }

                        currentTemplates.OnBodyGUI();

                        if (currentTemplates.GetType().FullName == typeof(ScriptTemplate).FullName)
                        {
                            if (templateType.Length > currentIndex)
                            {
                                var type = Type.GetType(templateType[currentIndex]);
                                if (type != null)
                                {
                                    templates[currentIndex] = Activator.CreateInstance(type) as ScriptTemplate;
                                    Debug.Log("create new:" + currentTemplates.GetType());
                                }
                                else
                                {
                                    Debug.LogFormat("{0} missing: clear templates", currentTemplates.GetType().FullName);
                                    templates.Clear();
                                }

                            }
                            else
                            {
                                Debug.Log("unknow err: clear templates");
                                templates.Clear();
                            }
                        }
                    }
                    else
                    {
                        templates.Clear();
                        Debug.Log("templates.Count <= currentIndex");
                    }
                }
                if (currentTemplates != null)
                {
                    currentTemplates.OnFootGUI();
                }

            }

        }
        private void InitEnviroment()
        {
            if (script == null) script = MonoScript.FromScriptableObject(this);
            showRule = TempScriptHelper.GetRuleShowState();
            if (string.IsNullOrEmpty(codeRule)) codeRule = TempScriptHelper.GetCodeRule();
            if (string.IsNullOrEmpty(authorName)) authorName = TempScriptHelper.GetAuthor();
            if (string.IsNullOrEmpty(authorName))
            {
                isSetting = true;
            }
        }
        private void AddTemplates()
        {
            var assemble = this.GetType().Assembly;
            var allTypes = assemble.GetTypes();
            foreach (var item in allTypes)
            {
                if (item.IsSubclassOf(typeof(ScriptTemplate)) && !item.IsAbstract)
                {
                    var template = Activator.CreateInstance(item);
                    templates.Add(template as ScriptTemplate);
                }
            }

            foreach (var item in templates)
            {
                item.OnEnable();
            }
            templateNames = templates.ConvertAll<string>(x => x.Name).ToArray();
            templateType = templates.ConvertAll<string>(x => x.GetType().FullName).ToArray();
        }
        public void LoadOldTemplates()
        {
            for (int i = 0; i < templates.Count; i++)
            {
                if (templates[i] == null)
                {
                    Debug.LogFormat("templates[{0}] == null",i);
                    templates.Clear();
                    return;
                }

                var newitem = TempScriptHelper.LoadFromJson(templates[i]);

                if (newitem == null)
                {
                    Debug.LogFormat("newitem[{0}] == null", i);
                    templates.Clear();
                    return;
                }
                templates[i] = newitem;
            }
            templateNames = templates.ConvertAll<string>(x => x.Name).ToArray();
            templateType = templates.ConvertAll<string>(x => x.GetType().FullName).ToArray();
        }
        private void DrawHead()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.ObjectField(script, typeof(MonoScript), false);
                if (!isSetting && GUILayout.Button("setting", EditorStyles.miniButtonRight, GUILayout.Width(60)))
                {
                    isSetting = true;
                }
                else if (isSetting && GUILayout.Button("confer", EditorStyles.miniButtonRight, GUILayout.Width(60)) && !string.IsNullOrEmpty(authorName))
                {
                    isSetting = false;
                }
            }

            if (!isSetting && GUILayout.Button("Clear", EditorStyles.toolbarButton))
            {
                if (currentTemplates != null)
                    currentTemplates.headerInfo = new TempScriptHeader();
                templates.Clear();
            }
        }
        private void DrawSettings()
        {
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.SelectableLabel("作者:", EditorStyles.miniLabel, GUILayout.Width(60));
                authorName = EditorGUILayout.TextField(authorName);
                if (EditorGUI.EndChangeCheck())
                {
                    if (!string.IsNullOrEmpty(authorName))
                    {
                        TempScriptHelper.SaveAuthor(authorName);
                    }
                }
            }
            EditorGUI.BeginChangeCheck();
            showRule = EditorGUILayout.ToggleLeft("在生成的代码末尾显示规范:（熟悉后可关闭此功能）", showRule);
            if (EditorGUI.EndChangeCheck())
            {
                TempScriptHelper.SetRuleShowState(showRule);
            }

            if (showRule)
            {
                using (var scrop = new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = scrop.scrollPosition;
                    EditorGUILayout.TextArea(codeRule);
                }
            }
        }
    }
    #endregion

    #region Tools
    /// <summary>
    /// 任何脚本的头
    /// </summary>
    [System.Serializable]
    public class TempScriptHeader
    {
        public string author;
        public string time;
        public string description;
        public List<string> detailInfo = new List<string>();
        public string scriptName;
        public string nameSpace;

        public void Update()
        {
            author = TempScriptHelper.GetAuthor();
            time = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }
    }

    /// <summary>
    /// 静态工具类
    /// </summary>
    public static class TempScriptHelper
    {
        private const string prefer_key = "temp_script_autor_name";
        private const string prefer_window = "temp_script_window";
        private const string code_rule_show = "temp_script_code_rule_show";

        public static void SaveAuthor(string author)
        {
            EditorPrefs.SetString(prefer_key, author);
        }

        public static string GetAuthor()
        {
            return EditorPrefs.GetString(prefer_key);
        }

        public static void SaveWindow(TempScriptWindow window)
        {
            var json = JsonUtility.ToJson(window);
            EditorPrefs.SetString(prefer_window, json);
        }
        public static TempScriptWindow GetWindow()
        {
            var window = EditorWindow.GetWindow<TempScriptWindow>();
            if (EditorPrefs.HasKey(prefer_key))
            {
                var json = EditorPrefs.GetString(prefer_window);
                JsonUtility.FromJsonOverwrite(json, window);
                window.LoadOldTemplates();
                return window;
            }
            return window;
        }
        internal static ScriptTemplate LoadFromJson(ScriptTemplate old)
        {
            if (!string.IsNullOrEmpty(old.type) && old.GetType().FullName != old.type)
            {
                var type = Type.GetType(old.type);
                if (type != null)
                {
                    var temp = Activator.CreateInstance(type);
                    JsonUtility.FromJsonOverwrite(old.json, temp);
                    return temp as ScriptTemplate;
                }
                else
                {
                    return old;
                }
            }
            else
            {
                old.type = old.GetType().FullName;
                return old;
            }
        }

        internal static void SetRuleShowState(bool enabled)
        {
            PlayerPrefs.SetInt(code_rule_show, enabled ? 1 : 0);
        }
        internal static bool GetRuleShowState()
        {
            if (PlayerPrefs.HasKey(code_rule_show))
            {
                return PlayerPrefs.GetInt(code_rule_show) == 1;
            }
            return true;
        }
        /*
         1.私有字段：_field,m_field
         2.公有字段：field
         2.属性：Property
         3.常量：CONST
         4.静态变量：Field,Property
             */
        internal static string GetCodeRule()
        {
            return @"#region 代码规范
/*************************************************************************************
        【变量命名】：
         1.私有字段：_field,m_field
         2.公有字段：field
         2.属性：Property
         3.常量：CONST
         4.静态变量：Field,Property
**************************************************************************************/
#endregion
";
        }

        internal static void QuickCreateTemplate<T>() where T : ScriptTemplate, new()
        {
            EditorApplication.ExecuteMenuItem("Assets/Create/C# Script");
            EditorApplication.update = () =>
            {
                if (Selection.activeObject)
                {
                    var path = AssetDatabase.GetAssetPath(Selection.activeObject);
                    var temp = new T();
                    temp.headerInfo.scriptName = Path.GetFileNameWithoutExtension(path);
                    var dir = Path.GetDirectoryName(path);
                    var scr = temp.CreateScript();
                    temp.SaveToFile(dir, scr);
                    EditorApplication.update = null;
                }
            };
        }
    }
    #endregion

    #region Templates
    /// <summary>
    /// 代码创建模板的模板
    /// </summary>
    [System.Serializable]
    public class ScriptTemplate
    {
        [System.Serializable]
        public class FieldItem
        {
            public string type;
            public string elementName;
            public string comment;
        }
        [System.Serializable]
        public class PropertyItem
        {
            public string type;
            public string elementName;
            public string comment;
            public bool get;
            public bool set;
            public PropertyItem()
            {
                get = set = true;
            }
        }
        [SerializeField]
        protected List<FieldItem> fields = new List<FieldItem>();
        [SerializeField]
        protected List<PropertyItem> propertys = new List<PropertyItem>();
        [SerializeField]
        protected List<string> imports = new List<string>();

        public string json;
        public string type;
        public TempScriptHeader headerInfo = new TempScriptHeader();
        public string path;
        public virtual string Name { get { return null; } }
        private ReorderableList detailList;

        public virtual void OnBodyGUI() { }
        public virtual void OnFootGUI()
        {
            if (detailList == null)
            {
                InitDetailList();
            }

            if (detailList.list != headerInfo.detailInfo)
            {
                detailList.list = headerInfo.detailInfo;
            }

            using (var horm = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Namespace", GUILayout.Width(70));
                headerInfo.nameSpace = EditorGUILayout.TextField(headerInfo.nameSpace, GUILayout.Width(60));
                EditorGUILayout.LabelField("Type", GUILayout.Width(40));
                headerInfo.scriptName = EditorGUILayout.TextField(headerInfo.scriptName, GUILayout.Width(60));
                EditorGUILayout.LabelField("简介", GUILayout.Width(40));
                headerInfo.description = EditorGUILayout.TextField(headerInfo.description);

                if (GUILayout.Button("Load", EditorStyles.miniButtonRight, GUILayout.Width(60)))
                {
                    OnLoadButtonClicked();
                }
            }
            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                using (var vertical = new EditorGUILayout.VerticalScope())
                {
                    detailList.DoLayoutList();
                }
                using (var vertical = new EditorGUILayout.VerticalScope(GUILayout.Width(60)))
                {
                    if (GUILayout.Button("Create", EditorStyles.miniButtonRight, GUILayout.Height(60)))
                    {
                        OnCreateButtonClicked();
                    }
                }
            }
        }

        public virtual void OnEnable()
        {
            type = this.GetType().FullName;
        }

        public string CreateScript()
        {
            var ns = CreateNameSpace();
            if (ns == null) return null;
            var nsstr = ComplieNameSpaceToString(ns);
            if (string.IsNullOrEmpty(nsstr)) return null;

            var scriptStr = GetHeader() + nsstr + GetFooter();

            if (string.IsNullOrEmpty(scriptStr))
            {
                EditorUtility.DisplayDialog("生成失败", "请看日志！", "确认");
                return null;
            }

            return scriptStr;


        }
        public void SaveToJson()
        {
            json = null;
            json = JsonUtility.ToJson(this);
            if (string.IsNullOrEmpty(type)){
                type = this.GetType().FullName;
            }
        }
        protected void InitDetailList()
        {
            detailList = new ReorderableList(headerInfo.detailInfo, typeof(string), true, false, true, true);
            detailList.onAddCallback += (x) => { headerInfo.detailInfo.Add(""); };
            detailList.drawHeaderCallback = (x) => { EditorGUI.LabelField(x, "详细信息"); };
            detailList.drawElementCallback += (x, y, z, w) => { headerInfo.detailInfo[y] = EditorGUI.TextField(x, headerInfo.detailInfo[y]); };
        }

        protected void DrawFieldItem(Rect rect, FieldItem dataItem, bool haveType)
        {
            if (haveType)
            {
                var rect01 = new Rect(rect.x, rect.y, rect.width * 0.2f, EditorGUIUtility.singleLineHeight);
                var typeRect = new Rect(rect.x + 0.2f * rect.width, rect.y, rect.width * 0.1f, EditorGUIUtility.singleLineHeight);
                var rect02 = new Rect(rect.x + rect.width * 0.3f, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
                var commentRect = new Rect(rect.x + 0.6f * rect.width, rect.y, rect.width * 0.1f, EditorGUIUtility.singleLineHeight);
                var rect03 = new Rect(rect.x + rect.width * 0.7f, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);

                dataItem.elementName = EditorGUI.TextField(rect01, dataItem.elementName);
                EditorGUI.LabelField(typeRect, "Type");
                dataItem.type = EditorGUI.TextField(rect02, dataItem.type);
                EditorGUI.LabelField(commentRect, "Comment");
                dataItem.comment = EditorGUI.TextField(rect03, dataItem.comment);
            }
            else
            {
                var left = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
                var right = new Rect(rect.x + rect.width * 0.4f, rect.y, rect.width * 0.6f, EditorGUIUtility.singleLineHeight);
                var center = new Rect(rect.x + rect.width * 0.3f, rect.y, rect.width * 0.1f, EditorGUIUtility.singleLineHeight);
                dataItem.elementName = EditorGUI.TextField(left, dataItem.elementName);
                EditorGUI.LabelField(center, "Comment");
                dataItem.comment = EditorGUI.TextField(right, dataItem.comment);
            }

        }
        protected void DrawPropertyItem(Rect rect, PropertyItem propertyItem)
        {
            var rect01 = new Rect(rect.x, rect.y, rect.width * 0.2f, EditorGUIUtility.singleLineHeight);
            var typeRect = new Rect(rect.x + 0.2f * rect.width, rect.y, rect.width * 0.1f, EditorGUIUtility.singleLineHeight);
            var rect02 = new Rect(rect.x + rect.width * 0.3f, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
            var commentRect = new Rect(rect.x + 0.6f * rect.width, rect.y, rect.width * 0.1f, EditorGUIUtility.singleLineHeight);
            var rect03 = new Rect(rect.x + rect.width * 0.7f, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);

            propertyItem.elementName = EditorGUI.TextField(rect01, propertyItem.elementName);
            EditorGUI.LabelField(typeRect, "Type");
            propertyItem.type = EditorGUI.TextField(rect02, propertyItem.type);
            EditorGUI.LabelField(commentRect, "Comment");
            propertyItem.comment = EditorGUI.TextField(rect03, propertyItem.comment);

            var getLabelRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width * 0.1f, EditorGUIUtility.singleLineHeight);
            var getRect = new Rect(rect.x + 0.1f * rect.width, rect.y + EditorGUIUtility.singleLineHeight, rect.width * 0.1f, EditorGUIUtility.singleLineHeight);
            var setLabelRect = new Rect(rect.x + 0.2f * rect.width, rect.y + EditorGUIUtility.singleLineHeight, rect.width * 0.1f, EditorGUIUtility.singleLineHeight);
            var setRect = new Rect(rect.x + 0.3f * rect.width, rect.y + EditorGUIUtility.singleLineHeight, rect.width * 0.1f, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(getLabelRect, "get");
            propertyItem.get = EditorGUI.Toggle(getRect, propertyItem.get);
            EditorGUI.LabelField(setLabelRect, "set");
            propertyItem.set = EditorGUI.Toggle(setRect, propertyItem.set);

        }
        protected string DrawNameSpace(Rect rect, string dataItem)
        {
            var rect1 = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            return EditorGUI.TextField(rect1, dataItem);
        }
        protected virtual CodeTypeDeclaration CreateMainType() { return null; }
        protected CodeNamespace CreateNameSpace()
        {
            var type = CreateMainType();
            if (type == null)
            {
                return null;
            }
            CodeNamespace nameSpace = new CodeNamespace(headerInfo.nameSpace);
            nameSpace.Types.Add(type);
            nameSpace.Imports.AddRange(imports.ConvertAll<CodeNamespaceImport>(x => new CodeNamespaceImport(x)).ToArray());
            return nameSpace;
        }
        protected string GetHeader()
        {
            headerInfo.Update();

            var str1 = "#region statement\r\n" +
            "/*************************************************************************************   \r\n" +
            "    * 作    者：       {0}\r\n" +
            "    * 时    间：       {1}\r\n" +
            "    * 说    明：       ";
            var str2 = "\r\n                       ";
            var str3 = "\r\n* ************************************************************************************/" +
            "\r\n#endregion\r\n";

            var headerStr = string.Format(str1, headerInfo.author, headerInfo.time);
            for (int i = 0; i < headerInfo.detailInfo.Count; i++)
            {
                if (i == 0)
                {
                    headerStr += string.Format("{0}.{1}", i + 1, headerInfo.detailInfo[i]);
                }
                else
                {
                    headerStr += string.Format("{0}{1}.{2}", str2, i + 1, headerInfo.detailInfo[i]);
                }
            }
            headerStr += str3;
            return headerStr;
        }
        protected string ComplieNameSpaceToString(CodeNamespace nameSpace)
        {
            using (Microsoft.CSharp.CSharpCodeProvider cprovider = new Microsoft.CSharp.CSharpCodeProvider())
            {
                StringBuilder fileContent = new StringBuilder();
                var option = new System.CodeDom.Compiler.CodeGeneratorOptions();
                option.BlankLinesBetweenMembers = false;
                using (StringWriter sw = new StringWriter(fileContent))
                {
                    cprovider.GenerateCodeFromNamespace(nameSpace, sw, option);
                }
                return fileContent.ToString();
            }
        }
        protected string GetFooter()
        {
            if (TempScriptHelper.GetRuleShowState())
            {
                var rule = TempScriptHelper.GetCodeRule();
                return rule;
            }
            return null;
        }
        /// <summary>
        /// 点击创建
        /// </summary>
        private void OnCreateButtonClicked()
        {
            if (string.IsNullOrEmpty(headerInfo.scriptName))
            {
                EditorUtility.DisplayDialog("脚本名为空", "请填写代码名称！", "确认");
                return;
            }


            if (string.IsNullOrEmpty(path))
            {
                if (ProjectWindowUtil.IsFolder(Selection.activeInstanceID))
                {
                    path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
                }
                else if (Selection.activeObject != null)
                {
                    var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        path = assetPath.Replace(System.IO.Path.GetFileName(assetPath), "");
                    }
                }
            }
            var scriptStr = CreateScript();
            if (!string.IsNullOrEmpty(scriptStr))
            {
                SaveToFile(path, scriptStr);
            }

        }
        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="scriptStr"></param>
        public void SaveToFile(string path, string scriptStr)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var scriptPath = string.Format("{0}/{1}.cs", path, headerInfo.scriptName);
                System.IO.File.WriteAllText(scriptPath, scriptStr, System.Text.Encoding.UTF8);
                AssetDatabase.Refresh();
            }
            else
            {
                EditorUtility.DisplayDialog("路径不明", "请选中文件夹后重试", "确认");
            }

        }

        /// <summary>
        /// 点击加载代码
        /// </summary>
        private void OnLoadButtonClicked()
        {
            if (!(Selection.activeObject is TextAsset))
            {
                EditorUtility.DisplayDialog("未选中", "请选中需要解析的代码后继续", "确认");
                return;
            }

            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!path.EndsWith(".cs"))
            {
                EditorUtility.DisplayDialog("未选中", "请选中需要解析的代码后继续", "确认");
                return;
            }

            using (var provider = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("CSharp"))
            {
                EditorUtility.DisplayDialog("未开发", ".net 3.5暂无该实现", "确认");

                var fileContent = System.IO.File.ReadAllText(path, Encoding.UTF8);
                using (StringReader sr = new StringReader(fileContent))
                {
                    var nameSpaceUnit = provider.Parse(sr);
                    Debug.Log(nameSpaceUnit);
                }
            }
        }
    }

    public abstract class ClassTemplate : ScriptTemplate
    {
        protected ReorderableList nameSpaceList;
        protected ReorderableList fieldsList;
        protected ReorderableList propertyList;

        public ClassTemplate()
        {
            fieldsList = new ReorderableList(fields, typeof(string));
            fieldsList.onAddCallback += (x) => { fields.Add(new FieldItem()); };
            fieldsList.drawHeaderCallback += (x) => { EditorGUI.LabelField(x, "字段"); };
            fieldsList.drawElementCallback += (x, y, z, w) =>
            {
                DrawFieldItem(x, fields[y], true);
            };

            nameSpaceList = new ReorderableList(imports, typeof(string));
            nameSpaceList.onAddCallback += (x) => { imports.Add(""); };
            nameSpaceList.drawHeaderCallback += (x) => { EditorGUI.LabelField(x, "命名空间"); };
            nameSpaceList.drawElementCallback += (x, y, z, w) =>
            {
                imports[y] = DrawNameSpace(x, imports[y]);
            };

            propertyList = new ReorderableList(propertys, typeof(string));
            propertyList.onAddCallback += (x) => { propertys.Add(new PropertyItem()); };
            propertyList.drawHeaderCallback += (x) => { EditorGUI.LabelField(x, "属性"); };
            propertyList.elementHeightCallback = (x) => { return 2 * EditorGUIUtility.singleLineHeight; };
            propertyList.drawElementCallback += (x, y, z, w) =>
            {
                DrawPropertyItem(x, propertys[y]);
            };

        }

        protected override CodeTypeDeclaration CreateMainType()
        {
            List<CodeMemberField> fields = new List<CodeMemberField>();
            foreach (var item in base.fields)
            {
                CodeMemberField prop = new CodeMemberField();
                prop.Type = new CodeTypeReference(item.type, CodeTypeReferenceOptions.GenericTypeParameter);
                prop.Attributes = MemberAttributes.Public;
                prop.Name = item.elementName;
                prop.Comments.Add(new CodeCommentStatement(item.comment));
                fields.Add(prop);
            }

            List<CodeMemberProperty> propertysMemper = new List<CodeMemberProperty>();
            foreach (var item in propertys)
            {
                CodeMemberProperty prop = new CodeMemberProperty();
                prop.Type = new CodeTypeReference(item.type, CodeTypeReferenceOptions.GenericTypeParameter);
                prop.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                prop.Name = item.elementName;
                prop.HasGet = item.get;
                prop.HasSet = item.set;
                //CodeExpression invokeExpression = new CodePropertyReferenceExpression();
                if(item.get) prop.GetStatements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(null)));
                prop.Comments.Add(new CodeCommentStatement(item.comment));
                propertysMemper.Add(prop);
            }

            CodeTypeDeclaration wrapProxyClass = new CodeTypeDeclaration(headerInfo.scriptName);
            wrapProxyClass.TypeAttributes = System.Reflection.TypeAttributes.Public;
            wrapProxyClass.IsClass = true;
           
            foreach (var field in fields){
                wrapProxyClass.Members.Add(field);
            }
            foreach (var prop in propertysMemper)
            {
                wrapProxyClass.Members.Add(prop);
            }
            return wrapProxyClass;
        }

        public override void OnBodyGUI()
        {
            nameSpaceList.DoLayoutList();
            fieldsList.DoLayoutList();
            propertyList.DoLayoutList();
        }
    }

    public abstract class ChildClassTemplate : ClassTemplate
    {
        protected virtual Type baseType { get { return typeof(object); } }
        protected override CodeTypeDeclaration CreateMainType()
        {
            var wrapProxyClass = base.CreateMainType();
            wrapProxyClass.BaseTypes.Add(new CodeTypeReference(baseType));
            return wrapProxyClass;
        }
    }
    #endregion

    #region Enum
    /// <summary>
    /// 1.枚举类型脚本
    /// </summary>
    [Serializable]
    public class EnumScriptTemplate : ScriptTemplate
    {
        [MenuItem("Assets/Create/C# TempScripts/Enum", priority = 5)]
        static void CreateEnum()
        {
            TempScriptHelper.QuickCreateTemplate<EnumScriptTemplate>();
        }

        public override string Name
        {
            get
            {
                return "Enum";
            }
        }

        private ReorderableList reorderableList;

        public EnumScriptTemplate()
        {
            reorderableList = new ReorderableList(fields, typeof(string));
            reorderableList.onAddCallback += (x) => { fields.Add(new FieldItem()); };
            reorderableList.drawHeaderCallback += (x) => { EditorGUI.LabelField(x, "枚举列表"); };
            reorderableList.drawElementCallback += (x, y, z, w) =>
            {
                DrawFieldItem(x, fields[y], false);
            };
        }

        protected override CodeTypeDeclaration CreateMainType()
        {
            List<CodeMemberField> fields = new List<CodeMemberField>();
            foreach (var item in base.fields)
            {
                CodeMemberField prop = new CodeMemberField();
                prop.Name = item.elementName;
                prop.Comments.Add(new CodeCommentStatement(item.comment));
                fields.Add(prop);
            }

            CodeTypeDeclaration wrapProxyClass = new CodeTypeDeclaration(headerInfo.scriptName);
            wrapProxyClass.TypeAttributes = System.Reflection.TypeAttributes.Public;
            wrapProxyClass.IsEnum = true;

            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement(headerInfo.description, true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            foreach (var field in fields)
            {
                wrapProxyClass.Members.Add(field);
            }

            return wrapProxyClass;
        }
        public override void OnBodyGUI()
        {
            reorderableList.DoLayoutList();
        }
    }
    #endregion

    #region DataModel
    /// <summary>
    /// 2.数据模拟类
    /// </summary>
    [Serializable]
    public class DataModelTemplate : ClassTemplate
    {
        [MenuItem("Assets/Create/C# TempScripts/Normal", priority = 5)]
        static void CreateModel()
        {
            TempScriptHelper.QuickCreateTemplate<DataModelTemplate>();
        }
        public override string Name
        {
            get
            {
                return "Normal";
            }
        }
        public DataModelTemplate()
        {
            imports.AddRange(new string[] { "System", "UnityEngine" });
        }
        protected override CodeTypeDeclaration CreateMainType()
        {
            var wrapProxyClass = base.CreateMainType();
            var destription = string.IsNullOrEmpty(headerInfo.description) ? "类" : headerInfo.description;
            //wrapProxyClass.CustomAttributes.Add(new CodeAttributeDeclaration("System.Serializable"));
            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement(destription, true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            return wrapProxyClass;
        }
    }
    #endregion

    #region Struct
    /// <summary>
    /// 5.结构体模板
    /// </summary>
    [Serializable]
    public class StructTempate : ScriptTemplate
    {
        [MenuItem("Assets/Create/C# TempScripts/Struct", priority = 5)]
        static void CreateStruct()
        {
            TempScriptHelper.QuickCreateTemplate<StructTempate>();
        }

        public override string Name
        {
            get
            {
                return "Struct";
            }
        }
        private ReorderableList nameSpaceList;
        private ReorderableList reorderableList;

        public StructTempate()
        {
            imports.AddRange(new List<string>() {
            "System",
            "UnityEngine",
            "UnityEngine.UI",
            "System.Collections",
            "System.Collections.Generic",
        });

            reorderableList = new ReorderableList(fields, typeof(string));
            reorderableList.onAddCallback += (x) => { fields.Add(new FieldItem()); };
            reorderableList.drawHeaderCallback += (x) => { EditorGUI.LabelField(x, "字段"); };
            reorderableList.drawElementCallback += (x, y, z, w) =>
            {
                DrawFieldItem(x, fields[y], true);
            };

            nameSpaceList = new ReorderableList(imports, typeof(string));
            nameSpaceList.onAddCallback += (x) => { imports.Add(""); };
            nameSpaceList.drawHeaderCallback += (x) => { EditorGUI.LabelField(x, "命名空间"); };
            nameSpaceList.drawElementCallback += (x, y, z, w) =>
            {
                imports[y] = DrawNameSpace(x, imports[y]);
            };
        }

        protected override CodeTypeDeclaration CreateMainType()
        {
            List<CodeMemberField> fields = new List<CodeMemberField>();
            foreach (var item in base.fields)
            {
                CodeMemberField prop = new CodeMemberField();
                prop.Type = new CodeTypeReference(item.type, CodeTypeReferenceOptions.GenericTypeParameter);
                prop.Attributes = MemberAttributes.Public;
                prop.Name = item.elementName;
                prop.Comments.Add(new CodeCommentStatement(item.comment));
                fields.Add(prop);
            }

            CodeTypeDeclaration wrapProxyClass = new CodeTypeDeclaration(headerInfo.scriptName);
            wrapProxyClass.TypeAttributes = System.Reflection.TypeAttributes.Public;
            wrapProxyClass.IsStruct = true;
            var destription = string.IsNullOrEmpty(headerInfo.description) ? "结构体" : headerInfo.description;
            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement(destription, true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            foreach (var field in fields){
                wrapProxyClass.Members.Add(field);
            }

            return wrapProxyClass;
        }

        public override void OnBodyGUI()
        {
            nameSpaceList.DoLayoutList();
            reorderableList.DoLayoutList();
        }
    }
    #endregion

    #region Interface
    /// <summary>
    /// 6.接口创建模板
    /// </summary>
    [Serializable]
    public class InterfaceTempate : ScriptTemplate
    {
        [MenuItem("Assets/Create/C# TempScripts/Interface", priority = 5)]
        static void CreateEnum()
        {
            TempScriptHelper.QuickCreateTemplate<InterfaceTempate>();
        }
        public override string Name
        {
            get
            {
                return "Interface";
            }
        }
        private ReorderableList nameSpaceList;
        private ReorderableList reorderableList;

        public InterfaceTempate()
        {
            imports.AddRange(new List<string> { "System", "UnityEngine" });
            reorderableList = new ReorderableList(propertys, typeof(string));
            reorderableList.onAddCallback += (x) => { propertys.Add(new PropertyItem()); };
            reorderableList.drawHeaderCallback += (x) => { EditorGUI.LabelField(x, "属性"); };
            reorderableList.elementHeightCallback = (x) => { return 2 * EditorGUIUtility.singleLineHeight; };
            reorderableList.drawElementCallback += (x, y, z, w) =>
            {
                DrawPropertyItem(x, propertys[y]);
            };

            nameSpaceList = new ReorderableList(imports, typeof(string));
            nameSpaceList.onAddCallback += (x) => { imports.Add(""); };
            nameSpaceList.drawHeaderCallback += (x) => { EditorGUI.LabelField(x, "命名空间"); };
            nameSpaceList.drawElementCallback += (x, y, z, w) =>
            {
                imports[y] = DrawNameSpace(x, imports[y]);
            };
        }


        protected override CodeTypeDeclaration CreateMainType()
        {
            List<CodeMemberProperty> propertysMemper = new List<CodeMemberProperty>();
            foreach (var item in propertys)
            {
                CodeMemberProperty prop = new CodeMemberProperty();
                prop.Type = new CodeTypeReference(item.type, CodeTypeReferenceOptions.GenericTypeParameter);
                prop.Attributes = MemberAttributes.Public;
                prop.Name = item.elementName;
                prop.HasGet = item.get;
                prop.HasSet = item.set;
                prop.Comments.Add(new CodeCommentStatement(item.comment));
                propertysMemper.Add(prop);
            }

            CodeTypeDeclaration wrapProxyClass = new CodeTypeDeclaration(headerInfo.scriptName);
            wrapProxyClass.TypeAttributes = System.Reflection.TypeAttributes.Public;
            wrapProxyClass.IsInterface = true;
            var destription = string.IsNullOrEmpty(headerInfo.description) ? "接口" : headerInfo.description;
            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement(destription, true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            foreach (var prop in propertysMemper)
            {
                wrapProxyClass.Members.Add(prop);
            }
            return wrapProxyClass;
        }

        public override void OnBodyGUI()
        {
            nameSpaceList.DoLayoutList();
            reorderableList.DoLayoutList();
        }
    }
    #endregion

    #region MonoBehaiver
    public class MonoBehaiverTemplate : ChildClassTemplate
    {
        [MenuItem("Assets/Create/C# TempScripts/Monobehaiver", priority = 5)]
        static void CreateEnum()
        {
            TempScriptHelper.QuickCreateTemplate<MonoBehaiverTemplate>();
        }
        public override string Name
        {
            get
            {
                return "Monobehaiver";
            }
        }
        protected override Type baseType
        {
            get
            {
                return typeof(MonoBehaviour);
            }
        }
        public MonoBehaiverTemplate()
        {
            imports.AddRange(new List<string> {
            "System",
            "UnityEngine",
            "UnityEngine.UI",
            "System.Collections",
            "System.Collections.Generic" });
        }
        protected override CodeTypeDeclaration CreateMainType()
        {
            var wrapProxyClass = base.CreateMainType();
            var destription = string.IsNullOrEmpty(headerInfo.description) ? "MonoBehaiver" : headerInfo.description;
            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement(destription, true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            return wrapProxyClass;
        }
    }
    #endregion

    #region Scriptable
    /// <summary>
    /// scriptableObject
    /// </summary>
    [Serializable]
    public class ScriptableObjTempate : ChildClassTemplate
    {
        [MenuItem("Assets/Create/C# TempScripts/Scriptable", priority = 5)]
        static void CreateEnum()
        {
            TempScriptHelper.QuickCreateTemplate<ScriptableObjTempate>();
        }
        public override string Name
        {
            get
            {
                return "Scriptable";
            }
        }
        protected override Type baseType
        {
            get
            {
                return typeof(ScriptableObject);
            }
        }
        protected override CodeTypeDeclaration CreateMainType()
        {
            var wrapProxyClass = base.CreateMainType();
            var destription = string.IsNullOrEmpty(headerInfo.description) ? "Scriptable对象" : headerInfo.description;
            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement(destription, true));
            wrapProxyClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            return wrapProxyClass;
        }
    }
    #endregion

    ///无限制自定义(只要继承于ScriptTemplate,并且不是抽象类,都可以自己加载到窗体上,快捷方式得自己定义(..还没实现自动添加))
    ///...
    /// <summary>
    /// UI模板
    /// </summary>
    //[Serializable]
    //public class UIPanelTempate : ScriptTemplate
    //{
    //    public override string Name
    //    {
    //        get
    //        {
    //            return "UIPanel";
    //        }
    //    }

    //    protected override CodeNamespace CreateNameSpace()
    //    {
    //        return null;
    //    }

    //    public override void OnBodyGUI()
    //    {

    //    }
    //}

}
