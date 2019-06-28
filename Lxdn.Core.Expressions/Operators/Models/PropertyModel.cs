using System;
using System.Xml;
using Lxd.Core.Expressions.Extensions;

namespace Lxd.Core.Expressions.Operators.Models
{
    [Operator("Property", Important = true)]
    public class PropertyModel : OperatorModel
    {
        public PropertyModel() { }

        public PropertyModel(XmlNode xml, OperatorModelFactory models)
        {
            this.Path = xml.GetMandatoryAttribute("valueOf");
        }

        public PropertyModel(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            this.Path = path;
        }

        public string Path { get; set; }
    }
}
