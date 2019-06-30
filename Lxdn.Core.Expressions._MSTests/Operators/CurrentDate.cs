
using System;
using System.Xml;

using Lxdn.Core._MSTests.Domain;
using Lxdn.Core.Expressions.Operators.Custom;
using Lxdn.Core.Expressions.Operators.Models;

namespace Lxdn.Core.Expressions._MSTests.Operators
{
    [Operator("DateTime.Now")]
    public class CurrentDateOperatorModel : OperatorModel
    {
        public CurrentDateOperatorModel(XmlNode xml) { }
    }

    public class CurrentDate : CustomOperator<DateTime, Person>
    {
        public CurrentDate(CurrentDateOperatorModel model, ExecutionEngine logic) : base(logic) {}

        protected override DateTime Evaluate(Person person)
        {
            return DateTime.UtcNow;
        }
    }
}
