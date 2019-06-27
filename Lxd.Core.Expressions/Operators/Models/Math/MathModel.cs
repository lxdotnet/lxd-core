using System;
using System.Xml;
using System.Linq;
using System.Linq.Expressions;
using Lxd.Core.Basics;
using Lxd.Core.Expressions.Exceptions;

namespace Lxd.Core.Expressions.Operators.Models.Math
{
    [Operator("Math.Add")]
    [Operator("Math.Divide")]
    [Operator("Math.Modulo")]
    [Operator("Math.Multiply")]
    [Operator("Math.Power")]
    [Operator("Math.Subtract")]
    public class MathModel : OperatorModel
    {
        public MathModel() { }

        public MathModel(XmlNode xml, OperatorModelFactory models)
        {
            var nodes = new Pair<XmlNode>(xml.SelectNodes("./*").OfType<XmlNode>().ToList());

            this.Left = models.CreateModel(nodes.Left);
            this.Right = models.CreateModel(nodes.Right);

            string operation = xml.Name.Split('.')[1];

            if (!Enum.IsDefined(typeof(ExpressionType), operation))
                throw new ExpressionConfigException(operation + " is unknown binary expression type");

            this.Operation = (ExpressionType)Enum.Parse(typeof(ExpressionType), operation);
        }

        public OperatorModel Left { get; set; }

        public OperatorModel Right { get; set; }

        [AllowedValue(ExpressionType.Add)]
        [AllowedValue(ExpressionType.Subtract)]
        [AllowedValue(ExpressionType.Divide)]
        [AllowedValue(ExpressionType.Multiply)]
        [AllowedValue(ExpressionType.Modulo)]
        [AllowedValue(ExpressionType.Power)]
        public ExpressionType Operation { get; set; }
    }
}
