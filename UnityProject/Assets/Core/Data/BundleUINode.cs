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
[System.Serializable]
public class BundleUINode : UINodeBase
{
#if UNITY_EDITOR
    public string guid;
    public bool good;
#endif
    public string bundleName;
    public override string IDName { get { return bundleName + panelName; } }
}
