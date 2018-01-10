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
namespace BridgeUI
{
    /// <summary>
    /// 组面板[包含容器]
    /// </summary>
    public class GroupPanel : PanelBase
    {
        [SerializeField]
        private Transform content;
        public override Transform Content { get { return content == null ? Group.Trans : content; } }

    }
}