using System.Xml;

using Lxdn.Core.Expressions.Extensions;
using Lxdn.Core.Validation;

namespace Lxdn.Core.Expressions.Operators.Models
{
    public class IfModel : OperatorModel
    {
        public IfModel() { }

        public IfModel(XmlNode xml, OperatorModelFactory models)
        {
            this.Condition = models.CreateModel(xml.GetMandatoryNode(@"./Condition/*"));
            this.Then = models.CreateModel(xml.GetMandatoryNode(@"./Then/*"));

            XmlNode @else = xml.SelectSingleNode(@"./Else/*");
            if (@else != null)
                this.Else = models.CreateModel(@else);
        }

        public OperatorModel Condition { get; set; }

        public OperatorModel Then { get; set; }

        [Optional]
        public OperatorModel Else { get; set; }
    }
}
