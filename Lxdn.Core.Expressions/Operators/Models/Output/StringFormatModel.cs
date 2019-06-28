using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Lxdn.Core.Expressions.Extensions;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Expressions.Operators.Models.Output
{
    [Operator("String.Format")]
    public class StringFormatModel : OperatorModel
    {
        public StringFormatModel() { }

        public StringFormatModel(XmlNode xml, OperatorModelFactory models)
        {
            var constant = new ConstModel(xml.GetMandatoryAttribute("pattern").SquareTagsToHtml());
            this.Arguments = xml.SelectNodes("./*").OfType<XmlNode>().Select(models.CreateModel).ToList();

            if (constant.Value.MatchesOf(@"\{.+?\}").Count() != this.Arguments.Count())
                throw new XmlConfigException("The count of arguments in the format string does not match the count of parameters");

            this.Format = constant;
        }

        public OperatorModel Format { get; set; }

        public IEnumerable<OperatorModel> Arguments { get; set; }
    }
}
