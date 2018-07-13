using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;

using Model = NodeGraph.DataModel;

namespace NodeGraph
{
    [Serializable]
    public class NodeGUI
    {

        [SerializeField]
        private int m_nodeWindowId;
        [SerializeField]
        private Rect m_baseRect;

        [SerializeField]
        private Model.NodeData m_data;

        private NodeView _nodeDataDrawer;

        private NodeView nodeDataDrawer
        {
            get
            {
                if (_nodeDataDrawer == null)
                {
                    _nodeDataDrawer = UserDefineUtility.GetUserDrawer(m_data.Object.GetType()) as NodeView;
                    if (_nodeDataDrawer == null) _nodeDataDrawer = new NodeView();
                    _nodeDataDrawer.target = m_data.Object;
                }
                return _nodeDataDrawer;

            }
        }
        [SerializeField]
        private NodeGraphController m_controller;

        [SerializeField]
        private GUIStyle m_nodeSyle;

        [SerializeField]
        private NodeGUIInspectorHelper m_nodeInsp;

        /*
			show error on node functions.
		*/
        private bool m_hasErrors = false;
        /*
    show progress on node functions(unused. due to mainthread synchronization problem.)
    can not update any visual on Editor while building AssetBundles through NodeGraph.DataModel.
*/
        private float m_progress;
        private bool m_running;

        /*
		 * Properties
		 */
        public string Name
        {
            get
            {
                return m_data.Name;
            }
            set
            {
                m_data.Name = value;
            }
        }
        public NodeGraphController Controller
        {
            get
            {
                return m_controller;
            }
            set
            {
                m_controller = value;
            }
        }

        public string Id
        {
            get
            {
                return m_data.Id;
            }
        }

        public Model.NodeData Data
        {
            get
            {
                return m_data;
            }
        }

        public Rect Region
        {
            get
            {
                return m_baseRect;
            }
        }

        public Model.NodeGraphObj ParentGraph
        {
            get
            {
                if (Controller == null)
                    return null;
                return Controller.TargetGraph;
            }
        }

        private NodeGUIInspectorHelper Inspector
        {
            get
            {
                if (m_nodeInsp == null)
                {
                    m_nodeInsp = ScriptableObject.CreateInstance<NodeGUIInspectorHelper>();
                    m_nodeInsp.hideFlags = HideFlags.DontSave;
                    m_nodeInsp.UpdateNodeGUI(this);
                }
                return m_nodeInsp;
            }
        }

        public void ResetErrorStatus()
        {
            m_hasErrors = false;
            Inspector.UpdateErrors(new List<string>());
        }

        public void AppendErrorSources(List<string> errors)
        {
            this.m_hasErrors = true;
            Inspector.UpdateErrors(errors);
        }

        public int WindowId
        {
            get
            {
                return m_nodeWindowId;
            }

            set
            {
                m_nodeWindowId = value;
            }
        }

        public NodeGUI(NodeGraphController controller, Model.NodeData data)
        {
            m_nodeWindowId = 0;
            m_controller = controller;
            m_data = data;
            m_data.Object.Initialize(m_data);
            m_baseRect = new Rect(m_data.X, m_data.Y, NGEditorSettings.GUI.NODE_BASE_WIDTH + nodeDataDrawer.SuperWidth, NGEditorSettings.GUI.NODE_BASE_HEIGHT + nodeDataDrawer.SuperHeight);
            m_nodeSyle = nodeDataDrawer == null ? EditorStyles.miniButton : nodeDataDrawer.InactiveStyle;
        }

        public NodeGUI Duplicate(NodeGraphController controller, float newX, float newY)
        {
            var data = m_data.Duplicate();
            data.X = newX;
            data.Y = newY;
            return new NodeGUI(controller, data);
        }

        public void SetActive(bool active)
        {
            if (active)
            {
                Selection.activeObject = Inspector;
                m_nodeSyle = nodeDataDrawer == null ? EditorStyles.miniButton : nodeDataDrawer.ActiveStyle;
            }
            else
            {
                m_nodeSyle = nodeDataDrawer == null ? EditorStyles.miniButton : nodeDataDrawer.InactiveStyle;
            }
        }

        private void RefreshConnectionPos(float yOffset)
        {
            for (int i = 0; i < m_data.InputPoints.Count; i++)
            {
                var point = m_data.InputPoints[i];
                point.Region = ConnectionPointDataUtility.UpdateRegion(this, point.IsInput, yOffset, i, m_data.InputPoints.Count);
                //point.UpdateRegion(this, yOffset, i, m_data.InputPoints.Count);
            }

            for (int i = 0; i < m_data.OutputPoints.Count; i++)
            {
                var point = m_data.OutputPoints[i];
                point.Region = ConnectionPointDataUtility.UpdateRegion(this, point.IsInput, yOffset, i, m_data.OutputPoints.Count);
                //point.UpdateRegion(this, yOffset, i, m_data.OutputPoints.Count);
            }
        }

        //private bool IsValidInputConnectionPoint(Model.ConnectionPointData point)
        //{
        //    return m_data.Operation.Object.IsValidInputConnectionPoint(point);
        //}

        /**
			retrieve mouse events for this node in this GraphEditor window.
		*/
        private void HandleNodeMouseEvent()
        {
            switch (Event.current.type)
            {

                /*
                        handling release of mouse drag from this node to another node.
                        this node doesn't know about where the other node is. the master only knows.
                        only emit event.
                    */
                case EventType.Ignore:
                    {
                        NodeGUIUtility.NodeEventHandler(new NodeEvent(NodeEvent.EventType.EVENT_CONNECTING_END, this, Event.current.mousePosition, null));
                        break;
                    }

                /*
					check if the mouse-down point is over one of the connectionPoint in this node.
					then emit event.
				*/
                case EventType.MouseDown:
                    {
                        Model.ConnectionPointData result = IsOverConnectionPoint(Event.current.mousePosition);

                        if (result != null)
                        {
                            NodeGUIUtility.NodeEventHandler(new NodeEvent(NodeEvent.EventType.EVENT_CONNECTING_BEGIN, this, Event.current.mousePosition, result));

                            break;
                        }
                        else
                        {
                            NodeGUIUtility.NodeEventHandler(new NodeEvent(NodeEvent.EventType.EVENT_NODE_CLICKED,
                                this, Event.current.mousePosition, null));
                        }
                        break;
                    }
            }

            /*
				retrieve mouse events for this node in|out of this GraphTool window.
			*/
            switch (Event.current.rawType)
            {
                case EventType.MouseUp:
                    {
                        bool eventSent = false;
                        // send EVENT_CONNECTION_ESTABLISHED event if MouseUp performed on ConnectionPoint
                        Action<Model.ConnectionPointData> raiseEventIfHit = (Model.ConnectionPointData point) =>
                        {
                            // Only one connectionPoint can send NodeEvent.
                            if (eventSent)
                            {
                                return;
                            }

                            //// If InputConnectionPoint is not valid at this moment, ignore
                            //if (!IsValidInputConnectionPoint(point))
                            //{
                            //    return;
                            //}

                            if (point.Region.Contains(Event.current.mousePosition))
                            {
                                NodeGUIUtility.NodeEventHandler(
                                    new NodeEvent(NodeEvent.EventType.EVENT_CONNECTION_ESTABLISHED,
                                        this, Event.current.mousePosition, point));
                                eventSent = true;
                                return;
                            }

                            if (nodeDataDrawer != null)
                            {
                                nodeDataDrawer.OnClickNodeGUI(this, Event.current.mousePosition, IsOverConnectionPoint(Event.current.mousePosition));
                            }
                        };
                        m_data.InputPoints.ForEach(raiseEventIfHit);
                        m_data.OutputPoints.ForEach(raiseEventIfHit);
                        break;
                    }
            }

            /*
				right click to open Context menu
			*/
            if (Event.current.type == EventType.ContextClick || (Event.current.type == EventType.MouseUp && Event.current.button == 1))
            {
                var menu = new GenericMenu();

                if (nodeDataDrawer != null)
                {
                    nodeDataDrawer.OnContextMenuGUI(menu, this);
                }

                menu.AddItem(
                    new GUIContent("Delete"),
                    false,
                    () =>
                    {
                        NodeGUIUtility.NodeEventHandler(new NodeEvent(NodeEvent.EventType.EVENT_NODE_DELETE, this, Vector2.zero, null));
                    }
                );

                menu.ShowAsContext();
                Event.current.Use();
            }
        }


        public void DrawConnectionInputPointMark(NodeEvent eventSource, bool justConnecting)
        {
            var defaultPointTex = NodeGUIUtility.pointMark;
            var lastColor = GUI.color;

            bool shouldDrawEnable = eventSource == null || eventSource.eventSourceNode != null;

            bool shouldDrawWithEnabledColor =
                shouldDrawEnable && justConnecting &&
                eventSource != null &&
                eventSource.eventSourceNode.Id != this.Id &&
                eventSource.point.IsOutput;

            foreach (var point in m_data.InputPoints)
            {
                if (shouldDrawWithEnabledColor && Controller.GetConnectType(eventSource.point, point) != null)
                {
                    GUI.color = NGEditorSettings.GUI.COLOR_CAN_CONNECT;
                }
                else
                {
                    GUI.color = (justConnecting) ? NGEditorSettings.GUI.COLOR_CAN_NOT_CONNECT : NGEditorSettings.GUI.COLOR_CONNECTED;
                }
                var rect = ConnectionPointDataUtility.GetGlobalPointRegion(point.IsInput, point.Region, this);
                GUI.DrawTexture(rect, defaultPointTex);
                GUI.color = lastColor;
            }
        }

        public void DrawConnectionOutputPointMark(NodeEvent eventSource, bool justConnecting, Event current)
        {
            var defaultPointTex = NodeGUIUtility.pointMark;
            var lastColor = GUI.color;

            bool shouldDrawEnable = eventSource == null || eventSource.eventSourceNode != null;

            bool shouldDrawWithEnabledColor =
                shouldDrawEnable && justConnecting
                && eventSource != null
                && eventSource.eventSourceNode.Id != this.Id
                && eventSource.point.IsInput;

            var globalMousePosition = current.mousePosition;

            foreach (var point in m_data.OutputPoints)
            {
                var pointRegion = ConnectionPointDataUtility.GetGlobalPointRegion(point.IsInput, point.Region, this);
                //var pointRegion = point.GetGlobalPointRegion(this);

                if (shouldDrawWithEnabledColor && Controller.GetConnectType(eventSource.point, point) != null)
                {
                    GUI.color = NGEditorSettings.GUI.COLOR_CAN_CONNECT;
                }
                else
                {
                    GUI.color = (justConnecting) ? NGEditorSettings.GUI.COLOR_CAN_NOT_CONNECT : NGEditorSettings.GUI.COLOR_CONNECTED;
                }
                GUI.DrawTexture(
                    pointRegion,
                    defaultPointTex
                );
                GUI.color = lastColor;

                // eventPosition is contained by outputPointRect.
                if (pointRegion.Contains(globalMousePosition))
                {
                    if (current.type == EventType.MouseDown)
                    {
                        NodeGUIUtility.NodeEventHandler(
                            new NodeEvent(NodeEvent.EventType.EVENT_CONNECTING_BEGIN, this, current.mousePosition, point));
                    }
                }
            }
        }

        public void DrawNode()
        {
            GUI.Window(m_nodeWindowId, m_baseRect, DrawThisNode, string.Empty, m_nodeSyle);
            Controller.DrawNodeGUI(this);
        }

        private void DrawThisNode(int id)
        {
            UpdateNodeRect();
            HandleNodeMouseEvent();
            DrawNodeContents();
        }

        private void DrawNodeContents()
        {
            var oldColor = GUI.color;
            var textColor = (EditorGUIUtility.isProSkin) ? Color.black : oldColor;
            var style = new GUIStyle(EditorStyles.label);
            style.alignment = TextAnchor.MiddleCenter;

            var connectionNodeStyleOutput = new GUIStyle(EditorStyles.label);
            connectionNodeStyleOutput.alignment = TextAnchor.MiddleRight;

            var connectionNodeStyleInput = new GUIStyle(EditorStyles.label);
            connectionNodeStyleInput.alignment = TextAnchor.MiddleLeft;

            var titleHeight = style.CalcSize(new GUIContent(Name)).y + NGEditorSettings.GUI.NODE_TITLE_HEIGHT_MARGIN;
            var nodeTitleRect = new Rect(0, 0, m_baseRect.width, titleHeight);
            //GUI.color = textColor;
            //GUI.Label(nodeTitleRect, Name, style);
            GUI.color = oldColor;

            if (m_running)
            {
                EditorGUI.ProgressBar(new Rect(10f, m_baseRect.height - 20f, m_baseRect.width - 20f, 10f), m_progress, string.Empty);
            }

            if (m_hasErrors)
            {
                GUIStyle errorStyle = new GUIStyle("CN EntryError");
                errorStyle.alignment = TextAnchor.MiddleCenter;
                var labelSize = GUI.skin.label.CalcSize(new GUIContent(Name));
                EditorGUI.LabelField(new Rect((nodeTitleRect.width - labelSize.x) / 2.0f - 28f, (nodeTitleRect.height - labelSize.y) / 2.0f - 7f, 20f, 20f), string.Empty, errorStyle);
            }

            // draw & update connectionPoint button interface.
            Action<Model.ConnectionPointData> drawConnectionPoint = (Model.ConnectionPointData point) =>
            {
                var label = point.Label;
                if (!string.IsNullOrEmpty(label))
                {
                    var region = point.Region;
                    // if point is output node, then label position offset is minus. otherwise plus.
                    var xOffset = (point.IsOutput) ? -m_baseRect.width : NGEditorSettings.GUI.INPUT_POINT_WIDTH;
                    var labelStyle = (point.IsOutput) ? connectionNodeStyleOutput : connectionNodeStyleInput;
                    var labelRect = new Rect(region.x + xOffset, region.y - (region.height / 2), m_baseRect.width, region.height * 2);

                    GUI.color = textColor;
                    GUI.Label(labelRect, label, labelStyle);
                    GUI.color = oldColor;
                }
                GUI.backgroundColor = Color.clear;
                Texture2D tex = (point.IsInput) ? NodeGUIUtility.inputPointBG : NodeGUIUtility.outputPointBG;
                GUI.Button(point.Region, tex, "AnimationKeyframeBackground");
                GUI.backgroundColor = Color.white;
            };
            m_data.InputPoints.ForEach(drawConnectionPoint);
            m_data.OutputPoints.ForEach(drawConnectionPoint);

            nodeDataDrawer.OnNodeGUI(new Rect(0, 0, m_baseRect.width, m_baseRect.height), Data);
            GUIStyle catStyle = new GUIStyle("WhiteMiniLabel");
            catStyle.alignment = TextAnchor.LowerRight;
            var categoryRect = new Rect(2f, m_baseRect.height - 14f, m_baseRect.width - 4f, 16f);
            GUI.Label(categoryRect, nodeDataDrawer == null ? "" : nodeDataDrawer.Category, catStyle);
        }

        public void UpdateNodeRect()
        {
            // UpdateNodeRect will be called outside OnGUI(), so it use inacurate but simple way to calcurate label width
            // instead of CalcSize()

            float labelWidth = GUI.skin.label.CalcSize(new GUIContent(this.Name)).x;
            float outputLabelWidth = 0f;
            float inputLabelWidth = 0f;
            float notdefultLabelHeight = 0f;

            if (m_data.InputPoints.Find(x => !string.IsNullOrEmpty(x.Label)) != null || m_data.OutputPoints.Find(x => !string.IsNullOrEmpty(x.Label)) != null)
            {
                notdefultLabelHeight += EditorGUIUtility.singleLineHeight;
            }

            if (m_data.InputPoints.Count > 0)
            {
                var inputLabels = m_data.InputPoints.OrderByDescending(p => p.Label.Length).Select(p => p.Label);
                if (inputLabels.Any())
                {
                    inputLabelWidth = GUI.skin.label.CalcSize(new GUIContent(inputLabels.First())).x;
                }
            }

            if (m_data.OutputPoints.Count > 0)
            {
                var outputLabels = m_data.OutputPoints.OrderByDescending(p => p.Label.Length).Select(p => p.Label);
                if (outputLabels.Any())
                {
                    outputLabelWidth = GUI.skin.label.CalcSize(new GUIContent(outputLabels.First())).x;
                }
            }

            var titleHeight = GUI.skin.label.CalcSize(new GUIContent(Name)).y + NGEditorSettings.GUI.NODE_TITLE_HEIGHT_MARGIN;

            // update node height by number of output connectionPoint.
            var nPoints = Mathf.Max(m_data.OutputPoints.Count, m_data.InputPoints.Count);
            this.m_baseRect = new Rect(m_baseRect.x, m_baseRect.y,
                m_baseRect.width,
                NGEditorSettings.GUI.NODE_BASE_HEIGHT + titleHeight + (NGEditorSettings.GUI.FILTER_OUTPUT_SPAN * Mathf.Max(0, (nPoints - 1)))
            );

            var newWidth = Mathf.Max(NGEditorSettings.GUI.NODE_BASE_WIDTH, outputLabelWidth + inputLabelWidth + NGEditorSettings.GUI.NODE_WIDTH_MARGIN);
            newWidth = Mathf.Max(newWidth, labelWidth + NGEditorSettings.GUI.NODE_WIDTH_MARGIN);

            m_baseRect = new Rect(m_baseRect.x, m_baseRect.y, newWidth + nodeDataDrawer.SuperWidth, m_baseRect.height + nodeDataDrawer.SuperHeight + notdefultLabelHeight);

            RefreshConnectionPos(titleHeight);
        }

        private Model.ConnectionPointData IsOverConnectionPoint(Vector2 touchedPoint)
        {

            foreach (var p in m_data.InputPoints)
            {
                var region = p.Region;

                //if (!IsValidInputConnectionPoint(p))
                //{
                //    continue;
                //}

                if (region.x <= touchedPoint.x &&
                    touchedPoint.x <= region.x + region.width &&
                    region.y <= touchedPoint.y &&
                    touchedPoint.y <= region.y + region.height
                )
                {
                    return p;
                }
            }

            foreach (var p in m_data.OutputPoints)
            {
                var region = p.Region;
                if (region.x <= touchedPoint.x &&
                    touchedPoint.x <= region.x + region.width &&
                    region.y <= touchedPoint.y &&
                    touchedPoint.y <= region.y + region.height
                )
                {
                    return p;
                }
            }

            return null;
        }

        public Rect GetRect()
        {
            return m_baseRect;
        }

        public Vector2 GetPos()
        {
            return m_baseRect.position;
        }

        public int GetX()
        {
            return (int)m_baseRect.x;
        }

        public int GetY()
        {
            return (int)m_baseRect.y;
        }

        public int GetRightPos()
        {
            return (int)(m_baseRect.x + m_baseRect.width);
        }

        public int GetBottomPos()
        {
            return (int)(m_baseRect.y + m_baseRect.height);
        }

        public void SetPos(Vector2 position)
        {
            m_baseRect.position = position;
            m_data.X = position.x;
            m_data.Y = position.y;
        }

        public void MoveBy(Vector2 distance)
        {
            m_baseRect.position = m_baseRect.position + distance;
            m_data.X = m_data.X + distance.x;
            m_data.Y = m_data.Y + distance.y;
        }

        public void SetProgress(float val)
        {
            m_progress = val;
            if (NodeGraphWindow.Window)
            {
                NodeGraphWindow.Window.Repaint();
            }
        }

        public void ShowProgress()
        {
            m_running = true;
        }

        public void HideProgress()
        {
            m_running = false;
        }

        public bool Conitains(Vector2 globalPos)
        {
            if (m_baseRect.Contains(globalPos))
            {
                return true;
            }
            foreach (var point in m_data.OutputPoints)
            {
                var rect = ConnectionPointDataUtility.GetGlobalPointRegion(point.IsInput, point.Region, this);
                if (rect.Contains(globalPos))
                {
                    return true;
                }
            }
            return false;
        }

        public Model.ConnectionPointData FindConnectionPointByPosition(Vector2 globalPos)
        {

            foreach (var point in m_data.InputPoints)
            {
                //if (!IsValidInputConnectionPoint(point))
                //{
                //    continue;
                //}

                var gregion = ConnectionPointDataUtility.GetGlobalPointRegion(point.IsInput, point.Region, this);
                if (point.GetGlobalRegion(this.Region).Contains(globalPos) ||
                    gregion.Contains(globalPos))
                {
                    return point;
                }
            }

            foreach (var point in m_data.OutputPoints)
            {
                var gregion = ConnectionPointDataUtility.GetGlobalPointRegion(point.IsInput, point.Region, this);
                if (point.GetGlobalRegion(this.Region).Contains(globalPos) ||
                    gregion.Contains(globalPos))
                {
                    return point;
                }
            }

            return null;
        }

        internal void DrawNodeGUI(NodeGUIEditor nodeGUIEditor)
        {
            //JudgeName();
            EditorGUI.BeginChangeCheck();
            if (nodeDataDrawer != null)
            {
                nodeDataDrawer.OnInspectorGUI(this);
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (Controller != null) Controller.Validate(this);
                EditorUtility.SetDirty(Data.Object);
                if (ParentGraph) EditorUtility.SetDirty(ParentGraph);
            }
        }

        public static void ShowTypeNamesMenu(string current, List<string> contents, Action<string> ExistSelected)
        {
            var menu = new GenericMenu();

            for (var i = 0; i < contents.Count; i++)
            {
                var type = contents[i];
                var selected = false;
                if (type == current) selected = true;

                menu.AddItem(
                    new GUIContent(type),
                    selected,
                    () =>
                    {
                        ExistSelected(type);
                    }
                );
            }
            menu.ShowAsContext();
        }

        public static void ShowFilterKeyTypeMenu(string current, Action<string> Selected)
        {
            var menu = new GenericMenu();

            menu.AddDisabledItem(new GUIContent(current));

            menu.AddSeparator(string.Empty);

            for (var i = 0; i < TypeUtility.KeyTypes.Count; i++)
            {
                var type = TypeUtility.KeyTypes[i];
                if (type == current) continue;

                menu.AddItem(
                    new GUIContent(type),
                    false,
                    () =>
                    {
                        Selected(type);
                    }
                );
            }
            menu.ShowAsContext();
        }
    }
}
