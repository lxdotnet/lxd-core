
using System;
using System.Xml;
using System.Linq;

using Lxd.Core.Expressions.Operators;
using Lxd.Core.Expressions.Operators.Models;
using Lxd.Core.Expressions.Verbs;

namespace Lxd.Core.Expressions
{
    public class OperatorFactory
    {
        private readonly ExecutionEngine engine;

        public OperatorFactory(ExecutionEngine engine)
        {
            this.engine = engine;
            this.Models = new OperatorModelFactory();
            this.Dependencies = new DependencyResolver().Consider(() => engine);
            this.Verbs = new VerbFactory(engine);
        }

        public OperatorFactory(ExecutionEngine engine, OperatorModelFactory models)
            : this(engine)
        {
            this.Models = models;
        }

        public OperatorModelFactory Models { get; private set; }

        public Operator CreateFrom(OperatorModel model, Type desired = null)
        {
            var modelType = model.GetType();

            // select single constructor that accepts a single model as a parameter:
            var constructor = this.Models.Sources.SelectMany(source => source.Assembly.GetTypes())
                .Where(type => typeof(Operator).IsAssignableFrom(type))
                .SelectMany(type => type.GetConstructors())
                .Where(ctor => ctor.IsPublic)
                .SingleOrDefault(ctor => ctor.GetParameters().SingleOrDefault(parameter => parameter.ParameterType.IsAssignableFrom(modelType)) != null);

            if (constructor == null)
                throw new ArgumentException("Cannot construct the operator for the model " + modelType.FullName);

            lock (this.engine)
            {
                // derive a new scope of the dependency resolver and 
                // enrich it with the parameters from current scope:
                var resolver = this.Dependencies.Chain()
                    .Consider(modelType, () => model)
                    .Consider(typeof(Type), () => desired);

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

        public DependencyResolver Dependencies { get; private set; }

        public VerbFactory Verbs { get; private set; }
    }
}
