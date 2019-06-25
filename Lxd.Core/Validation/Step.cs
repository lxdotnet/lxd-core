using System.Reflection;

namespace Lxd.Validation
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