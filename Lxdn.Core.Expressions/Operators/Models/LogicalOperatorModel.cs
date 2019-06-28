using System.Xml;
using System.Linq;
using System.Collections.Generic;

using Lxdn.Core.Validation;
using Lxdn.Core.Expressions.Operators.Models.Validation.OfModels;

namespace Lxdn.Core.Expressions.Operators.Models
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
