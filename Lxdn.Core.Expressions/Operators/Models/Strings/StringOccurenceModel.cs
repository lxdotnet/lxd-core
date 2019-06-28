
using System.Xml;
using Lxd.Core.Expressions.Extensions;

namespace Lxd.Core.Expressions.Operators.Models.Strings
{
    [Operator("String.Occurence")]
    public class StringOccurenceModel : OperatorModel
    {
        public StringOccurenceModel() {}

        public StringOccurenceModel(XmlNode xml, OperatorModelFactory models)
        {
            this.Source = models.CreateModel(xml.GetMandatoryNode(@"./*"));
            this.Value = new ConstModel(xml.GetMandatoryAttribute("of"));
            this.Occurence = xml.GetMandatoryAttribute<StringOccurenceKind>("predicate");
        }

        public OperatorModel Source { get; set; }

        public StringOccurenceKind? Occurence { get; set; }

        public OperatorModel Value { get; set; }
    }
}
