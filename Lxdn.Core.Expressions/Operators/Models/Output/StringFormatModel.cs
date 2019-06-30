
using System;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Lxdn.Core.Extensions;
using Lxdn.Core.Expressions.Extensions;

namespace Lxdn.Core.Expressions.Operators.Models.Output
{
    [Operator("String.Format")]
    public class StringFormatModel : OperatorModel
    {
        public StringFormatModel() { }

        public StringFormatModel(XmlNode xml, OperatorModelFactory models)
        {
            Func<string, string> squareTagsToHtml = s => 
                Regex.Replace(s, @"\[(?'tag'.+?)\]", match =>
                    string.Format("<{0}>", match.Groups["tag"].Value));

            var pattern = squareTagsToHtml(xml.GetMandatoryAttribute("pattern"));

            var constant = new ConstModel(pattern);
            this.Arguments = xml.SelectNodes("./*").OfType<XmlNode>().Select(models.CreateModel).ToList();

            if (constant.Value.MatchesOf(@"\{.+?\}").Count() != this.Arguments.Count())
                throw new XmlConfigException("The count of arguments in the format string does not match the count of parameters");

            this.Format = constant;
        }

        public OperatorModel Format { get; set; }

        public IEnumerable<OperatorModel> Arguments { get; set; }
    }
}
