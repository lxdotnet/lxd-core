
using System;
using System.Dynamic;
using System.Collections.Generic;

namespace Lxdn.Core.Basics
{
    public class CaseInsensitiveExpando : DynamicObject
    {
        private readonly Dictionary<string, object> values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.Set(binder.Name, value);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            this.values.TryGetValue(binder.Name, out result);
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

            this.Set(property, value);
            return true;
        }

        public CaseInsensitiveExpando Set(string name, object value)
        {
            this.values[name] = value;
            return this;
        }

        public override IEnumerable<string> GetDynamicMemberNames() => values.Keys;
    }
}
