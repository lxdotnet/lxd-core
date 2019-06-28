
namespace Lxdn.Core.Expressions.Operators.Models
{
    // discussion: this is a marker interface (ok, an abstract class, but does not matter)
    // for all operator models. It is NOT strictly needed, but increases the readability and understanding,
    // because otherwise we should simply use 'object' where an operator model is implied and we have
    // lots of such occurences.

    // as soon as we removed the information about the model from runtime,
    // we cannot use the concept 'many operators -> one implementation' anymore, because
    // as soon as we instantiated the implementation for one of many possible operators (attributes),
    // we don't have a property where to store the original operator id. This all means, that
    // from now we are restricted by the paradigm 'one operator -> one implementation' - A.D. 2016-05-16

    public abstract class OperatorModel {}
}
