
using System;
using System.Linq;
using System.Xml;
using System.Reflection;
using System.Linq.Expressions;

using Lxd.Core.Extensions;
using Lxd.Core.Expressions.Extensions;
using Lxd.Core.Expressions.Exceptions;

namespace Lxd.Core.Expressions.Operators.Models
{
    [Operator("Compare")]
    public class BinaryComparerModel : OperatorModel
    {
        public BinaryComparerModel() { }

        public BinaryComparerModel(XmlNode xml, OperatorModelFactory models)
        {
            var allowedOperations = typeof(BinaryComparerModel).GetProperty("Operation")
                .GetCustomAttributes<AllowedValueAttribute>()
                .Select(attribute => attribute.Value).OfType<ExpressionType>().ToList();
            try
            {
                this.Operation = xml.GetMandatoryAttribute<ExpressionType>("if");
            }
            catch (Exception e)
            {
                string message = "The value of 'operator' attribute is invalid.";
                message = message + " Allowed values are " + allowedOperations.Select(operation => operation.ToString()).Agglutinate(", ");
                throw new ExpressionConfigException(message, e);
            }

            if (allowedOperations.All(operation => operation != this.Operation))
            {
                string message = "The following operator is not supported: " + this.Operation;
                throw new ExpressionConfigException(message);
            }

            XmlNodeList operators = xml.SelectNodes(@"./*");
            if (operators == null || operators.Count != 2)
                throw new ExpressionConfigException("The condition must have exactly two operands.");

            this.Left = models.CreateModel(operators[0]);
            this.Right = models.CreateModel(operators[1]);
        }

        [Mandatory]
        [AllowedValue(ExpressionType.Equal)]
        [AllowedValue(ExpressionType.GreaterThanOrEqual)]
        [AllowedValue(ExpressionType.GreaterThan)]
        [AllowedValue(ExpressionType.LessThanOrEqual)]
        [AllowedValue(ExpressionType.LessThan)]
        [AllowedValue(ExpressionType.NotEqual)]
        public ExpressionType? Operation { get; set; }

        public OperatorModel Left { get; set; }

        public OperatorModel Right { get; set; }
    }
}
