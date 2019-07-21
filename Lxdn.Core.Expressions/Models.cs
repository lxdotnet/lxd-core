using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lxdn.Core.Basics;
using Lxdn.Core.Expressions.Operators.Custom;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Expressions
{
    public class Models : IEnumerable<Model>
    {
        public Models(IEnumerable<Model> models)
        {
            this.models = new List<Model>(models);
            this.Browser = new TypeBrowser(this);
            this.RuntimeInjector = new RuntimeInjector(this);
        }

        private readonly List<Model> models;

        public Model this[string id] => this
            .FirstOrDefault(model => model.Id == id)
            .ThrowIfDefault(() => new ArgumentException($"Unknown model '{id}'", nameof(id)));

        public ClosureVariable CreateClosureVariable(string id, Type type)
        {
            return new ClosureVariable(id, type, this);
        }

        public IEnumerable<ParameterExpression> AsParameters()
        {
            return this.models.Select(model => model.AsParameter());
        }

        public void Add(Model model)
        {
            if (this.models.Any(m => m.Id.Equals(model.Id)))
                throw new ArgumentException("Duplicate model: " + model.Id);

            this.models.Add(model);
        }

        public void Remove(Model model)
        {
            this.models.Remove(model);
        }

        public IEnumerator<Model> GetEnumerator()
        {
            return this.models.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.models.GetEnumerator();
        }

        public Model[] ToArray()
        {
            return this.models.ToArray();
        }

        public TypeBrowser Browser { get; private set; }

        internal RuntimeInjector RuntimeInjector { get; private set; }
    }
}
