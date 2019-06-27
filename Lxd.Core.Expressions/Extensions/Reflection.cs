
using Lxd.Core.Expressions.Operators.Custom;
using System.Linq.Expressions;
using System.Reflection;

namespace Lxd.Core.Expressions.Extensions
{
    internal static class Reflection
    {
        internal static Expression Call(this MethodInfo method, object instance, RuntimeInjector injector)
        {
            return Expression.Call(Expression.Constant(instance, method.DeclaringType), method, injector.Inject(method));
        }
    }
}
