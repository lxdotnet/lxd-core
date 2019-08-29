using System.Xml;
using System.Reflection;

using Lxdn.Core.IoC;
using Lxdn.Core.Basics;
using Lxdn.Core.Expressions.Operators.Models;

namespace Lxdn.Core.Expressions
{
    public class ExecutionEngine
    {
        public ExecutionEngine(IChainableResolver resolver, params Model[] models)
        {
            this.Models = new Models(models);
            this.Operators = new OperatorFactory(this, resolver ?? new Resolver(this));
            this.Operators.Models.Parse(Assembly.GetExecutingAssembly());
        }

        public ExecutionEngine(params Model[] models) : this(null, models) { }

        public Models Models { get; }

        public OperatorFactory Operators { get; }

        public IEvaluator<TReturn> Create<TReturn>(OperatorModel model)
        {
            return this.Operators.CreateFrom(model).ToEvaluator<TReturn>(this.Models);
        }

        public IEvaluator<TReturn> Create<TReturn>(XmlNode xml)
        {
            return this.Operators.CreateFrom(xml).ToEvaluator<TReturn>(this.Models);
        }
    }
}
