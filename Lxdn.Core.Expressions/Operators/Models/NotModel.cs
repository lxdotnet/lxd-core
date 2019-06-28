using System.Xml;
using Lxd.Core.Expressions.Extensions;

namespace Lxd.Core.Expressions.Operators.Models
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
