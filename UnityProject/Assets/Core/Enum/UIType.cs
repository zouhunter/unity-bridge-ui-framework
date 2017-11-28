/// <summary>
/// 基本的窗口类型（觉得能否多开）
/// </summary>
public enum UIFormType
{
    Fixed = 0,//固定窗口(只能打开单个)
    DragAble = 1//可拖拽(可以打开多个小窗体)
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

/// <summary>
/// UI窗体透明度类型(隐藏的一种方式)
/// </summary>
public enum UILucenyType
{
    //完全透明，不能穿透
    Lucency = 0,
    //半透明，不能穿透
    Translucence = 1,
    //低透明度，不能穿透
    ImPenetrable = 2,
    //可以穿透
    Pentrate = 3
}

//显示时的动画类型
public enum UIAnimType
{
    NoAnim = 0,
    LeftMoveIn = 1,
    ScaleFromZero = 2,
    ScaleFromLarge = 3
}

public enum HideRule
{
    HideChildObject = 0,//隐藏自己的可见物体
    HideGameObject = 1,//直接隐藏自己
    MoveToPoint = 2,//向空间移动
}


public enum CloseRule
{
    DestroyImmediate= 0,//快速销毁并从内存中清除
    DestroyDely = 1,//延迟销毁
    DestroyWithAnim = 2,//播放销毁动画
}

/// <summary>
/// ui的显示类型
/// </summary>
[System.Serializable]
public class UIType
{
    //层级优先
    public int layerIndex;
    //互斥关键字
    public string mutexKey;
    //位置控制
    public UIFormType form = UIFormType.Fixed;
    //绝对层级
    public UILayerType layer = UILayerType.Base;
    //隐藏透明度
    public UILucenyType hideLuceny = UILucenyType.Lucency;
    //出场动画
    public UIAnimType animType = UIAnimType.NoAnim;
    //关闭规则
    public CloseRule closeRule = CloseRule.DestroyImmediate;
    //隐藏规则
    public HideRule hideRule = HideRule.HideGameObject;
}