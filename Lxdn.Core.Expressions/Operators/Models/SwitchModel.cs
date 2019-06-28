
using System.Xml;
using System.Linq;
using System.Collections.Generic;

using Lxdn.Core.Validation;
using Lxdn.Core.Expressions.Extensions;

namespace Lxdn.Core.Expressions.Operators.Models
{
    public class SwitchModel : OperatorModel
    {
        public SwitchModel() { }

        public SwitchModel(XmlNode xml, OperatorModelFactory models)
        {
            this.Key = xml.GetMandatoryAttribute("key");

            this.Cases = xml.SelectNodes("./SwitchCase").OfType<XmlNode>()
                .Select(c => new SwitchCaseModel(c, models)).ToList();

            var @default = xml.SelectSingleNode("./Default/*");
            if (@default != null)
                this.Default = models.CreateModel(@default);
        }

        public string Key { get; set; }

        public IEnumerable<SwitchCaseModel> Cases { get; set; }

        [Optional]
        public OperatorModel Default { get; set; }
    }
}
