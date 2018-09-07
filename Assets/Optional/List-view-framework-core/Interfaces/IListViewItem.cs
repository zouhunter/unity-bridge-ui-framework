﻿using System;
using UnityEngine;

namespace ListView
{
    public interface IListViewItem
    {
        Vector3 localPosition { get; set; }
        Quaternion localRotation { get; set; }

        Action<Action> startSettling { set; }
        Action endSettling { set; }

        void SetActive(bool active);
        void SetSiblingIndex(int index);
    }

    public interface IListViewItem<TData, TIndex> : IListViewItem where TData : IListViewItemData<TIndex>
    {
        TData data { get; set; }
        Func<TIndex, IListViewItem<TData, TIndex>> getListItem { set; }

        void Setup(TData datum, bool firstTime);
    }
}
