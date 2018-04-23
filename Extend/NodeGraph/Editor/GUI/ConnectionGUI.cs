using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;
using NodeGraph.DataModel;

namespace NodeGraph
{
    [Serializable]
    public class ConnectionGUI
    {
        [SerializeField]
        private ConnectionData m_data;

        [SerializeField]
        private ConnectionPointData m_outputPoint;
        [SerializeField]
        private ConnectionPointData m_inputPoint;
        [SerializeField]
        private ConnectionGUIInspectorHelper m_inspector;
        [SerializeField]
        private NodeGraphController m_controller;
        private ConnectionView _connectionDrawer;
        private ConnectionView connectionDrawer
        {
            get
            {
                if(_connectionDrawer == null)
                {
                    if(m_data == null || m_data.Object == null)
                    {
                        _connectionDrawer = new ConnectionView();
                    }
                    else
                    {
                        _connectionDrawer = UserDefineUtility.GetUserDrawer(m_data.Object.GetType()) as ConnectionView;
                        if (_connectionDrawer == null) _connectionDrawer = new ConnectionView();
                        _connectionDrawer.target = m_data.Object;
                    }
                }
                return _connectionDrawer;
            }
        }
        public string ConnectionType
        {
            get
            {
                return m_data.ConnectionType;
            }
            set
            {
                m_data.ConnectionType = value;
            }
        }

        public string Id
        {
            get
            {
                return m_data.Id;
            }
        }

        public NodeGraphObj ParentGraph
        {
            get
            {
                return m_controller.TargetGraph;
            }
        }
        public NodeGraphController Controller
        {
            get
            {
                return m_controller;
            }
        }

        public ConnectionData Data
        {
            get
            {
                return m_data;
            }
        }
        public string OutputNodeId
        {
            get
            {
                return m_outputPoint.NodeId;
            }
        }

        public string InputNodeId
        {
            get
            {
                return m_inputPoint.NodeId;
            }
        }

        public ConnectionPointData OutputPoint
        {
            get
            {
                return m_outputPoint;
            }
        }

        public ConnectionPointData InputPoint
        {
            get
            {
                return m_inputPoint;
            }
        }


        public ConnectionGUIInspectorHelper Inspector
        {
            get
            {
                if (m_inspector == null)
                {
                    m_inspector = ScriptableObject.CreateInstance<ConnectionGUIInspectorHelper>();
                    m_inspector.hideFlags = HideFlags.DontSave;
                }
                return m_inspector;
            }
        }

        public bool IsSelected
        {
            get
            {
                return (m_inspector != null && Selection.activeObject == m_inspector && m_inspector.connectionGUI == this);
            }
        }

        private Rect m_buttonRect;

        public static ConnectionGUI LoadConnection(ConnectionData data, ConnectionPointData output, ConnectionPointData input, NodeGraphController controller)
        {
            return new ConnectionGUI(
                data,
                output,
                input,
                controller
            );
        }

        public static ConnectionGUI CreateConnection(string type,/* string type, */ConnectionPointData output, ConnectionPointData input, NodeGraphController controller)
        {
            var connection = NodeConnectionUtility.CustomConnectionTypes.Find(x => x.connection.Name == type).CreateInstance();

            return new ConnectionGUI(
                new ConnectionData(type, connection, output, input),
                output,
                input,
                controller
            );
        }

        private ConnectionGUI(ConnectionData data, ConnectionPointData output, ConnectionPointData input, NodeGraphController controller)
        {
            UnityEngine.Assertions.Assert.IsTrue(output.IsOutput, "Given Output point is not output.");
            UnityEngine.Assertions.Assert.IsTrue(input.IsInput, "Given Input point is not input.");
            m_controller = controller;
            m_inspector = ScriptableObject.CreateInstance<ConnectionGUIInspectorHelper>();
            m_inspector.hideFlags = HideFlags.DontSave;

            this.m_data = data;
            this.m_outputPoint = output;
            this.m_inputPoint = input;
           
            //connectionButtonStyle = "sv_label_0";
        }

        public Rect GetRect()
        {
            return m_buttonRect;
        }

        public void DrawConnection(List<NodeGUI> nodes)
        {
            var startNode = nodes.Find(node => node.Id == OutputNodeId);
            if (startNode == null)
            {
                return;
            }

            var endNode = nodes.Find(node => node.Id == InputNodeId);
            if (endNode == null)
            {
                return;
            }

            var startPoint = m_outputPoint.GetGlobalPosition(startNode.Region);

            var endPoint = m_inputPoint.GetGlobalPosition(endNode.Region);

            var centerPoint = startPoint + ((endPoint - startPoint) / 2);

            var pointDistanceX = NGEditorSettings.GUI.CONNECTION_CURVE_LENGTH;

            var startTan = new Vector3(startPoint.x + pointDistanceX, centerPoint.y, 0f);

            var endTan = new Vector3(endPoint.x - pointDistanceX, centerPoint.y, 0f);

            //var style = new GUIStyle(connectionButtonStyle);
            //DrawCurve
            DrawCurve(startPoint, endPoint, startTan, endTan);
            //处理右键事件
            HandleClick(centerPoint);
            //DrawNode
            connectionDrawer.OnConnectionGUI(startPoint, endPoint, startTan, endTan);
            //DrawLabel
            connectionDrawer.OnDrawLabel(centerPoint, ConnectionType);
        }
        private void DrawCurve(Vector2 startV3, Vector2 endV3, Vector2 startTan, Vector2 endTan)
        {
            Color lineColor;
            var lineWidth = connectionDrawer == null ? 3 : connectionDrawer.LineWidth;// (totalAssets > 0) ? 3f : 2f;

            if (IsSelected)
            {
                lineColor = NGEditorSettings.GUI.COLOR_ENABLED;
            }
            else
            {
                lineColor = connectionDrawer == null ? Color.gray : connectionDrawer.LineColor;
            }

            ConnectionGUIUtility.HandleMaterial.SetPass(0);

            Handles.DrawBezier(startV3, endV3, startTan, endTan, lineColor, null, lineWidth);
        }

        private void HandleClick(Vector3 center)
        {
            m_buttonRect = new Rect(center.x - 10, center.y - 35, 20, 50f);

            if ((Event.current.type == EventType.MouseUp && Event.current.button == 0))
            {
                var rightClickPos = Event.current.mousePosition;
                if (m_buttonRect.Contains(rightClickPos))
                {
                    this.Inspector.UpdateInspector(this);
                    ConnectionGUIUtility.ConnectionEventHandler(new ConnectionEvent(ConnectionEvent.EventType.EVENT_CONNECTION_TAPPED, this));
                    Event.current.Use();
                }

            }

            if (Event.current.type == EventType.ContextClick
               || (Event.current.type == EventType.MouseUp && Event.current.button == 1)
           )
            {
                var rightClickPos = Event.current.mousePosition;

                if (m_buttonRect.Contains(rightClickPos))
                {
                    var menu = new GenericMenu();

                    if (connectionDrawer != null)
                    {
                        connectionDrawer.OnContextMenuGUI(menu, this);
                    }

                    menu.AddItem(
                        new GUIContent("Delete"),
                        false,
                        () =>
                        {
                            Delete();
                        }
                    );
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }
        }

        public bool IsEqual(ConnectionPointData from, ConnectionPointData to)
        {
            return (m_outputPoint == from && m_inputPoint == to);
        }


        public void SetActive(bool active)
        {
            if (active)
            {
                Selection.activeObject = Inspector;
            }
        }
        public void DrawObject()
        {
            EditorGUI.BeginChangeCheck();
            if (connectionDrawer != null) connectionDrawer.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                Controller.Perform();
                EditorUtility.SetDirty( Data.Object);
                EditorUtility.SetDirty(ParentGraph);
            }
        }
        public void Delete()
        {
            ConnectionGUIUtility.ConnectionEventHandler(new ConnectionEvent(ConnectionEvent.EventType.EVENT_CONNECTION_DELETED, this));
        }
    }

    public static class NodeEditor_ConnectionListExtension
    {
        public static bool ContainsConnection(this List<ConnectionGUI> connections, ConnectionPointData output, ConnectionPointData input)
        {
            foreach (var con in connections)
            {
                if (con.IsEqual(output, input))
                {
                    return true;
                }
            }
            return false;
        }
    }
}