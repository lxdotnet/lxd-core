using System;
using Lxdn.Core.Basics;

namespace Lxdn.Core.Expressions
{
    public class ClosureVariable : Model, IDisposable
    {
        internal ClosureVariable(string id, Type type, Models models) : base(id, type)
        {
            this.models = models;
            this.models.Add(this);
        }

        private readonly Models models;

        public void Dispose()
        {
            this.models.Remove(this);
        }
    }
}
