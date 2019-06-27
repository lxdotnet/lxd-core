
using System;
using System.Xml;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

using Lxd.Core.Basics;
using Lxd.Core.Extensions;
using Lxd.Core.Validation;

using Lxd.Core.Expressions.Operators.Models;
using Lxd.Core.Expressions.Operators.Models.Output;
using Emitter = Lxd.Core.Expressions.OperatorModelEmitter;
using Lxd.Core.Expressions.Extensions;

namespace Lxd.Core.Expressions
{
    [DebuggerDisplay("Sources = {sources.Count}")]
    public class OperatorModelFactory : IEnumerable<Model>
    {
        private readonly List<OperatorSource> sources;

        public OperatorModelFactory()
        {
            this.sources = new List<OperatorSource>();
            var validation = new ValidationContext(t => typeof(OperatorModel).IsAssignableFrom(t));
            this.Validator = validation.Create();
            this.Namespaces = new OperatorNamespaceMapper(this.sources);
        }

        public IReadOnlyCollection<OperatorSource> Sources
        {
            get { return this.sources.AsReadOnly(); }
        }

        public void Add(OperatorSource source)
        {
            this.sources.Add(source);
        }

        public void Parse(Assembly assembly)
        {
            List<Model> operators = assembly.GetTypes()
                .Where(candidate => typeof(OperatorModel).IsAssignableFrom(candidate) && !candidate.IsAbstract)
                .SelectMany(this.ModelsFromType).ToList();

            if (operators.Any())
                this.Add(new OperatorSource(assembly, operators));
        }

        private IEnumerable<Model> ModelsFromType(Type prototype)
        {
            var operators = prototype.GetCustomAttributes<OperatorAttribute>(false)
                .DefaultIfEmpty(new OperatorAttribute(prototype.Name.Strip("Model"))).ToList();

            if (operators.Count == 1)
                return new Model(operators.Single().Value, prototype).Once();

            return operators
                .Select(o => o.Value)
                .Select(id => new { Id = id, FullName = prototype.Namespace + "." + id.CSharpify() })
                .Select(op => new { op.Id, Type = Emitter.TryLoad(op.FullName) ?? Emitter.DeriveFrom(prototype, op.FullName) })
                .Select(derived => new Model(derived.Id, derived.Type));
        }

        public OperatorModel CreateModel(XmlNode xml)
        {
            var model = this.FirstOrDefault(m => m.Id == xml.Name);

            if (model == null)
                throw new ArgumentException("Unknown operator model: " + xml.Name);

            var op = model.Type.GetConstructor(new[] { typeof(XmlNode) }).IfExists(ctor => (OperatorModel)ctor.Invoke(new [] { xml }))
                  ?? model.Type.GetConstructor(new[] { typeof(XmlNode), typeof(OperatorModelFactory) }).IfExists(ctor => (OperatorModel)ctor.Invoke(new object [] { xml, this }));
            
            if (op == null)
                throw new InvalidOperationException("No ctor found to construct from xml: " + model.Type.FullName);

            // below are workarounds for the compatibility with the xml of the old bidding service:
            if (xml.SelectNodes("./Placeholder").OfType<XmlNode>().Any())
            {
                op = new StringFormatExModel(xml, op, this);
            }

            if (!String.IsNullOrEmpty(xml.GetAttributeOrDefault("as")))
            {
                op = new LetModel(xml, op, this);
            }

            return op;
        }

        public IEnumerator<Model> GetEnumerator()
        {
            // enumerate all operators across all available sources
            return this.sources.SelectMany(models => models.Operators).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        //public IValidationContext Validator { get; private set; }
        public OperatorModelValidator Validator { get; private set; }

        private readonly Lazy<OperatorModelEmitter> emitter = new Lazy<OperatorModelEmitter>(() => new OperatorModelEmitter());

        internal OperatorModelEmitter Emitter
        {
            get { return emitter.Value; }
        }

        public OperatorNamespaceMapper Namespaces { get; private set; }
    }
}
