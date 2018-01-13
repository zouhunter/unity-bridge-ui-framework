using UnityEngine;

using System.Collections;

using System.Collections.Generic;

using System.Linq;

using System;



public class ComboBox : MonoBehaviour

{

    private Dictionary<object, object> dataSource;

    private object currentValue;

    private object currentDisplayText;

    private int currentIndex;

    private GUISkin skin;

    private Rect rect;

    private Rect relativeRect;

    private Rect parentWinRect;

    private Rect rectList;

    private Rect rectListView;

    private Rect rectListViewGroupTop;

    private Rect rectListViewGroupBottom;

    private bool showList;

    private int listItemCount;

    private Vector2 scrollPosition;

    private Texture2D scrollBarBg;

    private Texture2D scrollBarThumb;

    private float showListTime;

    private int guiDepth;

    private bool alreadyInitGuiDepth;



    /// <summary>

    /// 选择项更改事件参数 

    /// </summary>

    public class SelectItemChangedArgs : EventArgs

    {

        private object itemValue;

        private object itemDisplayText;

        public object ItemValue
        {

            get
            {

                return itemValue;

            }

        }

        public object ItemDisplayText
        {

            get
            {

                return itemDisplayText;

            }

        }

        public SelectItemChangedArgs(object iValue, object iDisplayText)

        {

            itemValue = iValue;

            itemDisplayText = iDisplayText;

        }

    }



    /// <summary>

    /// 选择项更改事件 

    /// </summary>

    public event EventHandler<SelectItemChangedArgs> OnSelectItemChanged;



    public object SelectValue
    {

        get { return currentValue; }

    }



    public object SelectDisplayText
    {

        get { return currentDisplayText; }

    }



    public float ShowListTime
    {

        get { return showListTime; }

    }



    /// <summary>

    /// 数据源 

    /// </summary>

    public Dictionary<object, object> DataSource
    {

        set
        {

            dataSource = value;

            currentDisplayText = null;

            currentValue = null;

        }

        get
        {

            return dataSource;

        }

    }



    /// <summary>

    /// 初始化各项参数,控件应该只调用一次此方法,进行重置数据源直接使用DataSource属性即可 

    /// </summary>

    /// <param name="s">

    /// A <see cref="GUISkin"/>

    /// </param>

    /// <param name="data">

    /// A <see cref="Dictionary<System.Object, System.Object>"/>

    /// </param>

    /// <param name="r">

    /// A <see cref="Rect"/>

    /// </param>

    /// <param name="listCount">

    /// A <see cref="System.Int32"/>

    /// </param>

    /// <param name="barBg">

    /// A <see cref="Texture2D"/>

    /// </param>

    /// <param name="barThumb">

    /// A <see cref="Texture2D"/>

    /// </param>

    /// <param name="depth">

    /// A <see cref="System.Int32"/>

    /// </param>

    public void Init(GUISkin s, Dictionary<object, object> data, Rect r, int listCount, Texture2D barBg, Texture2D barThumb, int depth)

    {

        skin = s;

        dataSource = data;

        relativeRect = r;

        listItemCount = listCount;

        scrollBarBg = barBg;

        scrollBarThumb = barThumb;

        currentIndex = -1;



        //将控件置于当前GUI元素之上,并且只在第一次调用初始化方法设置GUI深度,防止循环中重复调用

        if (!alreadyInitGuiDepth)
        {

            guiDepth = GUI.depth - 1;

            alreadyInitGuiDepth = true;

        }



        currentDisplayText = null;

        currentValue = null;



        GUI.skin.verticalScrollbar.normal.background = scrollBarBg;

        GUI.skin.verticalScrollbar.margin = new RectOffset(0, 0, 0, 0);

        GUI.skin.verticalScrollbarThumb.normal.background = scrollBarThumb;

    }



    /// <summary>

    /// 设置父类容器的坐标范围,计算控件在屏幕上真正的坐标位置 

    /// </summary>

    /// <param name="rectParent">

    /// A <see cref="Rect"/>

    /// </param>

    public void SetParentRect(Rect rectParent)

    {

        parentWinRect = rectParent;

        rect = new Rect(parentWinRect.x + relativeRect.x, parentWinRect.y + relativeRect.y, relativeRect.width, relativeRect.height);

    }



    /// <summary>

    /// 强行设置下拉列表是否是示 

    /// </summary>

    /// <param name="show">

    /// A <see cref="System.Boolean"/>

    /// </param>

    public void SetShowList(bool show)

    {

        if (showList)
        {

            showList = show;

        }

    }



    /// <summary>

    /// 绘制下拉列表框 

    /// </summary>

    public void Draw()

    {

        if (skin == null || dataSource == null)
        {

            return;

        }



        if (currentDisplayText == null || currentValue == null)
        {

            if (dataSource.Count > 0)
            {

                currentDisplayText = dataSource.Values.First();

                currentValue = dataSource.Keys.First();

                currentIndex = 0;

            }

            else
            {

                currentDisplayText = "无";

                currentValue = -1;

                currentIndex = -1;

            }

        }



        if (GUI.Button(rect, currentDisplayText == null ? "" : currentDisplayText.ToString(), skin.GetStyle("combobox")))
        {

            showList = !showList;

            if (showList)
            {

                showListTime = Time.time;

            }
            else
            {

                showListTime = 0f;

            }

        }



        this.DrawList();

    }



    /// <summary>

    /// 绘制列表项

    /// </summary>

    public void DrawList()

    {

        if (showList == true)
        {



            this.calculateItemCount();



            if (listItemCount < dataSource.Count)
            {



                //为了留出最方下的横线,这里高度减1

                rectList = new Rect(rect.x, rect.y + rect.height, rect.width, rect.height * listItemCount - 1);

                rectListView = new Rect(rect.x, rect.y + rect.height, rect.width - GUI.skin.verticalScrollbar.fixedWidth, rect.height * dataSource.Count);

                rectListViewGroupTop = new Rect(rectList.x, rectList.y, rectList.width, rectList.height + 1 - rect.height);

                rectListViewGroupBottom = new Rect(rectList.x, rectList.y + rectListViewGroupTop.height, rectList.width, rect.height);

                GUI.Box(rectListViewGroupTop, "", skin.GetStyle("comboxlist"));

                GUI.Box(rectListViewGroupBottom, "", skin.GetStyle("comboxlistbottom"));



                //scrollPosition = GUI.BeginScrollView (rectList, scrollPosition, rectListView, false, true);

                scrollPosition = Vector2.Lerp(scrollPosition, GUI.BeginScrollView(rectList, scrollPosition, rectListView, false, true), 0.5f);



                float top = rectList.y;

                for (int i = 0; i < dataSource.Count; i++)
                {

                    drawItem(new Rect(rectList.x, top, rect.width, rect.height), i);

                    top += rect.height;

                }

                GUI.EndScrollView();

            }

            else if (dataSource.Count > 0)
            {

                rectList = new Rect(rect.x, rect.y + rect.height, rect.width, rect.height * dataSource.Count - 1);

                rectListViewGroupTop = new Rect(rectList.x, rectList.y, rectList.width, rectList.height + 1 - rect.height);

                rectListViewGroupBottom = new Rect(rectList.x, rectList.y + rectListViewGroupTop.height, rectList.width, rect.height);

                GUI.Box(rectListViewGroupTop, "", skin.GetStyle("comboxlist"));

                GUI.Box(rectListViewGroupBottom, "", skin.GetStyle("comboxlistbottom"));

                GUI.BeginGroup(rectList);

                float top = 0;

                for (int i = 0; i < dataSource.Count; i++)
                {

                    drawItem(new Rect(0, top, rect.width, rect.height), i);

                    top += rect.height;

                }

                GUI.EndGroup();

            }

        }

    }



    /// <summary>

    /// 当listItemCount为0时,动态计算可以容纳的数据行数 

    /// </summary>

    private void calculateItemCount()

    {

        if (listItemCount == 0)
        {

            int availableAreaHeight = (int)(parentWinRect.height - relativeRect.y - relativeRect.height - 1);

            listItemCount = availableAreaHeight / (int)relativeRect.height;

        }

    }



    /// <summary>

    /// 绘制内容项,并响应事件 

    /// </summary>

    /// <param name="r">

    /// A <see cref="Rect"/>

    /// </param>

    /// <param name="index">

    /// A <see cref="System.Int32"/>

    /// </param>

    private void drawItem(Rect r, int index)

    {

        if (GUI.Button(r, dataSource.Values.ElementAt(index).ToString(), skin.GetStyle("comboxitem")))
        {

            currentDisplayText = dataSource.Values.ElementAt(index);

            currentValue = dataSource.Keys.ElementAt(index);

            if (currentIndex != index)
            {

                currentIndex = index;

                if (OnSelectItemChanged != null)
                {

                    OnSelectItemChanged(this, new SelectItemChangedArgs(currentValue, currentDisplayText));

                }

            }

            showList = false;

        }

    }



    void OnGUI()

    {

        GUI.depth = guiDepth;

        this.Draw();

    }



    /// <summary>

    /// 按时间比较各ComboBox控件的顺序,初衷是为了点击新的控件时,其它ComboBox控件下拉列表自动隐藏 

    /// </summary>

    public class ShowListComparer : IComparer<ComboBox>

    {

        #region IComparer[ComboBox] implementation

        int IComparer<ComboBox>.Compare(ComboBox x, ComboBox y)

        {

            return x.showListTime.CompareTo(y.showListTime) * -1;

        }

        #endregion

    }

}