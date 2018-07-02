using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using BridgeUI;
using BridgeUI.Model;

namespace BridgeUIEditor
{
    [CustomEditor(typeof(RuntimePanelGroup)), CanEditMultipleObjects()]
    public class RuntimePanelGroupDrawer : Editor
    {
        private PanelGroupObj groupObj;
        [CanEditMultipleObjects]
        private SerializedProperty scriptPorp;
        private SerializedProperty groupGuidProp;
        private SerializedProperty menuProp;
        private SerializedProperty resetMenuProp;
        private SerializedProperty assetbundleProp;
        private SerializedProperty groupassetProp;
        private Editor groupobjDrawer;
        private string groupobjPath;
        private string groupobjGuid;
        private AssetImporter importer;
        void OnEnable()
        {
            scriptPorp = serializedObject.FindProperty("m_Script");
            groupGuidProp = serializedObject.FindProperty("groupGuid");
            menuProp = serializedObject.FindProperty("menu");
            assetbundleProp = serializedObject.FindProperty("assetbundle");
            groupassetProp = serializedObject.FindProperty("groupasset");
            resetMenuProp = serializedObject.FindProperty("resetMenu");

            LoadGroupObj();
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(scriptPorp);
#if AssetBundleTools
            serializedObject.Update();
            DrawAcceptRect();
            DrawAssetOptions();

            if (groupobjDrawer != null){
                groupobjDrawer.OnInspectorGUI();
            }
            serializedObject.ApplyModifiedProperties();
#endif
        }

        private void DrawAssetOptions()
        {
            if (!CheckMenu()) EditorGUILayout.HelpBox("请选择正确的加载路径", MessageType.Error);

            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("[Menu] =", EditorStyles.label, GUILayout.Width(60)))
                {
                    if (Selection.activeObject != null)
                    {
                        if (ProjectWindowUtil.IsFolder(Selection.activeInstanceID))
                        {
                            var path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
                            if (path.Contains("Assets/StreamingAssets/"))
                            {
                                path = path.Replace("Assets/StreamingAssets/", "");
                            }
                            if (!path.Contains("/"))
                            {
                                menuProp.stringValue = path;
                            }
                        }
                    }
                }
                if (resetMenuProp.boolValue)
                {
                    menuProp.stringValue = EditorGUILayout.TextField(menuProp.stringValue);
                }
                else
                {
                    EditorGUILayout.LabelField("Auto Choise By Platform");
                }

                resetMenuProp.boolValue = EditorGUILayout.ToggleLeft("(const)", resetMenuProp.boolValue, GUILayout.Width(70));
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("[Bundle] =", EditorStyles.label, GUILayout.Width(70));
                if (resetMenuProp.boolValue)
                {
                    assetbundleProp.stringValue = EditorGUILayout.TextField(assetbundleProp.stringValue);

                    if (GUILayout.Button("r", GUILayout.Width(20)) && importer != null)
                    {
                        importer.assetBundleName = assetbundleProp.stringValue;
                        EditorUtility.SetDirty(importer);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField(assetbundleProp.stringValue);
                }

            }
        }

        private void UpdateBundleInfo()
        {
            serializedObject.Update();
            if (!string.IsNullOrEmpty(groupobjPath) && importer == null)
            {
                importer = AssetImporter.GetAtPath(groupobjPath);
            }

            if (groupObj != null)
            {
                groupassetProp.stringValue = groupObj.name;
            }

            if (groupObj != null && importer != null)
            {
                assetbundleProp.stringValue = importer.assetBundleName;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private bool CheckMenu()
        {
#if AssetBundleTools
            if (!resetMenuProp.boolValue) menuProp.stringValue = AssetBundleLoader.defultMenu;
#endif
            return !menuProp.stringValue.Contains("/")
                && !menuProp.stringValue.Contains("\\")
                && System.IO.Directory.Exists(Application.streamingAssetsPath + "/" + menuProp.stringValue);
        }
        private void LoadGroupObj()
        {
            if (!string.IsNullOrEmpty(groupGuidProp.stringValue))
            {
                var path = AssetDatabase.GUIDToAssetPath(groupGuidProp.stringValue);
                if (!string.IsNullOrEmpty(path))
                {
                    groupObj = AssetDatabase.LoadAssetAtPath<PanelGroupObj>(path);
                    if(groupObj != null)
                    {
                        groupobjPath = path;
                        UpdateDrawer();
                        UpdateBundleInfo();
                    }
                }
            }
        }
        private void DrawAcceptRect()
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, string.Format("[PanelGroup] = {0}", groupobjPath));

            switch (Event.current.type)
            {
                case EventType.mouseUp:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        if (groupObj) EditorGUIUtility.PingObject(groupObj);
                    }
                    break;
                case EventType.DragUpdated:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                    break;
                case EventType.DragPerform:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        foreach (var item in DragAndDrop.objectReferences)
                        {
                            if (item is PanelGroupObj)
                            {
                                RecordPanelGroupObj(item as PanelGroupObj);
                                break;
                            }
                        }
                    }
                    Event.current.Use();
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(groupobjPath))
            {
                var btnRect = new Rect(rect.x + 100, rect.y, rect.width - 100, rect.height);
                if (GUI.Button(btnRect, "(create new panel group)", EditorStyles.miniButtonRight))
                {
                    CreateNewPanelGroupObj();
                }
            }
        }
        /// <summary>
        /// 创建一个新的面板组
        /// </summary>
        private void CreateNewPanelGroupObj()
        {
            var new_group = ScriptableObject.CreateInstance<PanelGroupObj>();
            ProjectWindowUtil.CreateAsset(new_group, "new_group.asset");
            Selection.activeObject = target;
            EditorApplication.update = () =>
            {
                if(new_group == null)
                {
                    EditorApplication.update = null;
                }
                else
                {
                    var path = AssetDatabase.GetAssetPath(new_group);
                    if (!string.IsNullOrEmpty(path))
                    {
                        RecordPanelGroupObj(new_group);
                        EditorUtility.SetDirty(target);
                        EditorApplication.update = null;
                    }
                }
                
            };
        }
        public void LockInspectorWindow(bool isLock)
        {
            var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
            var window = EditorWindow.GetWindow(type);
            System.Reflection.PropertyInfo info = type.GetProperty("isLocked", System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance);
            info.SetValue(window, isLock, new object[] { });
        }
        /// <summary>
        /// 记录panelgroupobj
        /// </summary>
        /// <param name="obj"></param>
        private void RecordPanelGroupObj(PanelGroupObj obj)
        {
            if (groupObj != obj)
            {
                groupObj = obj;
                groupobjPath = AssetDatabase.GetAssetPath(obj);
                groupobjGuid = AssetDatabase.AssetPathToGUID(groupobjPath);
                groupGuidProp.stringValue = groupobjGuid;
                UpdateDrawer();
            }
        }

        private void UpdateDrawer()
        {
            if (groupObj != null){
                Editor.CreateCachedEditor(groupObj, typeof(PanelGroupObjDrawer), ref groupobjDrawer);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
