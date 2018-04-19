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
        private Editor groupobjDrawer;
        private string groupobjPath;
        private string groupobjGuid;
        void OnEnable()
        {
            scriptPorp = serializedObject.FindProperty("m_Script");
            groupGuidProp = serializedObject.FindProperty("groupGuid");
            LoadGroupObj();
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(scriptPorp);
            serializedObject.Update();
            DrawAcceptRect();
            if (groupobjDrawer != null){
                groupobjDrawer.OnInspectorGUI();
            }
            serializedObject.ApplyModifiedProperties();
        }
        private void LoadGroupObj()
        {
            if (!string.IsNullOrEmpty(groupGuidProp.stringValue))
            {
                groupobjPath = AssetDatabase.GUIDToAssetPath(groupGuidProp.stringValue);
                if (!string.IsNullOrEmpty(groupobjPath))
                {
                    groupObj = AssetDatabase.LoadAssetAtPath<PanelGroupObj>(groupobjPath);
                    UpdateDrawer();
                }
            }
        }
        private void DrawAcceptRect()
        {
            var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rect, string.Format("[PanelGroup={0}]", groupobjPath));

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
            if (groupObj != null)
            {
                Editor.CreateCachedEditor(groupObj, typeof(PanelGroupObjDrawer), ref groupobjDrawer);
            }
        }
    }
}
