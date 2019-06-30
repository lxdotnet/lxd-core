
using System;
using System.Xml;

using Lxdn.Core.Expressions.Operators.Custom;
using Lxdn.Core.Expressions.Operators.Models;

namespace Lxdn.Core.Expressions._MSTests.Operators
{
    public class TestModel : OperatorModel
    {
        public TestModel(XmlNode xml, OperatorModelFactory models)
        {
            this.Nested = models.CreateModel(xml.SelectSingleNode("./*"));
        }

        public OperatorModel Nested { get; set; }
    }

    public class TestOperator : CustomOperator<string, IRuntimeContext>
    {
        private readonly Func<IRuntimeContext, string> logic;

        public TestOperator(TestModel model, ExecutionEngine engine) : base(engine)
        {
            this.logic = runtime => runtime.Resolve(engine.Create<string>(model.Nested));
        }

        protected override string Evaluate(IRuntimeContext runtime)
        {
            return this.logic.Invoke(runtime);
        }
    }
}