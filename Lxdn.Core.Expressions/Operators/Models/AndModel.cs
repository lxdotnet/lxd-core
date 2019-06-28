using System.Xml;

namespace Lxdn.Core.Expressions.Operators.Models
{
    public class AndModel : LogicalOperatorModel
    {
        public AndModel() {}

        public AndModel(XmlNode xml, OperatorModelFactory models) : base(xml, models) { }
    }
}