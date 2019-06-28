
namespace Lxdn.Annotations
{
    public class Root : IStep
    {
        public Root(object value)
        {
            this.Value = value;
        }

        public object Value { get; }

        public override string ToString()
        {
            return Value.GetType().Name;
        }
    }
}