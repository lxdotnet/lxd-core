namespace Lxdn.Core.Aggregates
{
    public interface IAccessor<TValue>
    {
        TValue GetValue();
        void SetValue(TValue value);
    }

    public interface IAccessor : IAccessor<object> { }
}