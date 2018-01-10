using BridgeUI.Model;
namespace BridgeUI
{/// <summary>
 /// 基本的窗口类型（觉得能否多开）
 /// </summary>
    public enum UIFormType
    {
        Fixed = 0,//固定窗口(只能打开单个)
        DragAble = 1,//可拖拽(可以打开多个小窗体)
        HeadBar = 2,//没有关闭按扭(只有场景跳转时关闭)
    }
    /// <summary>
    /// 基本层级类型（绝对层级）
    /// </summary>
    public enum UILayerType
    {
        Base = 0,//最低层，可以被任何界面复盖
        Tip = 1,//自动关闭的小提示（自动关，所以可以高点）
        Warning = 2//用于程序状态警告（不可以被其他层级掩盖）
    }

    //显示时的动画类型
    public enum UIAnimType
    {
        NoAnim = 0,
        ScalePanel = 1,
        PosUpPanel = 2,
        RotatePanel = 3,
    }

    public enum HideRule
    {
        AlaphGameObject = 0,//隐藏自己的可见物体
        HideGameObject = 1,//直接隐藏自己
    }


    public enum CloseRule
    {
        DestroyNoraml = 0,//普通销毁
        DestroyImmediate = 1,//快速销毁并从内存中清除
        DestroyDely = 2,//延迟销毁
        HideGameObject = 3//缓存文件,仅当场景切换时销毁
    }

    /// <summary>
    /// ui的显示类型
    /// </summary>
    [System.Serializable]
    public class UIType
    {
        //层级优先
        public int layerIndex;
        //位置控制
        public UIFormType form = UIFormType.Fixed;
        //绝对层级
        public UILayerType layer = UILayerType.Base;
        //隐藏透明度
        public float hideAlaph = 0.2f;
        //出场动画
        public UIAnimType enterAnim = UIAnimType.NoAnim;
        //关闭动画
        public UIAnimType quitAnim = UIAnimType.NoAnim;
        //关闭规则
        public CloseRule closeRule = CloseRule.DestroyImmediate;
        //隐藏规则
        public HideRule hideRule = HideRule.HideGameObject;
    }
}