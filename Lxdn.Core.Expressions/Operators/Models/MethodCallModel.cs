using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Lxdn.Core.Expressions.Extensions;

namespace Lxdn.Core.Expressions.Operators.Models
{
    [Operator("Call")]
    public class MethodCallModel : OperatorModel
    {
        public MethodCallModel() { }

        public MethodCallModel(XmlNode xml, OperatorModelFactory models)
        {
            this.Target = xml.GetMandatoryAttribute("for");
            this.Method = xml.GetMandatoryAttribute("method");

            this.Arguments = xml.SelectNodes("./*").OfType<XmlNode>()
                .Select(models.CreateModel).ToList();
        }

        public string Target { get; set; } // can be a type, e.g. "System.String" for static calls. Otherwise a property. // todo: make it be an operator

        public string Method { get; set; }

        public IEnumerable<OperatorModel> Arguments { get; set; }
    }
}
