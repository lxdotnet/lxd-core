
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Lxdn.Core.Extensions;
using Lxdn.Core.Validation;
using Lxdn.Core.Expressions.Extensions;

namespace Lxdn.Core.Expressions.Operators.Models
{
    public class ThrowModel : OperatorModel
    {
        public ThrowModel() { }

        public ThrowModel(XmlNode xml, OperatorModelFactory models)
        {
            //var message = xml.GetAttributeOrDefault("message");

            //this.Message = !string.IsNullOrEmpty(message) 
            //    ? new ConstModel(message) : 
            //    models.CreateModel(xml.SelectSingleNode(@"./*"));

            this.Message = xml.SelectSingleNode(@"./Message/*").IfExists(m => models.CreateModel(m)) ??
                models.CreateModel(xml.SelectSingleNode(@"./*"));

            this.Type = xml.GetAttributeOrDefault("type");

            this.Arguments = xml.SelectNodes(@"./Arguments/*").OfType<XmlNode>().Select(models.CreateModel).ToList();
        }

        public OperatorModel Message { get; set; }

        [Optional]
        public string Type { get; set; }

        [Optional]
        public IEnumerable<OperatorModel> Arguments { get; set; }
    }
}
