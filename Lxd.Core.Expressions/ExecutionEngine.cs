using System.Xml;
using System.Reflection;
using Lxd.Core.Basics;
using Lxd.Core.Expressions.Operators.Models;

namespace Lxd.Core.Expressions
{
    public class ExecutionEngine
    {
        public ExecutionEngine(params Model[] models)
        {
            this.Models = new Models(models);
            this.Cache = new OperatorCache();
            this.Operators = new OperatorFactory(this);
            this.Operators.Models.Parse(Assembly.GetExecutingAssembly());
        }

        private ExecutionEngine() { }

        public OperatorCache Cache { get; private set; }

        public Models Models { get; private set; }

        public OperatorFactory Operators { get; private set; }

        internal ExecutionEngine Clone()
        {
            var clone = new ExecutionEngine { Models = new Models(this.Models) };
            clone.Operators = new OperatorFactory(clone, this.Operators.Models);
            return clone;
        }

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
