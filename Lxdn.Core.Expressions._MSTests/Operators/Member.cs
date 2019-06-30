
using System.Xml;
using System.Linq.Expressions;

using Lxdn.Core.Expressions.Operators.Models;

namespace Lxdn.Core.Expressions._MSTests.Operators
{
    public abstract class MemberModel : OperatorModel
    {
        protected MemberModel(XmlNode xml) {}

        public string Id { get; set; }
    }

    [Operator("TestGroup")]
    public class GroupModel : MemberModel
    {
        public GroupModel(XmlNode xml) : base(xml) {}
    }

    public class Member : Operator
    {
        public Member(MemberModel model) {}

        protected override Expression Create()
        {
            return Expression.Constant("Test", typeof(string));
        }
    }
}
