
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lxd.Core.Basics
{
    [DebuggerDisplay("{Track}")]
    public class PropertyAdapter : IProperty
    {
        public PropertyAdapter()
        {
            this.Track = PropertyTrack.Empty;
        }

        public PropertyAdapter(PropertyAdapter other)
            : this()
        {
            this.Track = new PropertyTrack(other.Track.ToList());
        }

        public PropertyAdapter(PropertyInfo property)
            : this()
        {
            this.Track.With(property);
        }

        public PropertyTrack Track { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public PropertyEntry LastOne
        {
            get { return this.Track.Last(); }
        }

        public Type Type
        {
            get { return this.LastOne.Property.PropertyType; }
        }

        public Expression ToExpression(Expression model)
        {
            return this.Track.Aggregate(model, (expression, entry) =>
            {
                MemberExpression member = Expression.Property(expression, entry.Property);

                if (!entry.HasIndex)
                    return member;

                IndexExpression index = Expression.MakeIndex(member, entry.Indexer,
                    new Expression[] { Expression.Constant(entry.Index) });

                return index;
            });
        }

        public IPropertyAccessor CreateAccessor(object entry)
        {
            var property = new Property(this.Track);
            return new PropertyAccessor(property, this.Track, entry);
        }
    }
}
