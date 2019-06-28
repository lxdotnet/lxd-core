
using System.Linq.Expressions;
using System.Reflection;
//using Lxdn.Core.Expressions.Operators.Custom;

namespace Lxdn.Core.Extensions
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
    }
}
