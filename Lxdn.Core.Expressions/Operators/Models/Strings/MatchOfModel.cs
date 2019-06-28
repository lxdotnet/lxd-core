using System;
using System.Xml;
using Lxdn.Core.Expressions.Extensions;

namespace Lxdn.Core.Expressions.Operators.Models.Strings
{
    [Obsolete]
    public class MatchOfModel : OperatorModel
    {
        public MatchOfModel() { }

        public MatchOfModel(XmlNode xml, OperatorModelFactory models)
        {
            this.Pattern = xml.GetMandatoryAttribute("pattern");
            this.Argument = models.CreateModel(xml.GetMandatoryNode("./*"));
        }

        public string Pattern { get; set; }

        public OperatorModel Argument { get; set; }
    }
}
