
using System.Linq;
using System.Reflection;

namespace Lxd.Core.Validation.Extensions
{
    internal static class Extensions
    {
        public static bool IsOptional(this PropertyInfo property)
        {
            return property.GetCustomAttributes<OptionalAttribute>().Any();
        }
    }
}