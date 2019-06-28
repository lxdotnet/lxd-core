
using System;
using System.Linq.Expressions;
using Lxd.Core.Basics;

namespace Lxd.Core.Expressions.Utils
{
    public class ObjectAdapter : IProperty
    {
        public ObjectAdapter(Type entry)
        {
            this.entry = entry;
        }

        private readonly Type entry;

        public Type Type
        {
            get { return this.entry.GetType(); }
        }

        public Expression ToExpression(Expression model)
        {
            return model;
        }

        public IPropertyAccessor CreateAccessor(object entry)
        {
            return new EntryAccessor(this, entry);
        }
    }

    public class EntryAccessor : IPropertyAccessor
    {
        private readonly ObjectAdapter root;

        private readonly object entry;

        public EntryAccessor(ObjectAdapter root, object entry)
        {
            this.root = root;
            this.entry = entry;
        }

        public object Value
        {
            get { return this.entry; }
            set { throw new NotSupportedException(); }
        }

        public IProperty Property
        {
            get { return this.root; }
        }
    }
}
