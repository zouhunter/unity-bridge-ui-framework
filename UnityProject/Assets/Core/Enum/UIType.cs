/// <summary>
/// 基本的窗口类型（觉得能否多开）
/// </summary>
public enum UIFormType
{
    Fixed,//固定窗口(只能打开单个)
    DragAble//可拖拽(可以打开多个小窗体)
}
/// <summary>
/// 基本层级类型（绝对层级）
/// </summary>
public enum UILayerType
{
    Base = 1,//最低层，可以被任何界面复盖
    Tip = 2,//自动关闭的小提示（自动关，所以可以高点）
    Warning = 3//用于程序状态警告（不可以被其他层级掩盖）
}

/// <summary>
/// UI窗体透明度类型(隐藏的一种方式)
/// </summary>
public enum UILucenyType
{
    //完全透明，不能穿透
    Lucency,
    //半透明，不能穿透
    Translucence,
    //低透明度，不能穿透
    ImPenetrable,
    //可以穿透
    Pentrate
}

//显示时的动画类型
public enum UIAnimType
{
    NoAnim,
    LeftMoveIn,
    ScaleFromZero,
    ScaleFromLarge
}

/// <summary>
/// ui的基本类型
/// </summary>
[System.Serializable]
public class UIType
{
    public int layerIndex;
    //关键字
    public string mutexKey;
    //位置
    public UIFormType form = UIFormType.Fixed;
    //层级
    public UILayerType layer = UILayerType.Base;
    //透明度
    public UILucenyType hideLuceny = UILucenyType.Lucency;
    //动画类型
    public UIAnimType animType = UIAnimType.NoAnim;
}