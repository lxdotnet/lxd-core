using System.Xml;
using System.Linq;
using System.Collections.Generic;

using Lxd.Core.Validation;
using Lxd.Core.Expressions.Operators.Models.Validation.OfModels;

namespace Lxd.Core.Expressions.Operators.Models
{
    [Validate(typeof(LogicalOperatorValidator))]
    public abstract class LogicalOperatorModel : OperatorModel
    {
        protected LogicalOperatorModel() {}

        protected LogicalOperatorModel(XmlNode xml, OperatorModelFactory models)
        {
            this.Operands = xml.SelectNodes(@"./*").OfType<XmlNode>()
                .Select(models.CreateModel).ToList();
        }

        public IEnumerable<OperatorModel> Operands { get; set; }
    }
}
