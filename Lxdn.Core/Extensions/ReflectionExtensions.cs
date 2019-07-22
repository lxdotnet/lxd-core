using System.Reflection;

namespace Lxdn.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static bool HasPublicGetter(this PropertyInfo property)
        {
            return property.CanRead && property.GetGetMethod().IfExists(get => get.IsPublic);
        }

        public static bool HasPublicSetter(this PropertyInfo property)
        {
            return property.CanWrite && property.GetSetMethod().IfExists(set => set.IsPublic);
        }
    }
}
