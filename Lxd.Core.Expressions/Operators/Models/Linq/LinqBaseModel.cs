using System.Xml;
using Lxd.Core.Expressions.Extensions;

namespace Lxd.Core.Expressions.Operators.Models.Linq
{
    public abstract class LinqBaseModel : OperatorModel
    {
        protected LinqBaseModel() { }

        protected LinqBaseModel(XmlNode xml, OperatorModelFactory models)
        {
            this.Enumerable = xml.GetAttributeOrDefault("in") ?? xml.GetAttributeOrDefault("of");
            if (string.IsNullOrEmpty(this.Enumerable))
                throw new XmlConfigException("The linq target collection must be referenced by 'in' of 'of' attribute.");

            this.Verb = xml.Name.Split('.')[1];
        }

        public string Enumerable { get; set; }

        public string Verb { get; set; }
    }
}
