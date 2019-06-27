using System.Xml;
using Lxd.Core.Expressions.Extensions;

namespace Lxd.Core.Expressions.Operators.Models.Strings
{
    [Operator("Regex.Match")]
    public class MatchesOfModel : OperatorModel
    {
        public MatchesOfModel() { }

        public MatchesOfModel(XmlNode xml, OperatorModelFactory models)
        {
            this.Pattern = xml.GetMandatoryAttribute("pattern");
            this.Argument = models.CreateModel(xml.SelectSingleNode("./*"));
        }

        public string Pattern { get; set; }

        public OperatorModel Argument { get; set; }
    }
}
