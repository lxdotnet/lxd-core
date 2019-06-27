
using System.Reflection;

namespace Lxd.Core.Validation
{
    public class OperatorStep
    {
        public OperatorStep(PropertyInfo property)
        {
            this.Property = property;
        }

        public PropertyInfo Property { get; private set; }

        public override string ToString()
        {
            return "." + this.Property.Name;
        }
    }

    public class CollectionMember : OperatorStep
    {
        public CollectionMember(PropertyInfo property, int index) : base(property)
        {
            this.Index = index;
        }

        public int Index { get; private set; }

        public override string ToString()
        {
            return "[" + this.Index + "]";
        }
    }
}