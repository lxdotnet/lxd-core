using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Lxdn.Core.Expressions.Operators.Models
{
    [Operator("Sequence")]
    public class CodeBlockModel : OperatorModel
    {
        public CodeBlockModel() { }

        public CodeBlockModel(XmlNode xml, OperatorModelFactory models)
        {
            this.Operators = xml.SelectNodes("./*").OfType<XmlNode>()
                .Select(models.CreateModel).ToList();
        }

        public IEnumerable<OperatorModel> Operators { get; set; }
    }
}
