
using System;

namespace Lxdn.Core._MSTests.Domain
{
    public class TestEntity : IEquatable<TestEntity>
    {
        public string Id { get; set; }

        public bool Equals(TestEntity other)
        {
            return this.Id.Equals(other.Id);
        }
    }

}
