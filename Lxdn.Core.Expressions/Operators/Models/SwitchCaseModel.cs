
using System.Xml;
using Lxdn.Core.Expressions.Extensions;

namespace Lxdn.Core.Expressions.Operators.Models
{
    [Operator("SwitchCase", Hidden = true)]
    public class SwitchCaseModel : OperatorModel
    {
        public SwitchCaseModel() { }

        public SwitchCaseModel(XmlNode xml, OperatorModelFactory models)
        {
            this.When = xml.GetMandatoryAttribute("when");
            this.Operator = models.CreateModel(xml.SelectSingleNode("./*"));
        }

        public string When { get; set; }

        public OperatorModel Operator { get; set; }
    }
}
