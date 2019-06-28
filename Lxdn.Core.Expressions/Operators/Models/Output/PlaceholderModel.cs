using System.Diagnostics;
using System.Xml;
using Lxdn.Core.Expressions.Extensions;

namespace Lxdn.Core.Expressions.Operators.Models.Output
{
    [DebuggerDisplay("[{Id,nq}]")]
    [Operator("Placeholder", Hidden = true)]
    public class PlaceholderModel : OperatorModel
    {
        public PlaceholderModel() { }

        public PlaceholderModel(XmlNode xml, OperatorModelFactory models)
        {
            this.Id = xml.GetMandatoryAttribute("of");
            this.Operator = models.CreateModel(xml.GetMandatoryNode("./*"));
        }

        public string Id { get; set; }

        public OperatorModel Operator { get; set; }
    }
}
