using System.Collections.Generic;

namespace Lxd.Core.Expressions
{
    public class OperatorCache
    {
        public OperatorCache()
        {
            this.collection = new Dictionary<string, Operator>();
        }

        private readonly Dictionary<string, Operator> collection;

        public bool Contains(string id)
        {
            return this.collection.ContainsKey(id);
        }

        public Operator this[string id]
        {
            get { return this.collection[id]; }
        }

        public void Add(string id, Operator op)
        {
            if (this.Contains(id))
                this.collection[id] = op;
            else
            {
                this.collection.Add(id, op);
            }
        }
    }
}
