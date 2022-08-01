namespace ParkIt.Prototype.Common.Data
{

    public interface ISectionCapacity
    {
        public string Section { get; }

        public int AvailableCapacity { get; }

    }

    public class SectionCapacity : ISectionCapacity
    {

        public string Section { get; set; }

        public int AvailableCapacity { get; set; }

    }
}
