
using System.Linq.Expressions;
using System.Reflection;
//using Lxd.Core.Expressions.Operators.Custom;

namespace Lxd.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static bool HasPublicGetter(this PropertyInfo property)
        {
            return property.CanRead && property.GetGetMethod() != null && property.GetGetMethod().IsPublic;
        }

        public static bool HasPublicSetter(this PropertyInfo property)
        {
            return property.CanWrite && property.GetSetMethod() != null && property.GetSetMethod().IsPublic;
        }

        //internal static Expression Call(this MethodInfo method, object instance, RuntimeInjector injector)
        //{
        //    return Expression.Call(Expression.Constant(instance, method.DeclaringType), method, injector.Inject(method));
        //}
    }
}
