using System.Reflection;

namespace Lxdn.Annotations
{
    public class Step : IStep
    {
        public Step(PropertyInfo property)
        {
            this.Property = property;
        }

        public PropertyInfo Property { get; }

        public override string ToString()
        {
            return "." + this.Property.Name; //.ToLowerCamelCase();
        }
    }
}