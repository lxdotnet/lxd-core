using System.Xml;
using System.Linq;
using Lxd.Core.Expressions.Extensions;
using Lxd.Core.Expressions.Exceptions;

namespace Lxd.Core.Expressions.Operators.Models.Linq
{
    [Operator("Linq.All")]
    [Operator("Linq.Any")]
    [Operator("Linq.Count")]
    [Operator("Linq.First")]
    [Operator("Linq.Where")]
    [Operator("Linq.Select")]
    [Operator("Linq.Aggregate")]
    [Operator("Linq.FirstOrDefault")]
    public class LinqLambdaModel : LinqBaseModel
    {
        public LinqLambdaModel() { }

        public LinqLambdaModel(XmlNode xml, OperatorModelFactory models)
            : base(xml, models)
        {
            var lambda = xml.SelectSingleNode("./Lambda");

            if (lambda != null)
            {
                this.Parameters = lambda.GetMandatoryAttribute("capture");//.SplitBy(' ', ',').ToList();

                if (!this.Parameters.Any())
                    throw new ExpressionConfigException("Inner lambda contains no closure variables");

                this.Body = models.CreateModel(lambda.SelectSingleNode("./*"));
            }
        }

        public string Parameters { get; set; }

        public OperatorModel Body { get; set; }
    }
}