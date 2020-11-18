
using System;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;
using System.Globalization;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Basics
{
    public class CaseInsensitiveExpando : DynamicObject
    {
        private readonly Dictionary<string, object> values;

        public CaseInsensitiveExpando()
        {
            values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public CaseInsensitiveExpando(IDictionary<string, object> values) : this()
        {
            values.Aggregate(this.values, (acc, pair) => { acc.Add(pair.Key, pair.Value); return acc; });
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Set(binder.Name, value);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            values.TryGetValue(binder.Name, out result);
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length != 1)
                throw new ArgumentException("Invalid number of index parameters", "indexes");

            var property = indexes[0] as string;
            if (property == null)
                throw new ArgumentException("Invalid type of index parameter", "indexes");

            result = this.values.ContainsKey(property) ? this.values[property] : null;
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length != 1)
                throw new ArgumentException("Invalid number of index parameters", "indexes");

            var property = indexes[0] as string;
            if (property == null)
                throw new ArgumentException("Invalid type of index parameter", "indexes");

            Set(property, value);
            return true;
        }

        public CaseInsensitiveExpando Set(string name, object value)
        {
            values[name] = value;
            return this;
        }

        public object Get(string name)
        {
            if (!values.ContainsKey(name))
                throw new ArgumentException("Unknown key: " + name, "name");

            return values[name];
        }

        public TReturn Get<TReturn>(string name, CultureInfo culture) => Get(name).ChangeType<TReturn>(culture);

        public TReturn Get<TReturn>(string name) => Get<TReturn>(name, CultureInfo.InvariantCulture);

        public override IEnumerable<string> GetDynamicMemberNames() => values.Keys;

        public IDictionary<string, object> AsDictionary() => values;
    }
}
