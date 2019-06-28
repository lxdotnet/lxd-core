using System.Dynamic;
using System.Collections;
using System.Collections.Generic;

namespace Lxdn.Core.Basics
{
    public class CaseInsensitiveEnumerableExpando : DynamicObject, IEnumerable<DynamicObject>
    {
        private readonly List<DynamicObject> members = new List<DynamicObject>();

        public CaseInsensitiveEnumerableExpando Add(DynamicObject item)
        {
            this.members.Add(item);
            return this;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if ((int)indexes[0] < this.members.Count)
            {
                result = this.members[(int)indexes[0]];
                return true;
            }
                
            return base.TryGetIndex(binder, indexes, out result);
        }

        public IEnumerator<DynamicObject> GetEnumerator()
        {
            return members.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}