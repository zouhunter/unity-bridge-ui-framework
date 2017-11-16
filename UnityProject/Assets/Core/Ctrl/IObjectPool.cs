public interface IObjectPool<T>
{
    T Allocate();
}