using System;
using System.Collections.Generic;

namespace ListView
{
    public abstract class NestedListViewItemData<TChild, TIndex> : INestedListViewItemData<TChild, TIndex>
    {
        protected List<TChild> m_Children;

        public abstract TIndex index { get; }
        public abstract string template { get; }

        public List<TChild> children
        {
            get { return m_Children; }
            set
            {
                if (childrenChanging != null)
                    childrenChanging(this, value);

                m_Children = value;
            }
        }

        public event Action<NestedListViewItemData<TChild, TIndex>, List<TChild>> childrenChanging;
    }
}
