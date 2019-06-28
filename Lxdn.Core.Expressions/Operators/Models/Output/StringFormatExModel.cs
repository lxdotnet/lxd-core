using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Lxdn.Core.Expressions.Operators.Models.Output
{
    /// <summary>
    /// For compatibility with old bidding service logic.
    /// </summary>
    public class StringFormatExModel : OperatorModel
    {
        public StringFormatExModel() { }

        public StringFormatExModel(XmlNode xml, OperatorModel format, OperatorModelFactory models)
        {
            this.Format = format;
            this.Placeholders = xml.SelectNodes("./Placeholder").OfType<XmlNode>()
                .Select(a => new PlaceholderModel(a, models)).ToList();
        }

        public OperatorModel Format { get; set; }

        public IEnumerable<PlaceholderModel> Placeholders { get; set; }
    }
}
