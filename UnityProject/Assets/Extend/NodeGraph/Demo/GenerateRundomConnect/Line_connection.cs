using UnityEngine;
using NodeGraph;
using System;
using NodeGraph.DataModel;

[CustomConnection(Line_connection.type)]
public class Line_connection : Connection
{
    public const string type = "line_connection";
    public bool asChild;
}
