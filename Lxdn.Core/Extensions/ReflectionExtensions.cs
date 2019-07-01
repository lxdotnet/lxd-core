using System.Reflection;

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
