
namespace Lxdn.Core.Validation
{
    public interface IValidator<in TModel>
    {
        bool IsInvalid(TModel model);
        ValidationError GetError(TModel model);
    }

    public interface IValidator : IValidator<object> { }
}