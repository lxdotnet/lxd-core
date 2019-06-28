using System;
using System.Linq;
using System.Reflection;

namespace Lxd.Core.Basics
{
    public class PropertyEntry
    {
        public PropertyEntry(PropertyInfo property, int index)
        {
            this.Property = property;
            this.Index = index;
        }

        public PropertyEntry(PropertyInfo property) : this(property, -1) {}

        public PropertyInfo Property { get; private set; }

        public int Index { get; private set; }

        public bool HasIndex
        {
            get { return this.Index != -1; }
        }

        public PropertyInfo Indexer
        {
            get
            {
                return this.Property.PropertyType.GetProperties()
                    .FirstOrDefault(p => p.GetIndexParameters().Any());
            }
        }

        public Type Type
        {
            get
            {
                if (!this.HasIndex)
                    return this.Property.PropertyType;

                PropertyInfo indexer = this.Indexer;

                if (indexer == null)
                    throw new InvalidOperationException("Could not apply index to the property " + this.Property.Name);

                return indexer.PropertyType;
            }
        }
    }
}
