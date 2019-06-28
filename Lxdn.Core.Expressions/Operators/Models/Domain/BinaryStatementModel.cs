
namespace Lxdn.Core.Expressions.Operators.Models.Domain
{
    public class BinaryStatementModel : OperatorModel
    {
        public OperatorModel Subject { get; set; }
        public VerbModel Verb { get; set; }
        public OperatorModel Object { get; set; }
    }
}
