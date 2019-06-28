
using System.Xml;
using Lxd.Core.Expressions.Exceptions;

namespace Lxd.Core.Expressions.Operators.Models
{
    [Operator("Constant", Important = true)]
    public class ConstModel : OperatorModel
    {
        public ConstModel() { }

        public ConstModel(XmlNode xml, OperatorModelFactory models)
        {
            XmlNode constant = xml.Attributes["value"];
            if (null == constant) // allow empty
                throw new ExpressionConfigException("The value of a constant is absent.");

            this.Value = constant.Value;
        }

        public ConstModel(string value)
        {
            this.Value = value;
        }

        public string Value { get; set; }
    }
}
