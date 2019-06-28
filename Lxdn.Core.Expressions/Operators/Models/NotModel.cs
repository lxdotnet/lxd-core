using System.Xml;
using Lxdn.Core.Expressions.Extensions;

namespace Lxdn.Core.Expressions.Operators.Models
{
    public class NotModel : OperatorModel
    {
        public NotModel() { }

        public NotModel(XmlNode xml, OperatorModelFactory models)
        {
            this.Operand = models.CreateModel(xml.GetMandatoryNode("./*"));
        }

        public OperatorModel Operand { get; set; }
    }
}
