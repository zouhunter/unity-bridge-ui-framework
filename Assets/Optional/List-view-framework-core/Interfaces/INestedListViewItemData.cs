using System;
using System.Collections.Generic;

namespace ListView
{
    public interface INestedListViewItemData<TChild, TIndex> : IListViewItemData<TIndex>
    {
        List<TChild> children { get; }
        event Action<NestedListViewItemData<TChild, TIndex>, List<TChild>> childrenChanging;
    }
}
