
using System.Runtime.Serialization;

namespace Lxdn.Core._MSTests.Domain
{
    public enum Country
    {
        [EnumMember(Value = "GL")] Global = 1,
        [EnumMember(Value = "EU")] Europe = 2,
        [EnumMember(Value = "AS")] Asia = 3
    }
}
