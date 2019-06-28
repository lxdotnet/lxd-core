using System.Xml;
using Lxdn.Core.Expressions.Extensions;
using Lxdn.Core.Validation;

namespace Lxdn.Core.Expressions.Operators.Models.Output
{
    public class ToStringModel : OperatorModel
    {
        public ToStringModel() { }

        public ToStringModel(XmlNode xml, OperatorModelFactory models)
        {
            this.Format = xml.GetAttributeOrDefault("format") ?? string.Empty;
            this.Culture = xml.GetAttributeOrDefault("culture");
            this.Argument = models.CreateModel(xml.SelectSingleNode(@"./*"));
        }

        public string Format { get; set; }

        [Optional]
        public string Culture { get; set; }

        public OperatorModel Argument { get; set; }
    }
}
