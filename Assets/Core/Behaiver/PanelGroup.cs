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
/// <summary>
/// 用于标记ui打开的父级
/// [3维场景中可能有多个地方需要打开用户界面]
/// </summary>
public abstract class PanelGroup : MonoBehaviour {

    public List<BridgeObj> bridges;

    public abstract List<UINodeBase> Nodes { get; }

    protected virtual void Awake()
    {
        UIFacade.RegistGroup(this);
    }
    protected virtual void OnDestroy()
    {
        UIFacade.UnRegistGroup(this);
    }
}
