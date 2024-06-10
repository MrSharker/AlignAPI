using NetTopologySuite.Geometries;
namespace AlignAPI.DB.Entities
{
    public class Mission
    {
        public int Id { get; set; }
        public string Agent { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public DateTime Date { get; set; }
        public Point Location { get; set; }
    }
}
