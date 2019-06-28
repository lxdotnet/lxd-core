using System;
using System.Xml;
using Lxdn.Core.Expressions.Extensions;

namespace Lxdn.Core.Expressions.Operators.Models
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
