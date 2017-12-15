
using NodeGraph;
using NodeGraph.DataModel;
using UnityEditor;

[CustomNodeGraphDrawer(typeof(Luncher))]
public class LuncherDawer : NodeDrawer
{
    public override string ActiveStyle
    {
        get
        {
            return "node 0 on";
        }
    }

    public override string InactiveStyle
    {
        get
        {
            return "node 0";
        }
    }

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
    public override void OnContextMenuGUI(GenericMenu menu)
    {
        base.OnContextMenuGUI(menu);
    }
}
