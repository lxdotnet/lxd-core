
namespace Lxdn.Core.Validation
{
    public interface IValidator<in TOperatorModel>
    {
        bool IsInvalid(TOperatorModel model);
        ValidationError GetError(TOperatorModel model);
    }

    public interface IValidator : IValidator<object> { }
}