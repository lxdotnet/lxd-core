using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lxd.Core.Basics
{
    public class PropertyTrack : IEnumerable<PropertyEntry>
    {
        public static PropertyTrack Empty
        {
            get { return new PropertyTrack(); }
        }

        private PropertyTrack()
        {
            this.entries = new List<PropertyEntry>();
        }

        public PropertyTrack(IEnumerable<PropertyEntry> entries) : this()
        {
            this.entries.AddRange(entries);
        }

        private readonly List<PropertyEntry> entries;

        public PropertyEntry this[int position]
        {
            get { return this.entries[position]; }
        }

        public int Count
        {
            get { return this.entries.Count; }
        }

        public PropertyTrack With(PropertyInfo property)
        {
            PropertyEntry entry = new PropertyEntry(property);
            return this.With(entry);
        }

        public PropertyTrack With(PropertyEntry entry)
        {
            this.entries.Add(entry);
            return this;
        }

        public PropertyTrack With(PropertyInfo property, int index)
        {
            PropertyEntry entry = new PropertyEntry(property, index);
            return this.With(entry);
        }

        public override string ToString()
        {
            return string.Join(".", this.entries.Select(entry =>
                    string.Format("{0}{1}", entry.Property.Name, 
                    entry.HasIndex ? string.Format("[{0}]", entry.Index) : "")).ToArray());
        }

        public IEnumerator<PropertyEntry> GetEnumerator()
        {
            return this.entries.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
