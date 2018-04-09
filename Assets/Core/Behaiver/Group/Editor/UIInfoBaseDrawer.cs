using UnityEngine;
using UnityEditor;
using BridgeUI;
namespace BridgeUIEditor
{
    public abstract class UIInfoBaseDrawer : PropertyDrawer
    {
        protected SerializedProperty panelNameProp;
        protected SerializedProperty typeProp;
        protected SerializedProperty layerProp;
        protected SerializedProperty formProp;
        protected SerializedProperty layerIndexProp;
        protected SerializedProperty instanceIDProp;
        protected SerializedObject serializedObject;
        protected const float widthBt = 20;
        protected float singleHeight;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            singleHeight = EditorGUIUtility.singleLineHeight;
            this.serializedObject = property.serializedObject;
            InitPropertys(property);
            if (!property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            else
            {
                return GetInfoItemHeight();
            }
        }
        protected virtual void InitPropertys(SerializedProperty property)
        {
            panelNameProp = property.FindPropertyRelative("panelName");
            typeProp = property.FindPropertyRelative("type"); ;
            formProp = typeProp.FindPropertyRelative("form");
            layerProp = typeProp.FindPropertyRelative("layer");
            layerIndexProp = typeProp.FindPropertyRelative("layerIndex");
            instanceIDProp = property.FindPropertyRelative("instanceID");
        }

        protected abstract float GetInfoItemHeight();


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect btnRect = new Rect(position.xMin, position.yMin, position.width * 0.9f, singleHeight);
            GUI.contentColor = Color.green;
            if (property.isExpanded && (instanceIDProp.intValue == 0 || EditorUtility.InstanceIDToObject(instanceIDProp.intValue) == null))
            {
                property.isExpanded = false;
            }
            if (GUI.Button(btnRect, panelNameProp.stringValue, EditorStyles.toolbarDropDown))
            {
                ResetBuildInfoOnOpen();

                property.isExpanded = !property.isExpanded;

                if (property.isExpanded)
                {
                    if (instanceIDProp.intValue == 0 || EditorUtility.InstanceIDToObject(instanceIDProp.intValue) == null)
                    {
                        var gopfb = GetPrefabItem();
                        if (gopfb != null)
                        {
                            InstantiatePrefab(gopfb);
                        }
                        else
                        {
                            Debug.Log("未找到预制体:" + panelNameProp.stringValue);
                        }
                    }
                }
                else
                {
                    if (instanceIDProp.intValue != 0)
                    {
                        HideItemIfInstenced();
                    }
                    instanceIDProp.intValue = 0;
                }
            }
            GUI.contentColor = Color.white;

            WorningIfNotRight(btnRect);

            InformationShow(btnRect);

            Rect acceptRect = new Rect(position.max.x - position.width * 0.1f, position.yMin, position.width * 0.1f, singleHeight);

            //DragAndDrapAction(acceptRect);

            DrawObjectField(acceptRect);

            if (property.isExpanded)
            {
                Rect opendRect = new Rect(position.xMin, position.yMin + singleHeight, position.width, position.height - singleHeight);
                DrawExpanded(opendRect);
            }
        }

        protected virtual void InformationShow(Rect rect)
        {
            var infoRect = rect;
            infoRect.x = infoRect.width - 125;
            infoRect.width = 25;
            GUI.color = new Color(0.3f, 0.5f, 0.8f);
            EditorGUI.SelectableLabel(infoRect, string.Format("[{0}]", ((UIFormType)formProp.enumValueIndex).ToString().Substring(0, 1)));

            infoRect.x += 50;
            infoRect.width = 50;
            GUI.color = new Color(0.8f, 0.8f, 0.4f);
            string str = Utility.LayerToString((UILayerType)layerProp.intValue, false);// LayerToString();
            EditorGUI.SelectableLabel(infoRect, string.Format("{0} {1}", str, layerIndexProp.intValue));
            GUI.color = Color.white;
        }

        private bool LayerContains(UILayerType layerEnum)
        {
            var layer = (int)layerEnum;
            return (layerProp.intValue & layer) == layer;
        }

        protected abstract void DrawExpanded(Rect opendRect);

        protected abstract void DrawObjectField(Rect acceptRect);

        protected virtual void DragAndDrapAction(Rect acceptRect)
        {
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    if (acceptRect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                    break;
                case EventType.DragPerform:
                    if (acceptRect.Contains(Event.current.mousePosition))
                    {
                        if (DragAndDrop.objectReferences.Length > 0)
                        {
                            var obj = DragAndDrop.objectReferences[0];
                            if (obj is GameObject)
                            {
                                OnDragPerformGameObject(obj as GameObject);
                            }
                        }
                        DragAndDrop.AcceptDrag();
                        Event.current.Use();
                    }
                    break;
            }
        }

        protected abstract void OnDragPerformGameObject(GameObject go);

        protected abstract void WorningIfNotRight(Rect rect);

        protected virtual void HideItemIfInstenced()
        {
            var obj = EditorUtility.InstanceIDToObject(instanceIDProp.intValue);
            if (obj != null)
            {
                var go = obj as GameObject;
                BridgeEditorUtility.ApplyPrefab(go);
                if (go.transform.parent.GetComponent<PanelGroup>() == null && go.transform.parent.childCount == 1)
                {
                    Object.DestroyImmediate(go.transform.parent.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(obj);
                }
            }
            instanceIDProp.intValue = 0;
        }

        protected virtual void ResetBuildInfoOnOpen()
        {
            //使用对象是UIGroupObj，将无法从button和Toggle加载
            //if (serializedObject.targetObject is PanelGroupObj)
            //{

            //}
        }

        protected abstract GameObject GetPrefabItem();

        protected virtual void InstantiatePrefab(GameObject gopfb)
        {
            if (gopfb != null)
            {
                GameObject go = PrefabUtility.InstantiatePrefab(gopfb) as GameObject;
                PanelGroup uigroup = null;
                if (serializedObject.targetObject is PanelGroup)
                {
                    uigroup = serializedObject.targetObject as PanelGroup;
                }
                else
                {
                    uigroup = Object.FindObjectOfType<PanelGroup>();
                }
                if (uigroup != null)
                {
                    Utility.SetTranform(go.transform, (UILayerType)layerProp.intValue, layerIndexProp.intValue, uigroup.transform);
                }
                else
                {
                    go.transform.SetParent(null);
                }

                instanceIDProp.intValue = go.GetInstanceID();
            }
        }


        protected virtual void Worning(Rect rect, string info)
        {
            GUI.color = Color.red;
            EditorGUI.SelectableLabel(rect, info);
            GUI.color = Color.white;
        }
    }
}