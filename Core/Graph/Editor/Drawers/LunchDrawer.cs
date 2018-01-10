
using NodeGraph;
using NodeGraph.DataModel;
using UnityEditor;

[CustomNodeGraphDrawer(typeof(Luncher))]
public class LuncherDawer : NodeDrawer
{
    public override string Category
    {
        get
        {
            return "empty";
        }
    }
    public override void OnInspectorGUI(NodeGUI gui)
    {
        EditorGUILayout.HelpBox("Any Lunch: Lunch SubPanels From Any State", MessageType.Info);
    }
}
