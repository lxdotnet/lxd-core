using System.Xml;
using Lxdn.Core.Expressions.Extensions;

namespace Lxdn.Core.Expressions.Operators.Models.Linq
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
