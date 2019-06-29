namespace Lxdn.Core._MSTests.Domain
{
    public class SportVehicle : TestVehicle
    {
        public SportVehicle(string id) : base(id) { }

        public SportVehicle(SportVehicle sv) : base(sv) { }
    }
}
