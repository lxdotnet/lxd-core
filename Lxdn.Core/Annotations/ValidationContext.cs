using System.Linq;
using System.Reflection;
using System.Collections;

namespace Lxdn.Annotations
{
    internal class ValidationContext
    {
        private ValidationContext(Path path, object value)
        {
            this.Path = path;
            this.Value = value;
        }

        public static ValidationContext Create(object root)
        {
            return new ValidationContext(Path.Begin(root), root);
        }

        public object Value { get; }

        public Path Path { get; }

        public ValidationContext StepInto(PropertyInfo property)
        {
            var step = new Step(property);
            return new ValidationContext(Path.GrowBy(step), property.GetValue(this.Value));
        }

        public ValidationContext StepInto(int index)
        {
            var member = new Member(index);
            var value = (this.Value as IEnumerable).OfType<object>().Skip(index).Take(1).Single();
            return new ValidationContext(this.Path.GrowBy(member), value);
        }
    }
}