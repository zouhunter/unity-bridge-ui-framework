using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class GraphWorp  {
    public string graphName;
    public string guid;

    public GraphWorp(string graphName, string guid)
    {
        this.graphName = graphName;
        this.guid = guid;
    }
}
