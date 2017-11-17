/// <summary>
/// 基本的窗口类型
/// </summary>
public enum UIFormType
{
    Fixed,//固定窗口(只能打开单个)
    DragAble//可拖拽(可以打开多个小窗体)
}
/// <summary>
/// 基本层级类型
/// </summary>
public enum UILayerType
{
    Bottom = 1,
    Middle = 2,
    Top = 3
}

/// <summary>
/// UI窗体透明度类型
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

/// <summary>
/// ui的基本类型
/// </summary>
[System.Serializable]
public class UIType
{
    public int layerIndex;
    //位置
    public UIFormType form = UIFormType.Fixed;
    //层级
    public UILayerType layer = UILayerType.Bottom;
    //透明度
    public UILucenyType luceny = UILucenyType.Lucency;

}