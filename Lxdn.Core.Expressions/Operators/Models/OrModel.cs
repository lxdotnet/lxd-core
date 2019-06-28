
using System.Xml;

namespace Lxdn.Core.Expressions.Operators.Models
{
    [Operator("Or")]
    public class OrModel : LogicalOperatorModel
    {
        public OrModel() { }

        public OrModel(XmlNode xml, OperatorModelFactory models) : base(xml, models) { }
    }
}
