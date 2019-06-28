namespace Lxdn.Core.Db
{
    public interface IParameterFacade
    {
        bool IsNull { get; }
        object Value { get; }
        string Name { get; }
    }
}