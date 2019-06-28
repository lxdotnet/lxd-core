using System.Xml;

namespace Lxdn.Core.Expressions.Operators.Models.Linq
{
    [Operator("Linq.Max")]
    [Operator("Linq.Min")]
    [Operator("Linq.Sum")]
    [Operator("Linq.Last")]
    [Operator("Linq.Average")]
    public class LinqScalarModel : LinqBaseModel
    {
        public LinqScalarModel() { }
        public LinqScalarModel(XmlNode xml, OperatorModelFactory models) : base(xml, models) { }
    }
}