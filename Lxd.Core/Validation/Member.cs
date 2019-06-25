
namespace Lxd.Validation
{
    public class Member : IStep
    {
        public Member(int index)
        {
            this.Index = index;
        }

        public int Index { get; }

        public override string ToString()
        {
            return $"[{Index}]";
        }
    }
}