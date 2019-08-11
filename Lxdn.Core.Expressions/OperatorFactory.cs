
using System;
using System.Xml;
using System.Linq;
using System.Reflection;

using Lxdn.Core.IoC;
using Lxdn.Core.Extensions;
using Lxdn.Core.Expressions.Operators;
using Lxdn.Core.Expressions.Operators.Models;
using Lxdn.Core.Expressions.Verbs;

namespace Lxdn.Core.Expressions
{
    public class OperatorFactory
    {
        private readonly ExecutionEngine engine;

        public OperatorFactory(ExecutionEngine engine, TypeResolver outer = null)
        {            
            this.engine = engine;
            
            //Dependencies = outer.Chain().Consider(engine);
            this.Dependencies = new TypeResolver(outer).Consider(engine);
            this.Models = new OperatorModelFactory(Dependencies);
            this.Verbs = new VerbFactory(engine);
        }

        public OperatorFactory(ExecutionEngine engine, OperatorModelFactory models, TypeResolver outer)
            : this(engine, outer)
        {
            this.Models = models;
        }

        public OperatorModelFactory Models { get; private set; }

        public Operator CreateFrom(OperatorModel model, Type desired = null)
        {
            var modelType = model.GetType();

            bool isModel(ParameterInfo parameter) => parameter.ParameterType.IsAssignableFrom(modelType);
            bool acceptsModel(ConstructorInfo ctor) => ctor.IsPublic && ctor.GetParameters().Count(isModel) == 1;

            // select single constructor that accepts a single model as a parameter:
            var constructor = this.Models.Sources
                .SelectMany(source => source.Assembly.GetTypes())
                .Where(type => typeof(Operator).IsAssignableFrom(type))
                .SelectMany(type => type.GetConstructors())
                .Where(acceptsModel).ToList()
                .ThrowIf(ctors => ctors.Count != 1, ctors => new ArgumentException($"Missing or ambiguous constructor for '{modelType.FullName}'"))
                .Single();

            lock (this.engine)
            {
                // derive a new scope of the dependency resolver and 
                // enrich it with the parameters from current scope:
                var resolver = this.Dependencies.Chain()
                    .Consider(modelType, model).Consider(desired ?? typeof(string));

                var args = constructor.GetParameters().Select(parameter => resolver.Resolve(parameter.ParameterType)).ToArray();
                var op = (Operator)constructor.Invoke(args);

                if (desired != null && op.Expression.Type != desired)
                    return op.As(desired);

                return op.Expression != null ? op : null; // a trick to force the creation of expression (which is lazy)
            }
        }

        public Operator CreateFrom(XmlNode xml, Type desired = null)
        {
            var model = this.Models.CreateModel(xml);
            return this.CreateFrom(model, desired);
        }

        public Property CreateProperty(string path)
        {
            return (Property) this.CreateFrom(new PropertyModel { Path = path });
        }

        public TypeResolver Dependencies { get; private set; }

        public VerbFactory Verbs { get; private set; }
    }
}
