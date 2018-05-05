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
using BridgeUI.Model;
namespace BridgeUI
{
    public enum MutexRule
    {
        NoMutex,//不排斥
        SameParentAndLayer,//排斥同父级中的同层级
        SameLayer,  //排斥同层级
    }
    public enum BaseShow
    {
        NoChange,//不改变父级状态
        Hide,//隐藏父级(在本面板关闭时打开)
        Destroy,//销毁父级(接管因为父级面关闭的面板)
    }
    /// <summary>
    /// 界面的显示状态
    /// </summary>
    [System.Serializable]
    public struct ShowMode
    {
        public bool auto;//当上级显示时显示
        public bool cover;//建立遮罩只允许当前面版操作
        public bool single;//隐藏所有打开的面板
        public MutexRule mutex;//排斥有相同类型面版
        public BaseShow baseShow;//父级的显示状态

        public ShowMode(bool auto, MutexRule mutex, bool cover, BaseShow baseShow, bool single)
        {
            this.auto = auto;
            this.mutex = mutex;
            this.cover = cover;
            this.baseShow = baseShow;
            this.single = single;
        }
    }
}