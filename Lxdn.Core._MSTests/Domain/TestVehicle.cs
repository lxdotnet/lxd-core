namespace Lxdn.Core._MSTests.Domain
{
    public class TestVehicle
    {
        public TestVehicle(string id)
        {
            this.Id = id;
        }

        public TestVehicle(SportVehicle sv) { }

        public string Id { get; set; }
    }
}
