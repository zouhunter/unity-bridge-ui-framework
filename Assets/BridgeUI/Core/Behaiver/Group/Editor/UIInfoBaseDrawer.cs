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
        protected SerializedProperty discriptionProp;
        protected SerializedProperty instanceIDProp;
        protected SerializedObject serializedObject; 
        protected const float widthBt = 20;
        protected float singleHeight;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            singleHeight = EditorGUIUtility.singleLineHeight;
            this.serializedObject = property.serializedObject;
            InitPropertys(property);
            typeProp = property.FindPropertyRelative("type");
            var height = EditorGUIUtility.singleLineHeight;
            if (property.isExpanded){
                height += EditorGUI.GetPropertyHeight(typeProp);
                height += GetInfoItemHeight();
            }
            return height;

        }
        protected virtual void InitPropertys(SerializedProperty property)
        {
            panelNameProp = property.FindPropertyRelative("panelName");
            discriptionProp = property.FindPropertyRelative("discription");
            typeProp = property.FindPropertyRelative("type");
            formProp = typeProp.FindPropertyRelative("form");
            layerProp = typeProp.FindPropertyRelative("layer");
            layerIndexProp = typeProp.FindPropertyRelative("layerIndex");
            instanceIDProp = property.FindPropertyRelative("instanceID");
        }

        protected abstract float GetInfoItemHeight();


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitPropertys(property);

            Rect btnRect = new Rect(position.xMin, position.yMin + 2f, position.width * 0.9f, singleHeight);
            GUI.contentColor = Color.green;
            if (property.isExpanded && (instanceIDProp.intValue == 0 || EditorUtility.InstanceIDToObject(instanceIDProp.intValue) == null))
            {
                property.isExpanded = false;
            }

            var showName = panelNameProp.stringValue +
                (string.IsNullOrEmpty(discriptionProp.stringValue) ? "" : (": " + discriptionProp.stringValue));

            if (panelNameProp != null && GUI.Button(btnRect, showName, EditorStyles.toolbarDropDown))
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
                            InstantiatePrefab(gopfb, GetParent());
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

            Rect acceptRect = new Rect(position.max.x - position.width * 0.1f, position.yMin + 2f, position.width * 0.1f, singleHeight);

            //DragAndDrapAction(acceptRect);

            DrawObjectField(acceptRect);

            if (property.isExpanded)
            {
                Rect opendRect = new Rect(position.xMin, position.yMin + singleHeight, position.width, position.height - singleHeight);
                DrawExpanded(opendRect);
            }
        }

        protected Rect GetPaddingRect(Rect rect, float padding)
        {
            var newRect = new Rect(rect.x + padding, rect.y + padding, rect.width - padding * 2, rect.height - padding * 2);
            return newRect;
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

            if (layerProp == null) return;//??
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

        protected virtual void InstantiatePrefab(GameObject gopfb, Transform parent)
        {
            if (gopfb != null)
            {
                GameObject go = PrefabUtility.InstantiatePrefab(gopfb) as GameObject;
                Utility.SetTranform(go.transform, (UILayerType)layerProp.intValue, layerIndexProp.intValue, parent);
                instanceIDProp.intValue = go.GetInstanceID();
            }
        }

        protected Transform GetParent()
        {
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
                return uigroup.transform;
            }
#if AssetBundleTools
            RuntimePanelGroup runtimeGroup = Object.FindObjectOfType<RuntimePanelGroup>();
            if (runtimeGroup != null)
            {
                return runtimeGroup.transform;
            }
#endif
            return null;
        }

        protected virtual void Worning(Rect rect, string info)
        {
            GUI.color = Color.red;
            EditorGUI.SelectableLabel(rect, info);
            GUI.color = Color.white;
        }
    }
}