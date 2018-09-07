
using System.Collections.Generic;

namespace BridgeUI.Control.Chart
{
    public interface IInject
    {
        void Inject<T>(IList<T> data);
        void Inject<T>(IList<T>[] datas );
    }
}