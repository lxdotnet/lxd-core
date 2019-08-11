
using System;
using System.Xml;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

using Lxdn.Core.IoC;
using Lxdn.Core.Basics;
using Lxdn.Core.Extensions;
using Lxdn.Core.Validation;

using Lxdn.Core.Expressions.Operators.Models;
using Lxdn.Core.Expressions.Operators.Models.Output;
using Emitter = Lxdn.Core.Expressions.OperatorModelEmitter;
using Lxdn.Core.Expressions.Extensions;

namespace Lxdn.Core.Expressions
{
    [DebuggerDisplay("Sources = {sources.Count}")]
    public class OperatorModelFactory : IEnumerable<Model>
    {
        private readonly TypeResolver resolver;

        private readonly List<OperatorSource> sources;

        private readonly Lazy<Emitter> emitter = new Lazy<Emitter>(() => new Emitter());               

        public OperatorModelFactory(TypeResolver resolver)
        {
            this.sources = new List<OperatorSource>();
            var validation = new ValidationContext(resolver, typeof(OperatorModel).IsAssignableFrom);
            this.Validator = validation.Create();
            this.Namespaces = new OperatorNamespaceMapper(this.sources);
            this.resolver = resolver;
        }

        public IEnumerable<OperatorSource> Sources => sources;

        public void Add(OperatorSource source) => sources.Add(source);

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

            //var local = resolver.Chain().Consider(xml, this);
            //var op = (OperatorModel) local.Resolve(model.Type);

            var op = model.Type.GetConstructor(new[] { typeof(XmlNode) }).IfExists(ctor => (OperatorModel)ctor.Invoke(new[] { xml }))
                  ?? model.Type.GetConstructor(new[] { typeof(XmlNode), typeof(OperatorModelFactory) }).IfExists(ctor => (OperatorModel)ctor.Invoke(new object[] { xml, this }));

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

        public IEnumerator<Model> GetEnumerator() => sources.SelectMany(models => models.Operators).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        //public IValidationContext Validator { get; private set; }
        public OperatorModelValidator Validator { get; private set; }

        internal Emitter Emitter => emitter.Value;

        public OperatorNamespaceMapper Namespaces { get; private set; }
    }
}
