
using System.Xml;
using Lxdn.Core.Expressions.Extensions;

namespace Lxdn.Core.Expressions.Operators.Models
{
    [Operator("LocalVariable")]
    public class LetModel : OperatorModel
    {
        public LetModel() {}

        public LetModel(XmlNode xml, OperatorModel operand, OperatorModelFactory models)
        {
            this.Operand = operand;
            this.Variable = xml.GetMandatoryAttribute("as");
        }

        public OperatorModel Operand { get; set; }

        public string Variable { get; set; }
    }
}
