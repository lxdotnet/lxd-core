using System;
using System.Diagnostics;

namespace Lxd.Core.Basics
{
    [DebuggerDisplay("{Digest}")]
    internal class PropertyAccessor : IPropertyAccessor, IExplain
    {
        public PropertyAccessor(IProperty property, PropertyTrack track, object entry)
        {
            this.entry = entry;
            this.track = track;
            this.Property = property;
        }

        private readonly object entry;

        private readonly PropertyTrack track;

        public IProperty Property { get; private set; }

        private object GetPropertyValue(object o, int position)
        {
            if (this.track.Count == position)
                return o;

            // entry must be non-null for instance methods and null for static methods
            if (null == o)
            {
                string message = string.Format("The property {0} is null.", this.track[position - 1].Property.Name);
                throw new PropertyAccessorNullReferenceException(message, position - 1);
            }

            PropertyEntry propertyEntry = this.track[position];
            try
            {
                object val = propertyEntry.Property.GetValue(o, null);
                return this.GetPropertyValue(val, position + 1);
            }
            catch (Exception e)
            {
                throw new PropertyAccessorException(string.Format("Cannot get property '{0}'.", propertyEntry.Property.Name), e);
            }            
        }

        private void SetPropertyValue(object o, object value, int position)
        {
            PropertyEntry propertyEntry = this.track[position];

            if (this.track.Count == position + 1)
            {
                propertyEntry.Property.SetValue(o, value, null);
                return;
            }

            object val = propertyEntry.Property.GetValue(o, null);
            this.SetPropertyValue(val, value, position + 1);
        }

        public object Value
        {
            get { return this.GetPropertyValue(this.entry, 0); }
            set { this.SetPropertyValue(this.entry, value, 0); }
        }

        public string Digest
        {
            get
            {
                return "Get value of " + 
                    ((null != (this.Property as IExplain)) ? ((IExplain)this.Property).Digest : this.Property.ToString()) +
                    " for " + this.entry.GetType().Name; 
            }
        }
    }
}
