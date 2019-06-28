
namespace Lxd.Core.Expressions.Verbs
{
    public interface IVerb<out TReturn>
    {
        TReturn Apply(object[] instances);
    }
}