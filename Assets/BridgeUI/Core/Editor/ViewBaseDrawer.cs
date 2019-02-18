using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BridgeUI;
using UnityEditor;

namespace BridgeUIEditor
{
    [CustomEditor(typeof(ViewBase),true)]
    public class PanelCoreDrawer : Editor
    {
        private List<int> referencedGameObjects;
        private MonoBehaviour scriptBehaiver;
        private Texture2D hierarchyEventIcon;
        private Texture2D HierarchyEventIcon
        {
            get
            {
                if (hierarchyEventIcon == null)
                {
                    var path = AssetDatabase.GUIDToAssetPath("aa2592a47b3d354439af0fb76c22a296");
                    hierarchyEventIcon = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
                return hierarchyEventIcon;
            }
        }

        private void OnEnable()
        {
            scriptBehaiver = target as MonoBehaviour;
            referencedGameObjects = new List<int>();
            EditorApplication.hierarchyWindowItemOnGUI -= DrawHierarchyIcon;
            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyIcon;
            AnalysisItemFromScript();
        }

        private void OnDisable()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= DrawHierarchyIcon;
        }

        private void DrawHierarchyIcon(int instanceID, Rect selectionRect)
        {
            if (Application.isPlaying) return;
            if(scriptBehaiver == null)
            {
                EditorApplication.hierarchyWindowItemOnGUI -= DrawHierarchyIcon;
                return;
            }
            if (Selection.activeGameObject != scriptBehaiver.gameObject) return;

            if (referencedGameObjects.Contains(instanceID))
            {
                Rect rect = new Rect(selectionRect.x + selectionRect.width - 16f, selectionRect.y + 4f, 16f, 8f);
                GUI.DrawTexture(rect, HierarchyEventIcon);
            }
        }

        private void AnalysisItemFromScript()
        {
            var fields = target.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField|System.Reflection.BindingFlags.FlattenHierarchy);

            if (fields != null && fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    var fieldItem = fields[i];

                    if(typeof(UnityEngine.Object).IsAssignableFrom( fieldItem.FieldType))
                    {
                        var value = fieldItem.GetValue(target);
                        if (value == null) continue;

                        if (value is Component)
                        {
                            referencedGameObjects.Add((value as Component).gameObject.GetInstanceID());
                        }
                        else if (value is GameObject)
                        {
                            referencedGameObjects.Add((value as GameObject).GetInstanceID());
                        }
                    }
                 
                }
            }
        }

    }
}