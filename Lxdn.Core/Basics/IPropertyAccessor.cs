namespace Lxdn.Core.Basics
{
    public interface IPropertyAccessor
    {
        object Value { get; set; }
        IProperty Property { get; }
    }
}
