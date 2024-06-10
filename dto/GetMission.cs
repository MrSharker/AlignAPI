namespace AlignAPI.dto
{
    public class GetMission
    {
        public int Id { get; set; }
        public string Agent { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public DateTime Date { get; set; }
    }
}
