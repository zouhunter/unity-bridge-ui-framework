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
public class NodeInfo {
    public int layerIndex;
    public UIFormType form = UIFormType.Fixed;
    public UILayerType layer = UILayerType.Bottom;
    public UILucenyType luceny = UILucenyType.Lucency;
    public string prefabGuid;
}
