using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lxdn.Core.Basics
{
    [DebuggerDisplay("{Digest}")]
    public class Property : IProperty, IExplain
    {
        public Property()
        {
            this.Track = PropertyTrack.Empty;
        }

        public Property(Property other)
            : this()
        {
            this.Track = new PropertyTrack(other.Track.ToList());
        }

        public Property(PropertyInfo property)
            : this()
        {
            this.Track.With(property);
        }

        internal Property(PropertyTrack track)
        {
            this.Track = track;
        }

        public virtual IPropertyAccessor CreateAccessor(object entry)
        {
            return new PropertyAccessor(this, this.Track, entry);
        }

        internal PropertyTrack Track { get; private set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public PropertyEntry LastOne
        {
            get { return this.Track[this.Track.Count - 1]; }
        }

        public virtual Type Type
        {
            get { return 0 == this.Track.Count ? null : this.LastOne.Property.PropertyType; }
        }

        public static Property operator +(Property p1, Property p2)
        {
            Property left = new Property(p1);
            p2.Track.ToList().ForEach(right => left.Track.With(right));
            return left;
        }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Digest
        {
            get { return this.Track.ToString(); }
        }

        public Expression ToExpression(Expression parameter)
        {
            return this.Track.Aggregate(parameter, (expression, entry) =>
                Expression.Property(expression, entry.Property));
        }
    }
}
